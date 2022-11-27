import { Component, OnInit } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import { clientStock } from '../TsClasses';

@Component({
    templateUrl: './leaderboard.component.html'
})
export class LeaderboardComponent {
    public allUsers: Array<clientStock>;

    ngOnInit() {
        this.GetAlleUsers();
    }

    constructor(private _http: HttpClient, private router: Router) { }

    GetAlleUsers() {
        this._http.get<clientStock[]>("api/stock/getUsersValue")
            .subscribe(users => {
                this.allUsers = users;
            },
                error => console.log(error)
            );
    }
}