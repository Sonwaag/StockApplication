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
    public errTxt: string;
    public hidden: boolean = true;

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
        this.createUser();
    }

    createUser() {
        var inndata = { username: this.skjema.value.username, password: this.skjema.value.password }
        
        this.http.post("api/Stock/createUser", inndata)
          .subscribe(_retur => {
              this.router.navigate(['/profile']);
          },
              error => {
                  console.log(error);
                  if (error.status === 400) {
                      this.errTxt = "Username is already taken!";
                      this.hidden = false;
                  }
              }
          );
    };
}