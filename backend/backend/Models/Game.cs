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
  public ushort[]? Hand { get; set; }
  public ushort[]? Deck { get; set; }

  public void ShuffleDeck() {
    if (Deck == null) return;

    Random rand = new Random();
    Deck = Deck.OrderBy(_ => rand.Next()).ToArray();
  }

  public void DealHand() {
    if (Deck == null) return;
    Hand ??= [];

    List<ushort> handList = [.. Hand];

    while (handList.Count < 12 && Deck.Length > 0) {
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

    var finished = Deck.Length == 0 && Hand.All(card => card == 0);

    // Empty hand if game is finished for optimal database space usage
    if (finished) {
      Hand = [];
    }

    return finished;
  }

  public SetCheckResult? IsSet(ushort[] indices) {
    if (GameIsFinished()) {
      FinishedAt = DateTime.Now;

      return new SetCheckResult { IsFinished = true, NewState = this };
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
      Fails += 1;
      return new SetCheckResult { IsSet = false, NewState = this };
    }

    // Order by Descending to make sure the correct card is removed
    // (From the top may cause the indices to be off by one)
    foreach (var index in indices.OrderByDescending(i => i)) {
      ReplaceCardInHand(index);
    }

    return new SetCheckResult { IsSet = true, NewState = this };
  }

  private static bool AllSameOrDifferent<T>(T a, T b, T c) where T : Enum {
    int ai = Convert.ToInt32(a);
    int bi = Convert.ToInt32(b);
    int ci = Convert.ToInt32(c);

    return (ai == bi && bi == ci) || (ai != bi && bi != ci && ai != ci);
  }
}
