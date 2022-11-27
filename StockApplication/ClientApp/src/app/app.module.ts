import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { ReactiveFormsModule } from '@angular/forms';

import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing.module';
import { HomeComponent } from './home/home.component';
import { NavMenuComponent } from './nav-meny/nav-meny.component';
import { FooterMenuComponent } from './footer-meny/footer-meny.component';
import { LogInComponent } from './login/login.component';
import { CreateUserComponent } from './createuser/createuser.component';
import { CreateCompanyComponent } from './createcompany/createcompany.component';
import { CompaniesComponent } from './companies/companies.component';
import { LeaderboardComponent } from './leaderboard/leaderbord.component';
import { ProfileComponent } from './profile/profile.component';
import { CompanyComponent } from './company/company.component';
import { UpdateUserComponent } from './update-user/update-user.component';

@NgModule({
  declarations: [
      AppComponent,
      HomeComponent,
      NavMenuComponent,
      FooterMenuComponent,
      LogInComponent,
      CreateUserComponent,
      CreateCompanyComponent,
      CompaniesComponent,
      LeaderboardComponent,
      ProfileComponent,
      CompanyComponent,
      UpdateUserComponent

  ],
  imports: [
      BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
      HttpClientModule,
      ReactiveFormsModule,
      AppRoutingModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
