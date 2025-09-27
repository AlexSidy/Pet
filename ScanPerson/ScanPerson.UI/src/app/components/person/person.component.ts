import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs';

import { LocationItem, PersonInfoItem } from '../../models/items/person.info.items';
import { PersonInfoRequest } from '../../models/requests/person.info.request';
import { WebApi } from '../../constants/constants';
import { ScanPersonResultResponse } from '../../models/responses/scan.person.result.response';
import { ScanPersonResponseBase } from '../../models/responses/scan.person.response.base';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [ CommonModule, FormsModule ],
  templateUrl: './person.component.html',
  styleUrls: ['./person.component.sass']
})
export class PersonComponent {

  public readonly title = 'ScanPerson.UI';
  public items: PersonInfoItem[] = [ new PersonInfoItem(1, ['Test3'], 'mail', new LocationItem()) ];
  public phoneNumber: string = '';

  private readonly url = '/' + WebApi + '/PersonInfo';

  constructor(private readonly httpClient: HttpClient) {
    this.httpClient = httpClient;
  }

  postLoad() {
    this.postItems().subscribe({
      next: (response : ScanPersonResultResponse<PersonInfoItem[]>): void => {
        if (response.isSuccess) {
          this.items = response.result;
        }
        if (!response.isSuccess) {
          alert(response.error);
          return;
        }
        // only warning (anotger show box)
        if (response.error) {
          console.info(response.error);
        }
      },
      error: (e): void => {
        alert('Failed to load. Please try again later:' + (e.error ?? e.message));
        console.log(e);
      },
      complete: (): void => {
      }
    });
  }

  postItems(): Observable<ScanPersonResultResponse<PersonInfoItem[]>> {
    return this.httpClient
    .post(`${this.url}/GetScanPersonInfoAsync`, new PersonInfoRequest(this.phoneNumber))as  Observable<ScanPersonResultResponse<PersonInfoItem[]>>;
  }
}

