import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CompaniesComponent } from './companies/companies.component';
import { CompanyComponent } from './company/company.component';
import { CreateCompanyComponent } from './createcompany/createcompany.component';
import { CreateUserComponent } from './createuser/createuser.component';
import { HomeComponent } from './home/home.component';
import { LeaderboardComponent } from './leaderboard/leaderbord.component';
import { ProfileComponent } from './profile/profile.component';
import { LogInComponent } from './login/login.component';
import { UpdateUserComponent } from './update-user/update-user.component';
//import { NavbarComponent } from './nav-meny/nav-meny.component';
import { WildComponent } from './wild/wild.component';

const appRoots: Routes = [
    { path: 'home', component: HomeComponent },
    { path: 'login', component: LogInComponent },
    { path: 'createuser', component: CreateUserComponent },
    { path: 'createcompany', component: CreateCompanyComponent },
    { path: 'companies', component: CompaniesComponent },
    { path: 'leaderboard', component: LeaderboardComponent },
    { path: 'profile', component: ProfileComponent },
    { path: 'company', component: CompanyComponent },
    { path: 'updateuser', component: UpdateUserComponent },
    { path: '', redirectTo: 'home', pathMatch: 'full' },
    { path: '**', pathMatch: 'full', component: WildComponent }
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
