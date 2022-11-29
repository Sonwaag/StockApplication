import { Component, OnInit } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import { user } from '../userClass';

@Component({
    templateUrl: 'login.component.html'
})
export class LogInComponent {
    skjema: FormGroup;
    public errTxt: string;
    public hidden: boolean = true; //error text box hidden

    validering = {

        username: [
            null, Validators.compose([Validators.required, Validators.pattern("[0-9a-zA-ZøæåØÆÅ\\-. ]{3,12}")]) //all characters between 3 and 12 
        ],
        password: [
            null, Validators.compose([Validators.required, Validators.pattern("[0-9a-zA-ZøæåØÆÅ\\-. ]{8,32}")]) //all characters between 8 and 32 
        ]
    }

    constructor(private http: HttpClient, private fb: FormBuilder, private router: Router) {
        this.skjema = fb.group(this.validering); //validate
    }


    onSubmit() {
        this.logIn();
    }

    logIn() {
        var inndata = { username: this.skjema.value.username, password: this.skjema.value.password } //create object matching login.cs class = string username, string password

        this.http.post("api/Stock/logIn", inndata) //try to logIn with username and password
            .subscribe(_retur => {
                this.router.navigate(['/profile']); //on success return to profile
            },
                error => {
                    console.log(error);
                    if (error.status === 400 || error.status === 404) { //Unauthorized or NotFound, username or password is wrong
                        this.errTxt = "Username or password was wrong!";
                        this.hidden = false; //show error box
                    }
                }
            );
    };
}
