import { Component, OnInit } from '@angular/core';
import { company } from '../TsClasses';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';

@Component({
    templateUrl: 'companies.component.html'
})
export class CompaniesComponent {
    public alleCompanies: Array<company>; //init of array

    ngOnInit() {
        this.GetAlleCompanies();
    }

    
    constructor(private _http: HttpClient, private router: Router) { }

    GetAlleCompanies() { //getting list of allCompanies and saving them in an company[] array
        this._http.get<company[]>("api/Stock/getAllCompanies")
            .subscribe(companies => {
                this.alleCompanies = companies; //updating array -> html can access with {{ alleCompanies }}
            },
            error => console.log(error)
        );
    }

    goToCompany(name: string) { //Redirect to company user clicked on
        this._http.get("api/Stock/SetCurrentCompany?name=" + name) //setting sesion
            .subscribe(_retur => {
                this.router.navigate(['/company']); //navigating to company component
            },
            error => console.log(error)
            );
    }
}