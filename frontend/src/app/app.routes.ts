// import { Routes } from '@angular/router';
// import { StartScreenComponent } from './start-screen/start-screen.component';
// import { GameComponent } from './game/game.component';
// import { GameListComponent } from './game-list/game-list.component';

// export const routes: Routes = [
//   { path: '', component: StartScreenComponent },
//   { path: 'game', component: GameComponent },
//   { path: 'game/:id', component: GameComponent },
//   { path: 'game-list', component: GameListComponent },
//   { path: '**', redirectTo: '' }
// ];
import { Routes } from '@angular/router';
import { StartScreenComponent } from './start-screen/start-screen.component';
import { GameComponent } from './game/game.component';
import { GameListComponent } from './game-list/game-list.component';
import { LoginScreenComponent } from './login-screen/login-screen.component';
import { AuthGuard } from '../service/auth/auth.guard';

export const routes: Routes = [
  { path: '', component: StartScreenComponent, canActivate: [AuthGuard] },
  { path: 'game', component: GameComponent, canActivate: [AuthGuard] },
  { path: 'game/:id', component: GameComponent, canActivate: [AuthGuard] },
  { path: 'game-list', component: GameListComponent, canActivate: [AuthGuard] },
  { path: 'login', component: LoginScreenComponent },
  { path: '**', redirectTo: '' }
];