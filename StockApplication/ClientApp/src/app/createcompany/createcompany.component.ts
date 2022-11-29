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
    public hidden: boolean = true; //error text box

    ngOnInit() {
        this.getCurrentUser();
    }
    getCurrentUser() {
        this.http.get<clientUser>("api/Stock/getCurrentUser") //get active user-session to ensure user is logged in
            .subscribe(user => {
                
            },
                error => {
                    console.log(error);
                    if (error.status === 401 || error.status === 404) { //unauthorized, need to log in
                        this.router.navigate(['/login']); //redirect to login
                    }
                }
            );
    };
    validering = {
        
        companyName: [
            null, Validators.compose([Validators.required, Validators.pattern("[0-9a-zA-ZøæåØÆÅ\\-. ]{3,32}")]) //companyname can be all characters between length 3 and 32
        ]
    }

    constructor(private http: HttpClient, private fb: FormBuilder, private router: Router) {
        this.skjema = fb.group(this.validering); //validate input 
    }

    onSubmit() {
        this.createCompany();
    }

    

    createCompany() {
        const companyName = this.skjema.value.companyName; //get value

        this.http.post("api/Stock/" + companyName, companyName) //create company with name
            .subscribe(_retur => {
                this.router.navigate(['/']); //navigate to home on success
            },
                error => {
                    console.log(error);
                    if (error.status === 401) { //Unauthorized, user not logged in
                        this.router.navigate(['/login']);
                    }
                    else if (error.status === 400) { //BadRequest, invalid name, or already exists.
                        this.errTxt = "Company already exists!";
                        this.hidden = false;
                    }
                }
            );
    };
}