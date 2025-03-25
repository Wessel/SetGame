import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Card, toCard } from '../../app/models/card';
import { lastValueFrom } from 'rxjs';
import { AuthService } from '../auth/auth.service';

@Injectable({ providedIn: 'root' })
export class GameService {
  private API_URL = 'http://localhost:5224/api/v1/Games';
  
  public deck: Card[] = [];
  public hand: Card[] = [];
  public foundSets: Card[][] = [];
  public possibleSets: Card[][] = [];

  public fails: number = 0;
  public hints: number = 0;
  public gameId: number = 0;
  public startDate: Date = new Date();
  public finishedAt?: Date;
  public selectedCards: Card[] = [];

  constructor(
    private http: HttpClient,
  ) {}

  public initGame(gameId?: string): Promise<string> {
    this.deck = [];
    this.hand = [];
    return gameId ? this.initializeExistingGame(gameId) : this.initializeDeck();
  }

  public stats(): any {
    const formattedDate = `${this.startDate.getDate().toString().padStart(2, '0')}-${(this.startDate.getMonth() + 1).toString().padStart(2, '0')}-${this.startDate.getFullYear()} ${this.startDate.getHours().toString().padStart(2, '0')}:${this.startDate.getMinutes().toString().padStart(2, '0')}`;
    return { size: this.deck.length, fails: this.fails, hints: this.hints, dateStarted: formattedDate };
  }

  public async initializeExistingGame(gameId: string): Promise<string> {
    try {
      const response = await lastValueFrom(this.http.get<any>(`${this.API_URL}/${gameId}`));
      
      response.deck.forEach((card: number) => {
        this.deck.push(toCard(card));
      });
      
      response.hand.forEach((card: number) => {
        this.hand.push(toCard(card));
      });
      
      this.startDate = new Date(response.startedAt);
      this.fails = response.fails;
      this.hints = response.hints;
      this.finishedAt = response.finishedAt ? new Date(response.finishedAt) : undefined;
      
      this.foundSets = [];
      for (let i = 0; i < response.found.length; i += 3) {
        this.foundSets.push(response.found.slice(i, i + 3).map((card: number) => toCard(card)));
      }
      
      this.gameId = response.id;
      this.updateSets();
      return response.id.toString();
    } catch (error) {
      console.error('Error initializing existing game', error);
      throw error;
    }
  }

  private async initializeDeck(): Promise<string> {
    try {
      const response = await lastValueFrom(this.http.post<any>(this.API_URL, {}));
      
      response.deck.forEach((card: number) => {
        this.deck.push(toCard(card));
      });
      
      response.hand.forEach((card: number) => {
        this.hand.push(toCard(card));
      });
      
      this.startDate = new Date(response.startedAt);
      this.fails = response.fails;
      this.hints = response.hints;
      this.gameId = response.id;
      this.finishedAt = response.finishedAt ? new Date(response.finishedAt) : undefined;
      
      this.foundSets = [];
      for (let i = 0; i < response.found.length; i += 3) {
        this.foundSets.push(response.found.slice(i, i + 3).map((card: number) => toCard(card)));
      }
      
      this.updateSets();
      return response.id.toString();
    } catch (error) {
      console.error('Error initializing deck', error);
      throw error;
    }
  }

  public async updateSets(): Promise<Card[][]> {
    try {
      const response = await lastValueFrom(
        this.http.get<number[][]>(`${this.API_URL}/SetsInHand/${this.gameId}`)
      );
      
      const cards = response.map((set: number[]) => 
        set.map((card: number) => this.hand[card])
      );
      
      this.possibleSets = cards;
      return cards;
    } catch (error) {
      console.error('Error updating sets', error);
      throw error;
    }
  }

  public async checkSet(cards: number[]): Promise<boolean> {
    try {
      const response = await lastValueFrom(
        this.http.post<any>(`${this.API_URL}/CheckSet/${this.gameId}`, cards)
      );
      
      this.hand = response.newState.hand.map((card: number) => toCard(card));
      this.deck = response.newState.deck.map((card: number) => toCard(card));
      this.fails = response.newState.fails;
      this.hints = response.newState.hints;
      this.finishedAt = response.newState.finishedAt ? new Date(response.newState.finishedAt) : undefined;
      
      this.foundSets = [];
      for (let i = 0; i < response.newState.found.length; i += 3) {
        this.foundSets.push(response.newState.found.slice(i, i + 3).map((card: number) => toCard(card)));
      }
      
      await this.updateSets();
      return response.isSet;
    } catch (error) {
      console.error('Error checking set', error);
      throw error;
    }
  }

  public selectCard(card: Card): void {
    const cardIndex = this.hand.indexOf(card);
    if (cardIndex === -1) return; // Card not found in hand

    if (this.selectedCards.includes(card)) {
      this.selectedCards = this.selectedCards.filter(c => c !== card);
    } else if (this.selectedCards.length < 3) {
      this.selectedCards.push(card);
    }

    if (this.selectedCards.length === 3) {
      const indices = this.selectedCards.map(c => this.hand.indexOf(c));
      this.checkSet(indices).then(isSet => {
        this.selectedCards = [];
      });
    }
  }

  public async deleteGame(): Promise<void> {
    try {
      await lastValueFrom(this.http.delete<void>(`${this.API_URL}/${this.gameId}`));
    } catch (error) {
      console.error('Error deleting game', error);
      throw error;
    }
  }

  public async showHint(): Promise<void> {
    try {
      await lastValueFrom(this.http.post<void>(`${this.API_URL}/Hint/${this.gameId}`, {}));
      this.hints++;
    } catch (error) {
      console.error('Error showing hint', error);
      throw error;
    }
  }
}