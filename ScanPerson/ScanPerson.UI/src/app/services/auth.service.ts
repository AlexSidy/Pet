import { Injectable } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { ScanPersonResultResponse } from '../models/responses/scan.person.result.response';
import { ACCES_TOKEN_KEY, AuthApi } from '../models/constants/constants';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { LoginRequest } from '../models/requests/login.request';
import { JwtHelperService } from '@auth0/angular-jwt';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private api: string = AuthApi + '/Auth';

  constructor(private httpClient: HttpClient, private router: Router, private jwtHelper: JwtHelperService) {}

  login(email: string, password: string): Observable<ScanPersonResultResponse> {
    var request = new LoginRequest(password, email);
    return this.httpClient
      .post<ScanPersonResultResponse>(this.api + '/login', request)
      .pipe(
        tap(response => {
          sessionStorage.setItem(ACCES_TOKEN_KEY, response.Result);
        })
      )
  }

  isAuthenticated(): boolean {
    var token = sessionStorage.getItem(ACCES_TOKEN_KEY);
    return token != null && !this.jwtHelper.isTokenExpired();
  }

  logout(): void {
    sessionStorage.removeItem(ACCES_TOKEN_KEY);
    this.router.navigate(['']);
  }
}
