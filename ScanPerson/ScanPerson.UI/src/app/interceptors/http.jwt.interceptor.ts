import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable } from 'rxjs';

import { ACCESS_TOKEN_KEY, WebApi } from '../constants/constants';
import { AuthService } from '../services/auth.service';

@Injectable()
export class HttpJwtInterceptor implements HttpInterceptor {
    constructor(private readonly authService: AuthService) {}

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        // add auth header with jwt if user is logged in and request is to api url
        console.log('Intercepting request:', request);
        const token = sessionStorage.getItem(ACCESS_TOKEN_KEY);
        if (this.authService.isAuthenticated() && request.url.includes(WebApi, 0))
        {
            const token = sessionStorage.getItem(ACCESS_TOKEN_KEY);
            request = request.clone({
                setHeaders: {
                    Authorization: `Bearer ${token}`
                }
            });
        }

        return next.handle(request);
    }
}