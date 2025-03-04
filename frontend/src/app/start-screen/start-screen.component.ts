import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-start-screen',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './start-screen.component.html',
  styleUrls: ['./start-screen.component.scss']
})
export class StartScreenComponent {
  
  constructor(private router: Router) {}
  
  startNewGame() {
    this.router.navigate(['/game']);
  }
  
  viewGames() {
    this.router.navigate(['/game-list']);
  }
}