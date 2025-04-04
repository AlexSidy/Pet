import { ApplicationConfig, provideZoneChangeDetection, importProvidersFrom } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideClientHydration } from '@angular/platform-browser';
import { provideHttpClient, withFetch } from '@angular/common/http';
import { JwtModule } from '@auth0/angular-jwt';

import { routes } from './app.routes';
import { ACCESS_TOKEN_KEY } from './constants/constants';
import { environment } from '../enviroments/enviroments';

export function tokenGetter() {
  return sessionStorage.getItem(ACCESS_TOKEN_KEY);
}

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideClientHydration(),
    provideHttpClient(withFetch()),
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
