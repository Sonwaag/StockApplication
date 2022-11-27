import { Component, OnInit } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import { user } from '../userClass';

@Component({
    templateUrl: 'createuser.component.html'
})
export class CreateUserComponent {
    skjema: FormGroup;

    validering = {
        
        username: [
            null, Validators.compose([Validators.required, Validators.pattern("[a-zA-ZøæåØÆÅ\\-. ]{2,30}")])
        ],
        password: [
            null, Validators.compose([Validators.required, Validators.pattern("[a-zA-ZøæåØÆÅ\\-. ]{2,30}")])
        ]
    }

    constructor(private http: HttpClient, private fb: FormBuilder, private router: Router) {
        this.skjema = fb.group(this.validering);
    }

    onSubmit() {
        this.createUser();
    }

    createUser() {
        var inndata = { username: this.skjema.value.username, password: this.skjema.value.password }
        
        this.http.post("api/Stock/createUser", inndata)
          .subscribe(_retur => {
              this.router.navigate(['/']);
          },
          error => console.log(error)
          );
    };
}