import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { lastValueFrom } from 'rxjs';
import { AuthService } from '../auth/auth.service';

@Injectable({
  providedIn: 'root'
})
export class UserDataService {
  private GAMES_API_URL = 'http://localhost:5224/api/v1/Games';

  constructor(
    private http: HttpClient,
    private authService: AuthService
  ) {}

  public async login(credentials: { username: string, password: string }): Promise<void> {
    try {
      await lastValueFrom(this.authService.login(credentials.username, credentials.password));
    } catch (error) {
      console.error('Login error:', error);
      throw error;
    }
  }

  public async getGames(): Promise<any[]> {
    try {
      const data = await lastValueFrom(this.http.get<any[]>(this.GAMES_API_URL));
      
      const sortedGames = data
        .sort((a: any, b: any) => new Date(a.startedAt).getTime() - new Date(b.startedAt).getTime())
        .reverse();
      
      return sortedGames;
    } catch (error) {
      console.error('Error getting games:', error);
      throw error;
    }
  }
}