import { Injectable } from '@angular/core';
import { Card, toCard } from '../../app/models/card';
// todo: Rewrite to use angular http client instead of axios, supports always sending tokens
import axios from 'axios';

@Injectable({ providedIn: 'root' })

export class GameService {
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

  constructor() {
  }

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
    const req = await axios.get('http://localhost:5224/api/v1/Games/' + gameId);
    req.data.deck.forEach((card: number) => {
      this.deck.push(toCard(card));

    });
      req.data.hand.forEach((card: number) => {
        this.hand.push(toCard(card));
      });
      this.startDate = new Date(req.data.startedAt);
      this.fails = req.data.fails;
      this.hints = req.data.hints;
      this.finishedAt = req.data.finishedAt;
      this.foundSets = [];
      for (let i = 0; i < req.data.found.length; i += 3) {
        this.foundSets.push(req.data.found.slice(i, i + 3).map((card: number) => toCard(card)));
      }
      
      this.gameId = req.data.id;
      await this.updateSets();
    
      return req.data.id;

  }

  private async initializeDeck(): Promise<string> {
    const res = await axios.post('http://localhost:5224/api/v1/Games', {});
    res.data.deck.forEach((card: number) => {
        this.deck.push(toCard(card));
    });

    res.data.hand.forEach((card: number) => {
        this.hand.push(toCard(card));
      });

      this.startDate = new Date(res.data.startedAt);
      this.fails = res.data.fails;
      this.hints = res.data.hints;
      this.gameId = res.data.id;
      this.finishedAt = res.data.finishedAt;
      this.foundSets = [];
      for (let i = 0; i < res.data.newState.found.length; i += 3) {
        this.foundSets.push(res.data.newState.found.slice(i, i + 3).map((card: number) => toCard(card)));
      }

      await this.updateSets();

      return res.data.id;
  }

  public selectCard(card: Card): void {
    const cardIndex = this.hand.indexOf(card);
    if (cardIndex === -1) return; // Card not found on the board

    if (this.selectedCards.includes(card)) {
      this.selectedCards = this.selectedCards.filter(c => c !== card);
    } else if (this.selectedCards.length < 3) {
      this.selectedCards.push(card);
    }

    if (this.selectedCards.length === 3) {
      const [card1, card2, card3] = this.selectedCards as [Card, Card, Card]; // ✅ Explicitly cast to a tuple
      if (this.isSet([this.hand.indexOf(card1), this.hand.indexOf(card2), this.hand.indexOf(card3)])) {
        this.replaceSet();
      }
    }
  }

  public async updateSets(): Promise<Card[][]> {
    const req = await axios.get(`http://localhost:5224/api/v1/Games/SetsInHand/${this.gameId}`);
    const cards = req.data
      .map((set: number[]) => 
        set.map((card: number) => this.hand[card])
      );

    this.possibleSets = cards;

    return cards;
  }

  private isSet(cards: number[]): boolean {
    this.selectedCards = [];
    
    axios.post(`http://localhost:5224/api/v1/Games/CheckSet/${this.gameId}`, cards).then((response) => {
      this.hand = response.data.newState.hand.map((card: number) => toCard(card));
      this.deck = response.data.newState.deck.map((card: number) => toCard(card));
      this.fails = response.data.newState.fails;
      this.hints = response.data.newState.hints;
      this.finishedAt = response.data.newState.finishedAt;
      this.foundSets = [];
      for (let i = 0; i < response.data.newState.found.length; i += 3) {
        this.foundSets.push(response.data.newState.found.slice(i, i + 3).map((card: number) => toCard(card)));
      }

      this.updateSets();

      return response.data.isSet;
    });

    return false;
  }


  private replaceSet(): void {
    this.hand.find((c, i) => {
      if (this.selectedCards.includes(c)) {
        this.hand[i] = this.deck.splice(0, 1)[0];
      }
    });
    this.selectedCards = [];
  }

  public async deleteGame() {
    await axios.delete('http://localhost:5224/api/v1/Games/' + this.gameId);
  }
}
