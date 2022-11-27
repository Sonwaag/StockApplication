import { Component, OnInit } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
    templateUrl: 'update-user.component.html'
})
export class UpdateUserComponent {
    skjema: FormGroup;
    

    validering = {

        username: [
            null, Validators.compose([Validators.required, Validators.pattern("[a-zA-ZøæåØÆÅ\\-. ]{2,30}")])
        ],
    }

    constructor(private http: HttpClient, private fb: FormBuilder, private router: Router) {
        this.skjema = fb.group(this.validering);
    }

    onSubmit() {
        this.updateUser();
    }

    updateUser() {
        var name = this.skjema.value.username;

        this.http.put("api/Stock/updateUser", name)
            .subscribe(_retur => {
                this.router.navigate(['/']);
            },
            error => console.log(error)

            );
    };
}