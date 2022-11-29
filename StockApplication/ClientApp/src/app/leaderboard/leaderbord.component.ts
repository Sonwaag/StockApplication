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
    seconds: number = 2;
    ngOnInit() {
        this.GetAlleUsers();
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
                var row = rows[i].getElementsByTagName("td")[1];
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

    GetAlleUsers() {
        this._http.get<clientStock[]>("api/stock/getUsersValue") //get list of all Users value to display them in a leaderboard
            .subscribe(users => {
                this.allUsers = users;
            },
                error => console.log(error)
            );
    }

}