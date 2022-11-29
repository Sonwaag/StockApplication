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
    public hidden: boolean = true; //error text box

    validering = {
        
        username: [
            null, Validators.compose([Validators.required, Validators.pattern("[0-9a-zA-ZøæåØÆÅ\\-. ]{3,12}")]) //all characters between 3 and 12 
        ],
        password: [
            null, Validators.compose([Validators.required, Validators.pattern("[0-9a-zA-ZøæåØÆÅ\\-. ]{8,32}")]) //all characters between 8 and 32
        ]
    }

    constructor(private http: HttpClient, private fb: FormBuilder, private router: Router) {
        this.skjema = fb.group(this.validering); //validate input
    }

    onSubmit() {
        this.createUser();
    }

    createUser() {
        var inndata = { username: this.skjema.value.username, password: this.skjema.value.password } //create object matching login.cs class = string username, string password
        
        this.http.post("api/Stock/createUser", inndata) //create user with log in information
          .subscribe(_retur => {
              this.router.navigate(['/profile']); //if success redirect to profilepage
          },
              error => {
                  console.log(error);
                  if (error.status === 400) { //badrequest username is taken or input error
                      this.errTxt = "Username is already taken!";
                      this.hidden = false; //show error text
                  }
              }
          );
    };
}