import { Routes } from '@angular/router';
import { HomeComponent } from './components/home/home.component';
import { PersonComponent } from './components/person/person.component';
import { AuthComponent } from './components/auth/auth.component';

export const routes: Routes = [
    { path: '', component: HomeComponent },
    { path: 'person', component: PersonComponent },
    { path: 'auth', component: AuthComponent },
  ];
