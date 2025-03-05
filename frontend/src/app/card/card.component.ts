import { Component, Input, OnInit } from '@angular/core';
import { Card, CardColor, CardCount, CardShade, CardShape } from '../models/card';

@Component({
  selector: 'game-card',
  templateUrl: './card.component.html',
  styleUrls: ['./card.component.scss']
})

export class CardComponent implements OnInit {
  @Input() card!: Card;
  @Input() selected: boolean = false;
  @Input() small: boolean = false;

  shapeFile: string = '';
  shapeWidth: number = 80;

  ngOnInit() {
    this.shapeFile = `shapes/${this.parseShape()}-${this.parseColor()}-${this.parseShade()}.svg`;
  }

  parseShade() {
    switch (this.card.shade) {
      case CardShade.Opaque: return 'opaque';
      case CardShade.Solid: return 'solid';
      default: return 'transparent';
    }
  }

  parseColor() {
    switch (this.card.color) {
      case CardColor.Red: return 'red';
      case CardColor.Green: return 'green';
      default: return 'blue';
    }
  }

  parseCount() {
    switch (this.card.count) {
      case CardCount.One: return [1];
      case CardCount.Two: return [1, 2];
      default: return [1, 2, 3];
    }
  }

  parseShape() {
    switch (this.card.shape) {
      case CardShape.Diamond: return 'diamond';
      case CardShape.Squiggle: return 'squiggle';
      default: return 'oval';
    }
  }
}
