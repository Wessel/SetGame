import { Injectable } from '@angular/core';
import { Card, toCard } from '../../app/models/card';
import axios from 'axios';

@Injectable({ providedIn: 'root' })

export class GameService {
  private deck: Card[] = [];
  public hand: Card[] = [];

  private gameId: number = 0;
  public fails: number = 0;
  public startDate: Date = new Date();
  public selectedCards: Card[] = [];

  constructor() {
    this.initializeDeck();
  }

  public stats(): any {
    const formattedDate = `${this.startDate.getDate().toString().padStart(2, '0')}-${(this.startDate.getMonth() + 1).toString().padStart(2, '0')}-${this.startDate.getFullYear()} ${this.startDate.getHours().toString().padStart(2, '0')}:${this.startDate.getMinutes().toString().padStart(2, '0')}`;
    return { size: this.deck.length, fails: this.fails, dateStarted: formattedDate };
  }

  private initializeDeck(): void {
    axios.post('http://localhost:5224/api/v1/Games', {}).then((response) => {
      console.log(response.data);
      response.data.deck.forEach((card: number) => {
        this.deck.push(toCard(card));
    });

      response.data.hand.forEach((card: number) => {
        this.hand.push(toCard(card));
      });

      this.startDate = new Date(response.data.startedAt);
      this.fails = response.data.fails;
      this.gameId = response.data.id;
    });
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
      console.log(this.hand.indexOf(card1), this.hand.indexOf(card2), this.hand.indexOf(card3));
      if (this.isSet([this.hand.indexOf(card1), this.hand.indexOf(card2), this.hand.indexOf(card3)])) {
        this.replaceSet();
      }
    }
  }


  private isSet(cards: number[]): boolean {
    // return (['shape', 'color', 'count', 'shade'] as (keyof Card)[]).every(attr => {
    //   const values = new Set([card1[attr], card2[attr], card3[attr]]);
    //   return values.size === 1 || values.size === 3;
    // });
      this.selectedCards = [];
    axios.post('http://localhost:5224/api/v1/Games/CheckSet/' + this.gameId, cards).then((response) => {
      console.log(response.data);
      this.hand = response.data.newState.hand.map((card: number) => toCard(card));
      this.deck = response.data.newState.deck.map((card: number) => toCard(card));
      this.fails = response.data.newState.fails;
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
}
