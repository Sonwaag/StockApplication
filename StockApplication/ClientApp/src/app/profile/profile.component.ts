import { Component, OnInit } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import { clientUser, clientStock } from '../TsClasses';

@Component({
    templateUrl: './profile.component.html',
})
export class ProfileComponent {
    public user: clientUser;
    public totalvalue: clientStock;
    public list: Array<clientStock>;

    constructor(private http: HttpClient, private fb: FormBuilder, private router: Router) { }

    ngOnInit() {
        this.getCurrentUser();
    }

    getCurrentUser() {
        this.http.get<clientUser>("api/Stock/getCurrentUser")
            .subscribe(user => {
                this.user = user;
                this.getUsersTotalValue(); 
                this.getUsersStocks();
            },
                error => console.log(error)
            );
    };
    getUsersTotalValue() {
        this.http.get<clientStock>("api/Stock/getUserValue")
            .subscribe(totalvalue => {
                this.totalvalue = totalvalue;
            },
                error => console.log(error)
            );
    };
    getUsersStocks() {
        this.http.get<clientStock[]>("api/Stock/getStocksForUser")
            .subscribe(list => {
                this.list = list;
            },
                error => console.log(error)
            );
    };

    deleteUser() {
        this.http.delete("api/Stock/deleteUser")
            .subscribe(_retur => {
                this.router.navigate(['/']);
            },
                error => console.log(error)
            );
    };

    logOut() {
        this.http.get("api/Stock/logOut/")
            .subscribe(_retur => {
                this.router.navigate(['/']);
            },
                error => console.log(error)
            );
    };
   
}