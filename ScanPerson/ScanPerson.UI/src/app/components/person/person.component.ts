import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs';

import { Person } from '../../models/items/person';
import { PersonRequest } from '../../models/requests/person.request';
import { WebApi } from '../../constants/constants';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [ CommonModule, FormsModule ],
  templateUrl: './person.component.html',
  styleUrls: ['./person.component.sass']
})
export class PersonComponent {
  title = 'ScanPerson.UI';
  url = "/" + WebApi + "/Person";
  items: Person[] = [ new Person(1, "Test3") ];

  constructor(private httpClient: HttpClient) {
    this.httpClient = httpClient;
  }

  getLoad() {
    this.getItems().subscribe({
      next: (response) => {
        this.items = response;
      },
      error: (e) => {
        console.log(e);
      },
      complete: () => {
      }
    });
  }

  postLoad() {
    this.postItems().subscribe({
      next: (response) => {
        this.items = [response];
      },
      error: (e) => {
        console.log(e);
      },
      complete: () => {
      }
    });
  }

  postItems(): Observable<Person> {
    return this.httpClient.post(`${this.url}/GetPerson`, new PersonRequest('login', 'password', 'email')) as  Observable<Person>;
  }

  getItems(): Observable<Person[]> {
    return this.httpClient.get<Person[]>(`${this.url}/GetPersons`) as Observable<Person[]>;
  }
}

