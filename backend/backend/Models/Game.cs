namespace Backend.Models;

public struct SetCheckResult {
  public bool? IsSet { get; set; }
  public bool? IsFinished { get; set; }
  public Game NewState { get; set; }
}

public class Game {
  public long Id { get; set; }
  public long UserId { get; set; }
  public DateTime StartedAt { get; set; }
  public DateTime? FinishedAt { get; set; }
  public int Fails { get; set; }
  public int Hints { get; set; }
  public ushort[]? Hand { get; set; }
  public ushort[]? Deck { get; set; }
  public ushort[] Found { get; set; } = Array.Empty<ushort>();

  public void ShuffleDeck() {
    if (Deck == null) return;

    Random rand = new();
    Deck = [.. Deck.OrderBy(_ => rand.Next())];
  }

  public void DealHand(int max = 12) {
    if (Deck == null) return;
    Hand ??= [];

    List<ushort> handList = [.. Hand];

    while (handList.Count < max && Deck.Length > 0) {
      handList.Add(Deck[0]);
      Deck = [.. Deck.Skip(1)];
    }

    Hand = [.. handList];
  }

  public void ReplaceCardInHand(int index) {
    if (Hand == null || Deck == null) return;

    var handList = Hand.ToList();

    if (Deck.Length < 1) {
      handList[index] = 0;
      Hand = [.. handList];
      return;
    }

    handList[index] = Deck[0];
    Deck = [.. Deck.Skip(1)];
    Hand = [.. handList];
  }

  public bool GameIsFinished() {
    if (Deck == null || Hand == null) return false;

    var finished = Deck.Length < 1 && Hand.Length < 12;

    // Check if no sets can be made with the remaining cards
    if (!finished && GetIndicesOfSet().Count == 0) {
      finished = true;
    }

    // Empty hand if game is finished for optimal database space usage
    if (finished) {
      Hand = [];
    }

    return finished;
  }

  public bool SetResult(ushort[] indices) {
    if (Hand == null || indices.Length != 3 || indices.Any(index => Hand[index] == 0)) {
      return false;
    }
    

    var cards = indices
      .Select(index => Card.ToCard(Hand[index]))
      .ToList();

    return (
      AllSameOrDifferent(cards[0].Shape, cards[1].Shape, cards[2].Shape) &&
      AllSameOrDifferent(cards[0].Color, cards[1].Color, cards[2].Color) &&
      AllSameOrDifferent(cards[0].Shade, cards[1].Shade, cards[2].Shade) &&
      AllSameOrDifferent(cards[0].Count, cards[1].Count, cards[2].Count)
    );
  }

  public SetCheckResult? IsSet(ushort[] indices) {
    // Also check if finished before anything else, to prevent the board getting stuck 
    if (GameIsFinished()) {
      FinishedAt = DateTime.UtcNow;
    
      return new SetCheckResult { IsFinished = true, NewState = this };
    }

    if (indices.Length != 3 || Hand == null || indices.Any(index => Hand[index] == 0)) {
      return null;
    }

    if (!SetResult(indices)) {
      Fails += 1;
      return new SetCheckResult { IsSet = false, NewState = this };
    }

    // Order by Descending to make sure the correct card is removed
    // (From the top may cause the indices to be off by one)
    foreach (var index in indices.OrderByDescending(i => i)) {
      var foundList = Found.ToList();
      foundList.Add(Hand[index]);
      Found = [.. foundList];

      if (Hand.Length < 13) {
          ReplaceCardInHand(index);
      } else {
          var handList = Hand.ToList();
          handList.RemoveAt(index);
          Hand = [.. handList];
      }
    }

    while (GetIndicesOfSet().Count < 1 && Deck?.Length > 0) {
      DealHand(Hand.Length + 1);
    }

    // Check if finished after removing the set
    if (GameIsFinished()) {
      FinishedAt = DateTime.UtcNow;
    
      return new SetCheckResult { IsFinished = true, NewState = this };
    }

    return new SetCheckResult { IsSet = true, NewState = this };
  }

  public List<int[]> GetIndicesOfSet() {
    if (Hand == null) return [];
    List<int[]> res = [];

    for (int i = 0; i < Hand.Length; i++) {
      for (int j = i + 1; j < Hand.Length; j++) {
        for (int k = j + 1; k < Hand.Length; k++) {
          var result = SetResult([(ushort)i, (ushort)j, (ushort)k]);

          if (result == true) {
            res.Add([i, j, k]);
          }
        }
      }
    }

    return res;
  }

  private static bool AllSameOrDifferent<T>(T a, T b, T c) where T : Enum {
    int ai = Convert.ToInt32(a);
    int bi = Convert.ToInt32(b);
    int ci = Convert.ToInt32(c);

    return (ai == bi && bi == ci) || (ai != bi && bi != ci && ai != ci);
  }
}
