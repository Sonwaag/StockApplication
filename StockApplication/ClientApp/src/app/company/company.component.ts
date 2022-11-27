import { Component, OnInit } from '@angular/core';
import { clientCompany } from '../TsClasses';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';

@Component({
    templateUrl: 'company.component.html'
})

export class CompanyComponent {
    public company: clientCompany;

    ngOnInit() {
        this.getCurrentCompany();
    }



    constructor(private http: HttpClient, private router: Router) { }

    getCurrentCompany() {
        this.http.get<clientCompany>("api/Stock/company/current")
            .subscribe(company => {
                this.company = company;
            },
            error => console.log(error)
        );
    }

}