
// export interface Card {
//   shape: 'oval' | 'squiggle' | 'diamond';
//   color: 'red' | 'green' | 'purple';
//   count: 1 | 2 | 3;
//   shading: 'solid' | 'striped' | 'open';
//   isSelected?: boolean;
// }

  export enum CardCount {
    One = 0x0003,
    Two = 0x0007,
    Three = 0x0008
  }

  export enum CardColor {
    Red = 0x0030,
    Green = 0x0070,
    Blue = 0x0080
  }

  export enum CardShape {
    Oval = 0x0300,
    Diamond = 0x0700,
    Squiggle = 0x0800
  }

  export enum CardShade {
    Transparent = 0x3000,
    Solid = 0x7000,
    Opaque = 0x8000
  }

  export interface Card {
    count: CardCount;
    color: CardColor;
    shape: CardShape;
    shade: CardShade;
    selected?: boolean;
  }

  export function toUshort(card: Card): number {
    return 0x0000 | card.count | card.color | card.shape | card.shade;
  }

  export function toCard(card: number): Card {
    return {
      count: card & 0x000F as CardCount,
      color: card & 0x00F0 as CardColor,
      shape: card & 0x0F00 as CardShape,
      shade: card & 0xF000 as CardShade
    };
  }