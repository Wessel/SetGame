import { Component, inject } from '@angular/core';
import { GameService } from '../../service/game/game.service';
import { Card } from '../models/card';
import { CommonModule } from '@angular/common';
import { CardComponent } from '../card/card.component';
import { ActivatedRoute, UrlSegment, Router } from '@angular/router';

@Component({
  selector: 'app-game',
  templateUrl: './game.component.html',
  imports: [CardComponent, CommonModule],
  styleUrls: ['./game.component.scss']
})

export class GameComponent {
  gameId: number = -1;
  hint: Card[] = []
  gameService: GameService = inject(GameService);
  route: ActivatedRoute = inject(ActivatedRoute);
  router: Router = inject(Router);

  constructor() {
    this.route.params.subscribe(params => {
      this.gameId = parseInt(params['id']);

      this.attachId(this.gameId);
    });
  }

  returnHome() {
    this.router.navigate(['/']);
  }

  async attachId(i: number) {
    const id = await this.gameService.initGame(i);

    this.route.snapshot.url.push(new UrlSegment(id.toString(), {}));
    window.history.replaceState({}, '', `/game/${id}`);
  }

  deleteGame() {
    this.gameService.deleteGame();
    this.returnHome();
  }

  async selectCard(card: Card) {
    this.gameService.selectCard(card);
    card.selected = !card.selected;
  }

  async showHint() {
    // if (this.hint.length > 0) return;

    this.hint = await this.gameService.showHint();
  }
}
