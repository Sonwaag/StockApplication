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
        //based on: https://www.w3schools.com/howto/howto_js_filter_table.asp and https://bobbyhadz.com/blog/typescript-get-value-of-input-element
        var input = document.getElementById("searchbar") as HTMLInputElement;
        input.addEventListener('input', function (event) {
            const target = event.target as HTMLInputElement;
            var str: string = target.value.toLowerCase();
            var rows = document.getElementById("tableSort").getElementsByTagName("tr");
            if (rows == null) {
                return;
            }
            for (var i = 1; i < rows.length; i++) {
                var row = rows[i].getElementsByTagName("td")[0];
                var name = row.textContent || row.innerText;
                if (name.toLowerCase().indexOf(str) > -1) {
                    rows[i].style.display = "";
                }
                else {
                    rows[i].style.display = "none";
                }
            }
        });
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