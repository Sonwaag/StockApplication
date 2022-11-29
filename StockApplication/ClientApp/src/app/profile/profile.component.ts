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
    public errTxt: string;

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
                error => {
                    console.log(error);
                    if (error.status === 401 || error.status === 404) {
                        this.router.navigate(['/login']);
                    }
                }
            );
    };
    getUsersTotalValue() {
        this.http.get<clientStock>("api/Stock/getUserValue")
            .subscribe(totalvalue => {
                this.totalvalue = totalvalue;
            },
                error => {
                    console.log(error);
                    if (error.status === 401 || error.status === 404) {
                        this.errTxt = "Could not find current user";
                    }
                }

            );
    };
    getUsersStocks() {
        this.http.get<clientStock[]>("api/Stock/getStocksForUser")
            .subscribe(list => {
                this.list = list;
            },
                error => {
                    console.log(error);
                    if (error.status === 401) {
                        this.router.navigate(['/login']);
                    }
                }
            );
    };

    clickMethod(name: string) {
        if (confirm("Are you sure you want to delete this user?")) {
            console.log(
            this.http.delete("api/Stock/deleteUser")
                .subscribe(_retur => {
                    this.router.navigate(['/']);
                },
                    error => {
                        console.log(error);
                        if (error.status === 401) {
                            this.router.navigate(['/login']);
                        }
                        if (error.status === 400) {
                            this.errTxt = "Could not delete user";
                        }
                    }
                )
            )
        }
    }

    logOut() {
        this.http.get("api/Stock/logOut/")
            .subscribe(_retur => {
                this.router.navigate(['/']);
            },
                error => {
                    console.log(error);
                    this.errTxt = "Could not log out. Sorry you're stuck here :)";
                }
            );
    };
   
}