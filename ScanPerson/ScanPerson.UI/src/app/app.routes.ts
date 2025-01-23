import { Routes } from '@angular/router';
import { AppComponent } from "./app.component";
import { HomeComponent } from './components/home/home.component';
import { AuthComponent } from './components/auth/auth.component';

export const routes: Routes = [
    { path: '', component: HomeComponent },
    { path: 'login', component: AuthComponent },

  ];
