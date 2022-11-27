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
    public company: clientCompany;
    public chart: any;
    public change: string;
    public tenMin: string;
    public clientStock: clientStock;
    public user: clientUser;
    
    @ViewChild('lineChart', { static: true }) private chartRef;
    @ViewChild('input', { static: true }) input: ElementRef;

    ngOnInit() {
        this.getCurrentCompany();
        this.getStock();
        this.getUser()
    }



    constructor(private http: HttpClient, private router: Router) { }

    getCurrentCompany() {
        this.http.get<clientCompany>("api/Stock/company/current")
            .subscribe(company => {
                this.company = company;
                this.createChart(company);
                this.valueInfo(company);
            },
            error => console.log(error)
        );
    }

    createChart(company: clientCompany) {
        const parse = JSON.parse(company.values);
        this.chart = new Chart(this.chartRef.nativeElement, { //new chart with context(ctx) and assigning data + designing, companyChart is global variable because we need to access it in our ajax call
            type: 'line',
            data: {
                labels: ["10 mins ago", "9 mins ago", "8 mins ago", "7 mins ago", "6 mins ago", "5 mins ago", "4 mins ago", "3 mins ago", "2 mins ago", "1 min ago"],
                datasets: [{
                    label: company.name,
                    data: parse,
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

    getPercentage(numA: number, numB: number) {
        let number = ((numA - numB) / numB) * 100;
        return (Math.round(number * 100) / 100).toFixed(2);
    }


    getStock() {
        this.http.get<clientStock>("api/Stock/GetCurrentStock")
            .subscribe(stock => {
                this.clientStock = stock;
            },
                error => console.log(error)
        );
    }

    getUser() {
        this.http.get<clientUser>("api/Stock/GetCurrentUser")
            .subscribe(user => {
                this.user = user;
            },
                error => console.log(error)
            );
    }
    buyStock() {
        var input_amount = this.input.nativeElement.innerHTML;
        this.http.get("api/Stock/buyStock?amount=" + input_amount)
            .subscribe(_retur => {
                this.getStock();
            },
                error => console.log(error)
            );
    }

    sellStock() {
        var input_amount = this.input.nativeElement.innerHTML;
        console.log(input_amount);
        this.http.get("api/Stock/sellStock?amount=" + input_amount)
            .subscribe(_retur => {
                this.getStock();
            },
                error => console.log(error)
            );
    }
   

}