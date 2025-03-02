import { Component, Input } from '@angular/core';
import { Card, CardColor, CardCount, CardShade, CardShape } from '../models/card';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-card',
  imports: [CommonModule],
  templateUrl: './card.component.html',
  styleUrls: ['./card.component.scss']
})
export class CardComponent {
  @Input() card!: Card;

  parseShade() {
    switch (this.card.shade) {
      case CardShade.Opaque:
        return 'opaque';
      case CardShade.Solid:
        return 'translucent';
      case CardShade.Transparent:
        return 'transparent';
    }
  }

  parseColor() {
    switch (this.card.color) {
      case CardColor.Red:
        return 'red';
      case CardColor.Green:
        return 'green';
      case CardColor.Blue:
        return 'blue';
    }
  }

  parseCount() {
    switch (this.card.count) {
      case CardCount.One:
        return 1;
      case CardCount.Two:
        return 2;
      case CardCount.Three:
        return 3;
    }
  }

  parseShape() {
    switch (this.card.shape) {
      case CardShape.Diamond:
        return 'diamond';
      case CardShape.Squiggle:
        return 'squiggle';
      case CardShape.Oval:
        return 'oval';
    }
  }
}
