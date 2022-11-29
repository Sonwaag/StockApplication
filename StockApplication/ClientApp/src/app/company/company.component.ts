import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { clientCompany } from '../TsClasses';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Chart } from 'chart.js';
import { clientStock, clientUser } from '../TsClasses';

@Component({
    templateUrl: 'company.component.html'
})

export class CompanyComponent {
    public hidden: boolean = true; //error text box
    public company: clientCompany;
    public chart: any;
    public change: string;
    public tenMin: string;
    public clientStock: clientStock;
    public user: clientUser;
    public value: number;
    public errTxt: string;
    
    
    @ViewChild('lineChart', { static: true }) private chartRef; //lineChart canvas child so we can set the content from typescript
    

    ngOnInit() { //calling functions on page load
        this.getCurrentCompany();
        this.getStock();
        this.getUser()
    }



    constructor(private http: HttpClient, private router: Router) { }

    getCurrentCompany() {
        this.http.get<clientCompany>("api/Stock/company/current") //get current company session
            .subscribe(company => {
                this.company = company;
                this.createChart(company);
                this.valueInfo(company);
            },
                error => {
                    console.log(error)
                    if (error.status === 401 || error.status === 404) {
                        this.router.navigate(['/companies']); //something wrong with company, return to list
                    }
                }
        );
    }

    createChart(company: clientCompany) { //create a chart for the current company
        const parse = JSON.parse(company.values); //parsing json to get values as an array
        this.chart = new Chart(this.chartRef.nativeElement, { //new chart with context(ctx) and assigning data + designing, companyChart is global variable because we need to access it in our ajax call
            type: 'line',
            data: {
                labels: ["10 mins ago", "9 mins ago", "8 mins ago", "7 mins ago", "6 mins ago", "5 mins ago", "4 mins ago", "3 mins ago", "2 mins ago", "1 min ago"],
                datasets: [{
                    label: company.name,
                    data: parse, //plot data in chart 
                    fill: false,
                    borderColor: 'rgb(75,192,192)',
                    pointHitRadius: 100,
                    pointRadius: 5
                }]
            }
        }); 
    }

    valueInfo(company: clientCompany) {
         //parsing json to get values as an array
        let array = JSON.parse(company.values); 
        this.change = this.getPercentage(array[array.length - 1], array[array.length - 2]); //increase/decrease percentage from last two values
        this.tenMin = this.getPercentage(array[array.length - 1], array[0]); //increase/decrease percentage from last ten minutes. (first value and last value compared)
    }

    getPercentage(numA: number, numB: number) { //function to get percentage increase/decrease in value
        let number = ((numA - numB) / numB) * 100;
        return (Math.round(number * 100) / 100).toFixed(2);
    }


    getStock() {
        this.http.get<clientStock>("api/Stock/GetCurrentStock") //returns amount of shares user owns at current company
            .subscribe(stock => {
                this.clientStock = stock;
            },
                error => {
                    console.log(error);
                    this.router.navigate(['/login']); //redirect to login if something went wrong
                }
        );
    }

    getUser() {
        this.http.get<clientUser>("api/Stock/GetCurrentUser") //get current user to displayer user.balance
            .subscribe(user => {
                this.user = user;
            },
                error => {
                    console.log(error);
                    this.router.navigate(['/login']); //redirect to login if something went wrong
                }
            );
    }
    buyStock() {
        const obj = {
            value: (<HTMLInputElement>document.getElementById("amount")).value //get input value from input field
        };
        console.log(obj);
        this.http.put("api/Stock/buyStock", obj)
            .subscribe(_retur => {
                this.getStock(); //update components if success
                this.getUser();
                this.getCurrentCompany();
                this.hidden = true;
            },
                error => {
                    console.log(error);
                    if (error.status === 401) { //Unauthorized, please log in
                        this.router.navigate(['/login']);
                    }
                    else if (error.status === 400) { //BadRequest, dberror or user cannot afford to buy
                        this.errTxt = "Can not buy this amount of shares!";
                        this.hidden = false;
                    }
                }
            );
    }

    sellStock() {   
        const obj = {
            value: (<HTMLInputElement>document.getElementById("amount")).value //get input value from input field
        };
        this.http.put("api/Stock/sellStock", obj)
            .subscribe(_retur => {
                this.getStock(); //update components if success
                this.getUser();
                this.getCurrentCompany();
                this.hidden = true;
            },
                error => {
                    console.log(error);
                    if (error.status === 401) { //Unauthorized, please log in
                        this.router.navigate(['/login']);
                    }
                    else if (error.status === 400) { //BadRequest, dberror or user do not own this amount
                        this.errTxt = "Can not sell this amount of shares!";
                        this.hidden = false;
                    }
                }
            );
    }
   

}