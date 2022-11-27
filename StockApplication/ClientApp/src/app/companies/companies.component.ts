import { Component, OnInit } from '@angular/core';
import { company } from '../TsClasses';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';

@Component({
    templateUrl: 'companies.component.html'
})
export class CompaniesComponent {
    public alleCompanies: Array<company>;

    ngOnInit() {
        this.GetAlleCompanies();
    }

    
    constructor(private _http: HttpClient, private router: Router) { }

    GetAlleCompanies() {
        this._http.get<company[]>("api/Stock/")
            .subscribe(companies => {
                this.alleCompanies = companies;
            },
            error => console.log(error)
        );
    }

    goToCompany(id: string) {
        this._http.get("api/Stock/SetCurrentCompany?id=" + id)
            .subscribe(_retur => {
                console.log("Session set"+ id)
                this.router.navigate(['/company']);
            },
            error => console.log(error)
            );
    }
}