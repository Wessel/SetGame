import { Component } from '@angular/core';
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
  gameId: string = '';
  hint: Card[] = []

  constructor(public gameService: GameService, private route: ActivatedRoute, private router: Router) {
    this.route.params.subscribe(params => {
      this.gameId = params['id'];

      this.attachId(this.gameId);
    });
  }

  returnHome() {
    this.router.navigate(['/']);
  }

  async attachId(i: string) {
    const id = await this.gameService.initGame(i);

    this.route.snapshot.url.push(new UrlSegment(id, {}));
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
    if (this.gameService.possibleSets.length < 0) return console.error('No valid sets found');

    this.hint = [this.gameService.possibleSets[0][0], this.gameService.possibleSets[0][1]]
  }
}
