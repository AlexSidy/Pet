// main.ts
import { enableProdMode } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { environment } from './enviroments/enviroments';
import { AppComponent } from './app/app.component';
import { JwtModule, JWT_OPTIONS } from '@auth0/angular-jwt';
import { bootstrapApplication } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { ACCES_TOKEN_KEY } from './app/models/constants/constants';

export function tokenGetter() {
  return sessionStorage.getItem(ACCES_TOKEN_KEY);
}

if (environment.aspNetCoreEnviroment == 'Production') {
  enableProdMode();
}

// Конфигурация для JwtModule
const jwtOptions = {
  tokenGetter: tokenGetter,
  allowedDomains: environment.tokenWhiteListDomains,
};

bootstrapApplication(AppComponent, {
  providers: [
    {
      provide: JWT_OPTIONS,
      useValue: jwtOptions,
    },
    JwtModule, // Добавляем сам JwtModule
    HttpClientModule // Добавляем HttpClientModule
  ]
}).catch(err => console.error(err));