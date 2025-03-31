// import { Component } from '@angular/core';

// @Component({
//   selector: 'app-login-screen',
//   imports: [],
//   templateUrl: './login-screen.component.html',
//   styleUrl: './login-screen.component.scss'
// })
// export class LoginScreenComponent {

// }

import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../service/auth/auth.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-login-screen',
  templateUrl: './login-screen.component.html',
  styleUrls: ['./login-screen.component.scss'],
  standalone: true,
  imports: [FormsModule, CommonModule]
})
export class LoginScreenComponent {
  username: string = '';
  password: string = '';
  error: string = '';

  constructor(private authService: AuthService, private router: Router) {}

  login() {
    this.error = '';
    
    if (!this.username || !this.password) {
      this.error = 'Username and password are required';
      return;
    }

    this.authService.login({ username: this.username, password: this.password })
      .subscribe({
        next: () => {
          this.router.navigate(['/']);
        },
        error: (err) => {
          this.error = 'Invalid username or password';
          console.error('Login error:', err);
        }
      });
  }
}