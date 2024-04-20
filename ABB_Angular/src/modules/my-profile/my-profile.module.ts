import { NgModule } from '@angular/core';

import { FormsModule, ReactiveFormsModule } from "@angular/forms";

import { CommonModule } from '@angular/common';
import { ThemeModules } from './../../mat_component_modules/theme.modules';

import { MyProfileRoutingModule } from "./my-profile.routing.module";
import { MyProfileComponent } from "./my-profile/my-profile.component";
import { PersonalInformationComponent } from "./personal-information/personal-information.component";
import { BillingInformationComponent } from "./biling-information/billing-information.component";
import { ChangePasswordComponent } from "./change-password/change-password.component";
import { MatCardModule } from '@angular/material/card';
import { CompanyDetailsComponent } from './Company-Details/Company-Details.component';
@NgModule({
  imports: [
    CommonModule,
    ThemeModules,
    MatCardModule,
    MyProfileRoutingModule
  ],
  exports: [],
  declarations: [MyProfileComponent,
    PersonalInformationComponent,
    BillingInformationComponent,
    CompanyDetailsComponent,
    ChangePasswordComponent,],
  providers: [],
})
export class MyProfileModule { }




