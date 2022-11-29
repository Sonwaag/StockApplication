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
        this.http.get<clientUser>("api/Stock/getCurrentUser") //getCurrentUser
            .subscribe(user => {
                this.user = user;
                this.getUsersTotalValue();
                this.getUsersStocks();
            },
                error => {
                    console.log(error); //Unauthorized, user not logged in, redirect to login page
                    if (error.status === 401 || error.status === 404) {
                        this.router.navigate(['/login']);
                    }
                }
            );
    };
    getUsersTotalValue() {
        this.http.get<clientStock>("api/Stock/getUserValue") //get totalvalue of user to display value of all assets
            .subscribe(totalvalue => {
                this.totalvalue = totalvalue;
            },
                error => {
                    console.log(error);
                    if (error.status === 401 || error.status === 404) { //notfound or unauthorized, show error message
                        this.errTxt = "Could not find current user";
                    }
                }

            );
    };
    getUsersStocks() {
        this.http.get<clientStock[]>("api/Stock/getStocksForUser") //get list of all owned stocks
            .subscribe(list => {
                this.list = list;
            },
                error => {
                    console.log(error);
                    if (error.status === 401) { //not logged in redirect to login
                        this.router.navigate(['/login']);
                    }
                }
            );
    };

    clickMethod() {
        if (confirm("Are you sure you want to delete this user?")) { //confirm box to confirm if you want to delete user or not
            console.log(
            this.http.delete("api/Stock/deleteUser")
                .subscribe(_retur => {
                    this.router.navigate(['/']); //return to home page on succes
                },
                    error => {
                        console.log(error);
                        if (error.status === 401) { //unauthorized please log in
                            this.router.navigate(['/login']);
                        }
                        if (error.status === 400) {
                            this.errTxt = "Could not delete user"; //couold not delete user
                        }
                    }
                )
            )
        }
    }

    logOut() {
        this.http.get("api/Stock/logOut/")
            .subscribe(_retur => {
                this.router.navigate(['/']); //log out success, return to homepage
            },
                error => {
                    console.log(error);
                    this.errTxt = "Could not log out. Sorry you're stuck here :)"; //something went wrong on server side
                }
            );
    };
   
}