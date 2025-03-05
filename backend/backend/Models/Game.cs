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

    bool matchingShapes = cards.Select(card => card.Shape).Distinct().Count() == 1;
    bool matchingColors = cards.Select(card => card.Color).Distinct().Count() == 1;
    bool matchingShades = cards.Select(card => card.Shade).Distinct().Count() == 1;
    bool matchingCounts = cards.Select(card => card.Count).Distinct().Count() == 1;

    if (!matchingShapes && !matchingColors && !matchingShades && !matchingCounts) {
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
}
