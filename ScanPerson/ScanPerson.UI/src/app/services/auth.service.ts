import { Injectable } from '@angular/core';
import { JwtHelperService } from '@auth0/angular-jwt';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';

import { ScanPersonResultResponse } from '../models/responses/scan.person.result.response';
import { ACCESS_TOKEN_KEY, AuthApi } from '../constants/constants';
import { LoginRequest } from '../models/requests/login.request';
import { RegisterRequest } from '../models/requests/register.request';


@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private readonly api: string = "/" + AuthApi + '/Auth';

  constructor(
    private readonly httpClient: HttpClient,
    private readonly router: Router,
    private readonly jwtHelper: JwtHelperService) {}

  register(email: string, password: string) {
    let request = new RegisterRequest(password, email);
    return this.httpClient
      .post<ScanPersonResultResponse>(this.api + '/RegisterAsync', request)
      .subscribe({
        next: (response) => {
          if (response?.isSuccess) {
            alert('Register is success.');
          }
          else {
            alert(response?.error ?? 'An error occurred during registration');
          }
        },
        error: (e) => {
          console.log('Registration error:' + e.error);
          alert('Failed to register. Please try again later:' + e.error);
        },
        complete: () => {}
      });
  }  

  login(email: string, password: string) {
    let request = new LoginRequest(password, email);
    return this.httpClient
      .post<ScanPersonResultResponse>(this.api + '/LoginAsync', request)
      .subscribe({
        next: (response) => {
          if (response.isSuccess && this.isBrowser()) {
            sessionStorage.setItem(ACCESS_TOKEN_KEY, response.result);
            alert('Login is success.');
            this.router.navigate(['']);
          } 
          else {
            alert(response.error ?? 'An error occurred during login');
          }
        },
        error: (e) => {
          console.log('Registration error:' + e.error);
          alert('Failed to register. Please try again later:' + e.error);
        },
        complete: () => {}
      });
  }

  isAuthenticated(): boolean {
    if (this.isBrowser()) {
      const token = sessionStorage.getItem(ACCESS_TOKEN_KEY);
      return !!token && token != 'undefined' && !this.jwtHelper.isTokenExpired(token);
    }
    return false;
  }

  logout(): void {
    if (this.isBrowser()) {
      sessionStorage.removeItem(ACCESS_TOKEN_KEY);
    }
    this.router.navigate(['']);
  }

  private isBrowser(): boolean {
    return typeof window !== 'undefined' && typeof window.sessionStorage !== 'undefined';
  }
}
