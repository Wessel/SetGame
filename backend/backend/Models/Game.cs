namespace backend.Models;

public struct SetCheckResult {
  public bool? IsSet { get; set; }
  public bool? IsFinished { get; set; }
  public Game NewState { get; set; }
}

public class Game {
  public long Id { get; set; }
  public DateTime StartedAt { get; set; }
  public DateTime? FinishedAt { get; set; }
  public int Fails { get; set; }
  public int Hints { get; set; }
  public ushort[]? Hand { get; set; }
  public ushort[]? Deck { get; set; }
  public ushort[] Found { get; set; } = Array.Empty<ushort>();

  public void ShuffleDeck() {
    if (Deck == null) return;

    Random rand = new Random();
    Deck = Deck.OrderBy(_ => rand.Next()).ToArray();
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

    var finished = Deck.Length == 0 && Hand.Length < 12;

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

// todo: refactor`
  public SetCheckResult? IsSet(ushort[] indices, bool remove = true) {
    if (remove) {
      if (GameIsFinished()) {
        FinishedAt = DateTime.UtcNow;

        return new SetCheckResult { IsFinished = true, NewState = this };
      }
    }

    if (indices.Length != 3 || Hand == null) {
      return null;
    }

    if (indices.Any(index => Hand[index] == 0)) {
      return null;
    }

    var cards = indices
      .Select(index => Card.ToCard(Hand[index]))
      .ToList();

    if (!(
      AllSameOrDifferent(cards[0].Shape, cards[1].Shape, cards[2].Shape) &&
      AllSameOrDifferent(cards[0].Color, cards[1].Color, cards[2].Color) &&
      AllSameOrDifferent(cards[0].Shade, cards[1].Shade, cards[2].Shade) &&
      AllSameOrDifferent(cards[0].Count, cards[1].Count, cards[2].Count)
    )) {
      // todo(wessel): Split up for checking algorithm to game data
      Fails += 1;
      return new SetCheckResult { IsSet = false, NewState = this };
    }

    // Order by Descending to make sure the correct card is removed
    // (From the top may cause the indices to be off by one)
    if (remove) {
      foreach (var index in indices.OrderByDescending(i => i)) {
        var foundList = Found.ToList();
        foundList.Add(Hand[index]);
        Found = foundList.ToArray();

        if (Hand.Length < 13)
        {
            ReplaceCardInHand(index);
        }
        else
        {
            var handList = Hand.ToList();
            handList.RemoveAt(index);
            Hand = handList.ToArray();
        }
      }

      while (GetIndicesOfSet().Count < 1 && Deck?.Length > 0) {
        DealHand(Hand.Length + 1);
      }
    }

    return new SetCheckResult { IsSet = true, NewState = this };
  }

  private static bool AllSameOrDifferent<T>(T a, T b, T c) where T : Enum {
    int ai = Convert.ToInt32(a);
    int bi = Convert.ToInt32(b);
    int ci = Convert.ToInt32(c);

    return (ai == bi && bi == ci) || (ai != bi && bi != ci && ai != ci);
  }

  public List<int[]> GetIndicesOfSet() {
    if (Hand == null) return new List<int[]>();
    List<int[]> res = new List<int[]>();

    for (int i = 0; i < Hand.Length; i++) {
        for (int j = i + 1; j < Hand.Length; j++) {
            for (int k = j + 1; k < Hand.Length; k++) {
                var result = IsSet([(ushort)i, (ushort)j, (ushort)k], false);

                if (result?.IsSet == true) {
                    Console.WriteLine($"Found set at {i}, {j}, {k}");
                    res.Add([i, j, k]);
                }
            }
        }
    }

    return res;
  }
}
