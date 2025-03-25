import { Injectable } from '@angular/core';
import axios, { AxiosError } from 'axios';

// axios.defaults.headers.common['Authorization'] = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJhZG1pbiIsImp0aSI6ImRmNThhYTU3LWZkNzItNGIzYS05OTNmLTY4NjAyNGMzYjdlNSIsImV4cCI6MTc0MjgxODEzOCwiaXNzIjoid2Vzc2VsLmdnIiwiYXVkIjoid2Vzc2VsLmdnIn0.hDf8qcxXeSFQhmgnMzBrH3ZJJMplwZ-1RQwNxeZo5ok';
axios.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

@Injectable({
  providedIn: 'root'
})
export class UserDataService {

  constructor() {
    // this.login({ username: 'admin', password: 'password' });
  }

  public async login(credentials: any) {
    const response = await axios.post('http://localhost:5224/api/Auth/login', credentials);
    localStorage.setItem('token', response.data.token);    
  }

  public async getGames(): Promise<any> {
    const req = await axios.get('http://localhost:5224/api/v1/Games', {
      headers: {
        'Authorization': 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJhZG1pbiIsImp0aSI6ImRmNThhYTU3LWZkNzItNGIzYS05OTNmLTY4NjAyNGMzYjdlNSIsImV4cCI6MTc0MjgxODEzOCwiaXNzIjoid2Vzc2VsLmdnIiwiYXVkIjoid2Vzc2VsLmdnIn0.hDf8qcxXeSFQhmgnMzBrH3ZJJMplwZ-1RQwNxeZo5ok'
      }
    });
    const sortedGames = req.data
      .sort((a: any, b: any) => new Date(a.startedAt).getTime() - new Date(b.startedAt).getTime())
      .reverse();
    return sortedGames;
  }
}
