import { Routes } from '@angular/router';
import { StartScreenComponent } from './start-screen/start-screen.component';
import { GameComponent } from './game/game.component';
import { GameListComponent } from './game-list/game-list.component';

export const routes: Routes = [
  { path: '', component: StartScreenComponent },
  { path: 'game', component: GameComponent },
  { path: 'game/:id', component: GameComponent },
  { path: 'game-list', component: GameListComponent },
  { path: '**', redirectTo: '' }
];