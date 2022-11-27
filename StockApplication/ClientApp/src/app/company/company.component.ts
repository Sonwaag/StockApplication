import { Component, OnInit, ViewChild } from '@angular/core';
import { clientCompany } from '../TsClasses';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Chart } from 'chart.js';

@Component({
    templateUrl: 'company.component.html'
})

export class CompanyComponent {
    public company: clientCompany;
    public chart: any;
    @ViewChild('lineChart', { static: true }) private chartRef;

    ngOnInit() {
        this.getCurrentCompany();
    }



    constructor(private http: HttpClient, private router: Router) { }

    getCurrentCompany() {
        this.http.get<clientCompany>("api/Stock/company/current")
            .subscribe(company => {
                this.company = company;
                this.createChart(company);
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

}