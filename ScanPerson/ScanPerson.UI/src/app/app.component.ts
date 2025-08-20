import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet,RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthService } from './services/auth.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,
    RouterOutlet,
    FormsModule,
    RouterModule],
  providers: [AuthService],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.sass']
})
export class AppComponent {

  public get isLoggedIn(): boolean {
    return this.authService.isAuthenticated();
  }

  constructor(private readonly authService: AuthService) {
  }

  public logout(): void {
    this.authService.logout();
  }

}
