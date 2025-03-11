import { Injectable } from '@angular/core';
import { Card, toCard } from '../../app/models/card';
import axios from 'axios';

@Injectable({ providedIn: 'root' })

export class GameService {
  public deck: Card[] = [];
  public hand: Card[] = [];
  public possibleSets: Card[][] = [];

  public gameId: number = 0;
  public fails: number = 0;
  public startDate: Date = new Date();
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
    return { size: this.deck.length, fails: this.fails, dateStarted: formattedDate };
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
      this.gameId = res.data.id;

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
    const req = await axios.get('http://localhost:5224/api/v1/Games/SetsInHand/' + this.gameId);
    const cards = req.data.map((set: number[]) => set.map((card: number) => this.hand[card]));

    this.possibleSets = cards;

    return cards;
  }


  private isSet(cards: number[]): boolean {
    console.log(this.possibleSets);
    // return (['shape', 'color', 'count', 'shade'] as (keyof Card)[]).every(attr => {
    //   const values = new Set([card1[attr], card2[attr], card3[attr]]);
    //   return values.size === 1 || values.size === 3;
    // });
      this.selectedCards = [];
    axios.post('http://localhost:5224/api/v1/Games/CheckSet/' + this.gameId, cards).then((response) => {
      this.hand = response.data.newState.hand.map((card: number) => toCard(card));
      this.deck = response.data.newState.deck.map((card: number) => toCard(card));
      this.fails = response.data.newState.fails;
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
