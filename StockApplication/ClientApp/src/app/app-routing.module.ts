import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CompaniesComponent } from './companies/companies.component';
import { CompanyComponent } from './company/company.component';
import { CreateCompanyComponent } from './createcompany/createcompany.component';
import { CreateUserComponent } from './createuser/createuser.component';
import { HomeComponent } from './home/home.component';
import { LeaderboardComponent } from './leaderboard/leaderbord.component';
import { ProfileComponent } from './profile/profile.component';
import { SwapPageComponent } from './swappage/swappage.component';

const appRoots: Routes = [
    { path: 'home', component: HomeComponent },
    { path: 'swappage', component: SwapPageComponent },
    { path: 'createuser', component: CreateUserComponent },
    { path: 'createcompany', component: CreateCompanyComponent },
    { path: 'companies', component: CompaniesComponent },
    { path: 'leaderboard', component: LeaderboardComponent },
    { path: 'profile', component: ProfileComponent },
    { path: 'company', component: CompanyComponent },
    { path:'', redirectTo:'home', pathMatch:'full' }
]

@NgModule({
    imports: [
        RouterModule.forRoot(appRoots)
    ],
    exports: [
        RouterModule
    ]
})

export class AppRoutingModule { }
