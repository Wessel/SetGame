import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { Router } from '@angular/router';

interface LoginResponse {
  token: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private API_URL = 'http://localhost:5224/api/v1/Auth';
  private tokenKey = 'auth_token';
  private isAuthenticatedSubject = new BehaviorSubject<boolean>(this.hasToken());
  public redirectUrl: string | null = null;

  constructor(private http: HttpClient, private router: Router) {}

  login(username: string, password: string): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.API_URL}/login`, { username, password })
      .pipe(
        tap(response => {
          localStorage.setItem(this.tokenKey, response.token);
          this.isAuthenticatedSubject.next(true);
        })
      );
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
    this.isAuthenticatedSubject.next(false);
    this.router.navigate(['/login']);
  }

  isAuthenticated(): Observable<boolean> {
    return this.isAuthenticatedSubject.asObservable();
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  private hasToken(): boolean {
    return !!localStorage.getItem(this.tokenKey);
  }
}