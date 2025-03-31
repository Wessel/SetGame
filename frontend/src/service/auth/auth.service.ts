import { Injectable, inject } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';

import { environment } from '../../environments/environment';

import { Credentials, LoginResponse } from './auth.types';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private http: HttpClient = inject(HttpClient);
  private router: Router = inject(Router);

  private isAuthenticatedSubject = new BehaviorSubject<boolean>(this.hasToken());

  login(credentials: Credentials): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${environment.authUrl}/login`, credentials)
      .pipe(
        tap(response => {
          localStorage.setItem(environment.authTokenKey, response.token);
          this.isAuthenticatedSubject.next(true);
        })
      );
  }

  logout(): void {
    localStorage.removeItem(environment.authTokenKey);
    this.isAuthenticatedSubject.next(false);
    this.router.navigate([ '/login' ]);
  }

  isAuthenticated(): Observable<boolean> {
    return this.isAuthenticatedSubject.asObservable();
  }

  getToken(): string | null {
    return localStorage.getItem(environment.authTokenKey);
  }

  private hasToken(): boolean {
    return !!localStorage.getItem(environment.authTokenKey);
  }
}