import { Injectable } from '@angular/core';
import { CanActivate, Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Observable, map, take } from 'rxjs';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> | Promise<boolean> | boolean {
    return this.authService.isAuthenticated().pipe(
      take(1),
      map(isAuthenticated => {
        if (!isAuthenticated) {
          // Store the attempted URL for redirecting after login
          this.authService.redirectUrl = state.url;
          
          // Navigate to the login page
          this.router.navigate(['/login']);
          return false;
        }
        return true;
      })
    );
  }
}