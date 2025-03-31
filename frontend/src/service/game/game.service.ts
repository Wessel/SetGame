import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { lastValueFrom } from 'rxjs';

import { Card, toCard } from '../../app/models/card';
import { environment } from '../../environments/environment';

import { GameResponse, SetCheckResponse, HintResponse } from './game.types';

@Injectable({ providedIn: 'root' })
export class GameService {
  private http = inject(HttpClient);

  public  deck:          Card[]   = [];
  public  hand:          Card[]   = [];
  public  hint:          Card[]   = [];
  private selectedCards: Card[]   = [];
  public  foundSets:     Card[][] = [];
  public  possibleSets:  Card[][] = [];

  public fails:  number = 0;
  public hints:  number = 0;
  public gameId: number = 0;
  
  public startDate:   Date = new Date();
  public finishedAt?: Date;

  private updateState(game: GameResponse): void {
    // Clear hint if set is found
    if ((this.deck.length + this.hand.length) !== (game.deck.length + game.hand.length)) {
      this.hint.length = 0;
    }
    
    this.gameId = game.id;
    this.fails = game.fails;
    this.hints = game.hints;
    this.startDate = game.startedAt;
    this.finishedAt = game.finishedAt;

    this.deck = game.deck.map(toCard);
    this.hand = game.hand.map(toCard);
    this.foundSets = [];

    // Is stored as [card1, card2, card3, ...], chunk into sets of 3
    game.found.reduce((acc: Card[][], card: number, index: number) => {
      if (index % 3 === 0) acc.push([]);
      acc[acc.length - 1].push(toCard(card));
      return acc;
    }, this.foundSets);

    // Shows all sets in hand for admins
    this.getSetsInHand();
  }

  public async initGame(gameId?: number): Promise<number> {
    // Fetch game data from the server, use lastValueFrom to convert Observable to Promise
    let game: GameResponse;
    if (gameId) {
      game = await lastValueFrom(this.http.get<GameResponse>(`${environment.apiUrl}/${gameId}`))
    } else {
      game = await lastValueFrom(this.http.post<GameResponse>(environment.apiUrl, {}));
    }

    // Update the game state with the fetched data
    this.updateState(game);

    return game.id;
  }

  private async getSetsInHand(): Promise<void> {
    const sets = await lastValueFrom(
      this.http.get<number[][]>(`${environment.apiUrl}/SetsInHand/${this.gameId}`)
    );
    
    this.possibleSets = sets.map(set => set.map(cardIndex => this.hand[cardIndex]));
  }

  private async checkSet(cards: number[]): Promise<boolean> {
    const response = await lastValueFrom(
      this.http.post<SetCheckResponse>(`${environment.apiUrl}/CheckSet/${this.gameId}`, cards)
    );

    this.updateState(response.newState);

    return response.isSet;
  }

  public selectCard(card: Card): void {
    // Card not found in hand
    if (this.hand.indexOf(card) === -1) return;

    // Unselect card if already selected
    if (this.selectedCards.includes(card)) {
      this.selectedCards.splice(this.selectedCards.indexOf(card), 1);
    // Select card if less than 3 selected
    } else if (this.selectedCards.length < 3) {
      this.selectedCards.push(card);
    }

    // Check if 3 cards are selected. If so, check if they form a set
    if (this.selectedCards.length === 3) {
      this.checkSet(this.selectedCards.map(c => this.hand.indexOf(c)));
      this.selectedCards.length = 0;
    }
  }

  public async showHint(): Promise<void> {
    const hint = await lastValueFrom(this.http.get<HintResponse>(`${environment.apiUrl}/Hint/${this.gameId}`));
    this.hints++;

    this.hint = hint.map((indexOfCard: number) => this.hand[indexOfCard]);
  }

  public deleteGame(): void {
    this.http.delete<void>(`${environment.apiUrl}/${this.gameId}`).subscribe();
  }
}