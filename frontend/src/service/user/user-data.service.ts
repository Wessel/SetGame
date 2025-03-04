import { Injectable } from '@angular/core';
import axios from 'axios';

@Injectable({
  providedIn: 'root'
})
export class UserDataService {

  constructor() { }

  public async getGames(): Promise<any> {
    const req = await axios.get('http://localhost:5224/api/v1/Games');
    const sortedGames = req.data
      .sort((a: any, b: any) => new Date(a.startedAt).getTime() - new Date(b.startedAt).getTime())
      .reverse();
    return sortedGames;
  }
}
