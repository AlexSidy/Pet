import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ajax } from 'rxjs/ajax';

import { Item } from './item';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, FormsModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.sass'
})
export class AppComponent implements OnInit {
  title = 'ScanPerson.UI111';
  url = "http://localhost:8000/api/Test";
  items: Item[] = [];

  constructor() {
  }

  ngOnInit() {
    this.getItems().subscribe({
      next: (res) => {
        this.items = res.response as Item[];
      },
      error: (e) => {
        console.log(e);
      },
      complete: () => {
      }
    });
  }

  getItems() {
      return ajax(this.url);
  }
}
