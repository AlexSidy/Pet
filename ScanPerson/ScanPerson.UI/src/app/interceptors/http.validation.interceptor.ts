import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable()
export class ValidationInterceptor implements HttpInterceptor {
    constructor() { }
    private unknownError: string = 'An unknown error occurred.';
    private validationErros: string = 'validation errors';

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return next.handle(request).pipe(catchError(err => {
            if (err.status === 400 && err.error.title.includes(this.validationErros)) {
                return throwError(() => new Error(this.aggregateValidationErrors(err.error?.errors)));
            }

            return next.handle(request);
        }))
    }

    aggregateValidationErrors(validationErrors: { [key: string]: string[]}) : string {
        if (!validationErrors || typeof validationErrors !== 'object') {
            return this.unknownError;
        }

        let aggregatedMessage: string = this.validationErros + ' : ';
        const fieldNames = Object.keys(validationErrors);
        fieldNames.forEach(fieldName => {
            aggregatedMessage += '\n' + fieldName + '\n' + validationErrors[fieldName].join(',');
        });

        return aggregatedMessage;
    }
}