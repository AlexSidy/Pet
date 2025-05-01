import { ApplicationConfig, provideZoneChangeDetection, importProvidersFrom } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideClientHydration } from '@angular/platform-browser';
import { provideHttpClient, withFetch, withInterceptorsFromDi, HTTP_INTERCEPTORS } from '@angular/common/http';
import { JwtModule } from '@auth0/angular-jwt';

import { routes } from './app.routes';
import { ACCESS_TOKEN_KEY } from './constants/constants';
import { environment } from '../enviroments/enviroments';
import { HttpJwtInterceptor } from '../app/interceptors/http.jwt.interceptor';
import { HttpUnauthorizedInterceptor } from '../app/interceptors/http.unauthorized.interceptor';

export function tokenGetter() {
  return sessionStorage.getItem(ACCESS_TOKEN_KEY);
}

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideClientHydration(),
    provideHttpClient(withFetch(), withInterceptorsFromDi()),
    {
      provide: HTTP_INTERCEPTORS,
      useClass: HttpJwtInterceptor,
      multi: true // Указывает, что это может быть не единственный перехватчик
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: HttpUnauthorizedInterceptor,
      multi: true // Указывает, что это может быть не единственный перехватчик
    },
    importProvidersFrom(
      JwtModule.forRoot({
          config: {
              tokenGetter: tokenGetter,
              allowedDomains: environment.tokenWhiteListDomains,
              disallowedRoutes: [],
          },
      })),
    ],
};
