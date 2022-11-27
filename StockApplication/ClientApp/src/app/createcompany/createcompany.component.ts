import { Component, OnInit } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';


@Component({
    templateUrl: 'createcompany.component.html'
})
export class CreateCompanyComponent {
    skjema: FormGroup;
    

    validering = {
        
        companyName: [
            null, Validators.compose([Validators.required, Validators.pattern("[a-zA-ZøæåØÆÅ\\-. ]{2,30}")])
        ]
    }

    constructor(private http: HttpClient, private fb: FormBuilder, private router: Router) {
        this.skjema = fb.group(this.validering);
    }

    onSubmit() {
        this.createCompany();
    }

    

    createCompany() {
        const companyName = this.skjema.value.companyName;

        this.http.post("api/Stock/" + companyName, companyName)
            .subscribe(_retur => {
                this.router.navigate(['/']);
            },
            error => console.log(error)
            );
    };
}