import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../service/auth/auth.service';

@Component({
  selector: 'app-header',
  imports: [],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss'
})

export class HeaderComponent {
  constructor(private router: Router, private authService: AuthService) {}

  routeTo(target: string) {
    this.router.navigate([target]);
  }

  logout(): void {
    this.authService.logout();
    // The AuthService should handle navigation to the login page
  }
}
