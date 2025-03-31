import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { lastValueFrom } from 'rxjs';

import { AuthService } from '../auth/auth.service';
import { environment } from '../../environments/environment';

import { Credentials } from '../auth/auth.types';
import { GameResponse } from '../game/game.types';

@Injectable({ providedIn: 'root' })
export class UserDataService {
  private http: HttpClient = inject(HttpClient);
  private authService: AuthService = inject(AuthService);

  public async login(credentials: Credentials): Promise<void> {
    await lastValueFrom(this.authService.login(credentials));
  }

  public async getGames(): Promise<GameResponse[]> {
    const games = await lastValueFrom(this.http.get<GameResponse[]>(environment.apiUrl));
    
    return games
      .sort((a: GameResponse, b: GameResponse) => 
        new Date(a.startedAt).getTime() - new Date(b.startedAt).getTime()
      ).reverse();
  }
}