import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { ReactiveFormsModule } from '@angular/forms';

import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing.module';
import { HomeComponent } from './home/home.component';
import { NavMenuComponent } from './nav-meny/nav-meny.component';
import { FooterMenuComponent } from './footer-meny/footer-meny.component';
import { SwapPageComponent } from './swappage/swappage.component';
import { CreateUserComponent } from './createuser/createuser.component';
import { CreateCompanyComponent } from './createcompany/createcompany.component';
import { CompaniesComponent } from './companies/companies.component';
import { LeaderboardComponent } from './leaderboard/leaderbord.component';
import { ProfileComponent } from './profile/profile.component';
import { CompanyComponent } from './company/company.component';

@NgModule({
  declarations: [
      AppComponent,
      HomeComponent,
      NavMenuComponent,
      FooterMenuComponent,
      SwapPageComponent,
      CreateUserComponent,
      CreateCompanyComponent,
      CompaniesComponent,
      LeaderboardComponent,
      ProfileComponent,
      CompanyComponent
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
