import { Component, OnInit } from '@angular/core';
import { user } from '../userClass';
import { HttpClient } from '@angular/common/http';

@Component({
    templateUrl: './leaderboard.component.html'
})
export class LeaderboardComponent {
    public alleUsers: Array<user>;

    ngOnInit() {
        this.GetAlleUsers();
    }

    constructor(private _http: HttpClient) { }

    GetAlleUsers() {
        this._http.get<user[]>("api/stock/getAllUsers")
            .subscribe(users => {
                this.alleUsers = users;
            },
                error => console.log(error)
            );
    }
}