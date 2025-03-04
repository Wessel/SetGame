import { Component } from '@angular/core';
import { GameService } from '../../service/game/game.service';
import { Card } from '../models/card';
import { CommonModule } from '@angular/common';
import { CardComponent } from '../card/card.component';
import { ActivatedRoute, UrlSegment } from '@angular/router';

@Component({
  selector: 'app-game',
  templateUrl: './game.component.html',
  imports: [CardComponent, CommonModule],
  styleUrls: ['./game.component.scss']
})

export class GameComponent {
  gameId: string = '';

  constructor(public gameService: GameService, private route: ActivatedRoute) {
    this.route.params.subscribe(params => {
      this.gameId = params['id'];
      // You can use this.gameId to fetch game-specific data if needed
      this.attachId(this.gameId);
    });
        // this.route.snapshot.url.push({ path: this.gameId });
        //     this.route.snapshot.params['id'] = this.gameId;
        // // Update the URL without reloading the page

  }

  async attachId(i: string) {
    const id = await this.gameService.initGame(i);
    this.route.snapshot.url.push(new UrlSegment(id, {}));
    window.history.replaceState({}, '', `/game/${id}`);
  }

  selectCard(card: Card) {
    this.gameService.selectCard(card);
    card.selected = !card.selected;
  }
}
