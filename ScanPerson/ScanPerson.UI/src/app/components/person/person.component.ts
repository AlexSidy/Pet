import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs';

import { Person } from '../../models/items/person';
import { PersonRequest } from '../../models/requests/person.request';
import { WebApi } from '../../constants/constants';
import { ScanPersonResultResponse } from "../../models/responses/scan.person.result.response";

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
        if (response.isSuccess) {
          this.items = [...response.result];
        }
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
      next: (response : ScanPersonResultResponse): void => {
        if (response.isSuccess) {
          this.items = [response.result];
        }
      },
      error: (e): void => {
        alert('Failed to register. Please try again later:' + e.error);
        console.log(e);
      },
      complete: (): void => {
      }
    });
  }

  postItems(): Observable<ScanPersonResultResponse> {
    return this.httpClient.post(`${this.url}/GetPersonAsync`, new PersonRequest('login', 'password', 'email')) as  Observable<ScanPersonResultResponse>;
  }

  getItems(): Observable<ScanPersonResultResponse> {
    return this.httpClient.get<ScanPersonResultResponse>(`${this.url}/GetPersonsAsync`) as Observable<ScanPersonResultResponse>;
  }
}

