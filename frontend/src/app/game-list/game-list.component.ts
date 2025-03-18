import { Router } from '@angular/router';
import { Component, inject } from '@angular/core';
import { UserDataService } from '../../service/user/user-data.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-game-list',
  imports: [CommonModule],
  templateUrl: './game-list.component.html',
  styleUrl: './game-list.component.scss'
})
export class GameListComponent {
  games: { id: number, startedAt: Date, finishedAt: Date | null, fails: number, hand: any[], deck: any[] }[] | undefined;
  userService: UserDataService = inject(UserDataService);
  router: Router = inject(Router);
  
  constructor() { this.getGames() }

  async getGames() {
    this.games = await this.userService.getGames();
  }

  enterGame(game: number) {
    this.router.navigate(['/game', game]);
  }

  viewGames() {
    this.router.navigate(['/']);
  }
}
