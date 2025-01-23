import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { Observable } from 'rxjs';

import { Person } from './models/person';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { environment } from '../enviroments/enviroments';
import { ACCES_TOKEN_KEY } from './models/constants/constants';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, FormsModule, HttpClientModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.sass'
})
export class AppComponent {
  title = 'ScanPerson.UI1';
  url = "/webApi/Person";
  items: Person[] = [ new Person(1, "Test3") ];

  constructor(private httpClient: HttpClient) {
    this.httpClient = httpClient;
  }

  load() {
    this.getItems().subscribe({
      next: (response) => {
        debugger;
        this.items = response;
      },
      error: (e) => {
        debugger;
        console.log(e);
      },
      complete: () => {
      }
    });
  }

  postLoad() {
    this.postItems().subscribe({
      next: (response) => {
        debugger;
        this.items = response;
      },
      error: (e) => {
        debugger;
        console.log(e);
      },
      complete: () => {
      }
    });
  }

  postItems(): Observable<Person[]> {
    return this.httpClient.post(this.url, {Id: 100}) as  Observable<Person[]>;
  }

  getItems(): Observable<Person[]> {
      return this.httpClient.get(this.url) as  Observable<Person[]>;
  }
}
