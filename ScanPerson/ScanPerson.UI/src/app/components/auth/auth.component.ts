import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-auth',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './auth.component.html',
  styleUrl: './auth.component.sass'
})
export class AuthComponent {

  public email: string = '';
  public password: string = '';

  constructor(private authService: AuthService) { }

  login() {
    this.authService.login(this.email, this.password);
  }

  register() {
    this.authService.register(this.email, this.password);
  }
}
