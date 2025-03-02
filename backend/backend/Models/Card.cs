namespace backend.Models;

// 3, 7 and 8 are chosen due to them in no way, 
// shape or form adding up or substracting into eachother
public enum CardCount { One = 0x0003, Two = 0x0007, Three = 0x0008 }
public enum CardColor { Red = 0x0030, Green = 0x0070, Blue = 0x0080 }
public enum CardShape { Oval = 0x0300, Diamond = 0x0700, Squiggle = 0x0800 }
public enum CardShade { Transparent = 0x3000, Solid = 0x7000, Opaque = 0x8000 }

public struct Card {
  public CardCount Count { get; set; }
  public CardColor Color { get; set; }
  public CardShape Shape { get; set; }
  public CardShade Shade { get; set; }

  public readonly ushort ToUshort() {
    return (ushort)(0x0000 | Convert.ToUInt16(Count)
                           | Convert.ToUInt16(Color)
                           | Convert.ToUInt16(Shape)
                           | Convert.ToUInt16(Shade));
  }

  public static Card ToCard(int card) {
    return new Card {
      Count = (CardCount)(card & 0x000F),
      Color = (CardColor)(card & 0x00F0),
      Shape = (CardShape)(card & 0x0F00),
      Shade = (CardShade)(card & 0xF000)
    };
  }
}
