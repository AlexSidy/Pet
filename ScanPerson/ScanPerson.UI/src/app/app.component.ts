import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { Observable } from 'rxjs';

import { Item } from './item';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, FormsModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.sass'
})
export class AppComponent {
  title = 'ScanPerson.UI1';
  url = "/api/Test";
  //url = "/test";
  //url = "https://scanperson.webapi/api/Test";
  //url = "https://localhost:8081/api/Test";
  items: Item[] = [ new Item(1, "Test3") ];
  httpClient: HttpClient;

  constructor(httpClient: HttpClient) {
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

  getItems(): Observable<Item[]> {
      return this.httpClient.get(this.url) as  Observable<Item[]>;
  }
}
