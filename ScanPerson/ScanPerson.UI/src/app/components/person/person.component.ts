import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs';

import { LocationItem, PersonInfoItem } from '../../models/items/person.info.items';
import { PersonInfoRequest } from '../../models/requests/person.info.request';
import { WebApi } from '../../constants/constants';
import { ScanPersonResultResponse } from '../../models/responses/scan.person.result.response';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [ CommonModule, FormsModule ],
  templateUrl: './person.component.html',
  styleUrls: ['./person.component.sass']
})
export class PersonComponent {

  public readonly title = 'ScanPerson.UI';
  public items: PersonInfoItem[] = [ new PersonInfoItem(1, 'Test3', 'mail', new LocationItem()) ];
  public phoneNumber: string = '';

  private readonly url = '/' + WebApi + '/PersonInfo';

  constructor(private readonly httpClient: HttpClient) {
    this.httpClient = httpClient;
  }

  postLoad() {
    this.postItems().subscribe({
      next: (response : ScanPersonResultResponse): void => {
        if (response.isSuccess) {
          this.items = [response.result];
        }
      },
      error: (e): void => {
        alert('Failed to register. Please try again later:' + (e.error ?? e.message));
        console.log(e);
      },
      complete: (): void => {
      }
    });
  }

  postItems(): Observable<ScanPersonResultResponse> {
    return this.httpClient.post(`${this.url}/GetScanPersonInfoAsync`, new PersonInfoRequest(this.phoneNumber)) as  Observable<ScanPersonResultResponse>;
  }
}

