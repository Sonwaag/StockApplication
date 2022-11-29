import { Component, OnInit } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
    templateUrl: 'update-user.component.html'
})
export class UpdateUserComponent {
    skjema: FormGroup;
    public hidden: boolean = true;
    public errTxt: string;

    validering = {

        username: [
            null, Validators.compose([Validators.required, Validators.pattern("[0-9a-zA-ZøæåØÆÅ\\-. ]{3,12}")]) //username needs to be between 3 and 12 characters
        ],
    }

    constructor(private http: HttpClient, private fb: FormBuilder, private router: Router) {
        this.skjema = fb.group(this.validering); //validate input
    }

    onSubmit() {
        this.updateUser();
    }

    updateUser() {
        const obj = {
            value: this.skjema.value.username //new user name, StringWrapper
        };

        this.http.put("api/Stock/updateUser", obj)
            .subscribe(_retur => {
                this.router.navigate(['/profile']); //user updated, return to profilepage
            },
                error => {
                    console.log(error);
                    if (error.status === 401) { //user not logged in, return to login
                        this.router.navigate(['/login']);
                    }
                    else if (error.status === 400) { //username is taken
                        this.errTxt = "Username is taken!";
                        this.hidden = false; //display error text
                    }
                }

            );
    };
}