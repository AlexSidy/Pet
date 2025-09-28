import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms'; 


import { Observable } from 'rxjs';

import { PersonInfoItem } from '../../models/items/person.info.items';
import { PersonInfoRequest } from '../../models/requests/person.info.request';
import { WebApi } from '../../constants/constants';
import { ScanPersonResultResponse } from '../../models/responses/scan.person.result.response';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [ CommonModule, ReactiveFormsModule ],
  templateUrl: './person.component.html',
  styleUrls: ['./person.component.sass']
})
export class PersonComponent {

  public readonly title = 'Добро пожаловать в мой PET-проект';
  public items: PersonInfoItem[] = [];
  public personForm: FormGroup;
  public isLoading: boolean = false; 

  get phoneNumberControl() {
    return this.personForm.get('phoneNumber');
  }

  private readonly url = '/' + WebApi + '/PersonInfo';

  constructor(
    private readonly httpClient: HttpClient,
    private readonly fb: FormBuilder
  ) { 
       this.personForm = this.fb.group({
      phoneNumber: ['', [
        Validators.required, 
        // должен начинаться с 9 и состоящий из 10 цифр.
        Validators.pattern('^9[0-9]{9}$') 
      ]]
    });
   }

  postLoad() {
    if (this.personForm.invalid) {
      this.personForm.markAllAsTouched();
      return;
    }

    this.isLoading = true;

    this.postItems().subscribe({
      next: (response : ScanPersonResultResponse<PersonInfoItem>): void => {
        if (response.isSuccess) {
          this.items = [response.result];
        }
        if (!response.isSuccess) {
          alert(response.error);
          return;
        }
        // only warning (another show box)
        if (response.error) {
          console.info(response.error);
        }
      },
      error: (e): void => {
        alert(e.error ?? e.message);
        console.log(e);
      },
      complete: (): void => {
        this.isLoading = false; 
      }
    });
  }

  postItems(): Observable<ScanPersonResultResponse<PersonInfoItem>> {
    return this.httpClient
    .post(`${this.url}/GetScanPersonInfoAsync`, new PersonInfoRequest(this.phoneNumberControl!.value)) as  Observable<ScanPersonResultResponse<PersonInfoItem>>;
  }
}

