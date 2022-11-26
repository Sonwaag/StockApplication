import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
    templateUrl: 'createcompany.component.html'
})
export class CreateCompanyComponent {
    constructor(private _http: HttpClient) { }


}