import { Component } from '@angular/core';
import { GameService } from '../../service/game/game.service';
import { Card } from '../models/card';
import { CommonModule } from '@angular/common';
import { CardComponent } from '../card/card.component';

@Component({
  selector: 'app-game',
  templateUrl: './game.component.html',
  imports: [CardComponent, CommonModule],
  styleUrls: ['./game.component.scss']
})
export class GameComponent {
  constructor(public gameService: GameService) { }

  selectCard(card: Card) {
    this.gameService.selectCard(card);
    card.selected = !card.selected;
  }
}
