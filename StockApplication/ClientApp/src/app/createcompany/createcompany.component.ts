import { Component, OnInit } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import { clientUser } from '../TsClasses';


@Component({
    templateUrl: 'createcompany.component.html'
})
export class CreateCompanyComponent {
    skjema: FormGroup;
    public errTxt: string;
    public hidden: boolean = true;

    ngOnInit() {
        this.getCurrentUser();
    }
    getCurrentUser() {
        this.http.get<clientUser>("api/Stock/getCurrentUser")
            .subscribe(user => {
                
            },
                error => {
                    console.log(error);
                    if (error.status === 401 || error.status === 404) {
                        this.router.navigate(['/login']);
                    }
                }
            );
    };
    validering = {
        
        companyName: [
            null, Validators.compose([Validators.required, Validators.pattern("[0-9a-zA-ZøæåØÆÅ\\-. ]{3,32}")])
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
                error => {
                    console.log(error);
                    if (error.status === 401) {
                        this.router.navigate(['/login']);
                    }
                    else if (error.status === 400) {
                        this.errTxt = "Company already exists!";
                        this.hidden = false;
                    }
                }
            );
    };
}