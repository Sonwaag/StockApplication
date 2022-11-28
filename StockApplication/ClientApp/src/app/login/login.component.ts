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

    validering = {

        username: [
            null, Validators.compose([Validators.required, Validators.pattern("[a-zA-ZøæåØÆÅ\\-. ]{3,12}")])
        ],
        password: [
            null, Validators.compose([Validators.required, Validators.pattern("[a-zA-ZøæåØÆÅ\\-. ]{8,32}")])
        ]
    }

    constructor(private http: HttpClient, private fb: FormBuilder, private router: Router) {
        this.skjema = fb.group(this.validering);
    }


    onSubmit() {
        this.logIn();
    }

    logIn() {
        var inndata = { username: this.skjema.value.username, password: this.skjema.value.password }

        this.http.post("api/Stock/logIn", inndata)
            .subscribe(_retur => {
                this.router.navigate(['/']);
            },
                error => console.log(error)
            );
    };
}
