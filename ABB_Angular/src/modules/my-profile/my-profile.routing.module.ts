import { NgModule } from "@angular/core";
import { RouterModule } from "@angular/router";
import { AppRouteGuard } from "@shared/auth/auth-route-guard";
import { BillingInformationComponent } from "./biling-information/billing-information.component";
import { CompanyDetailsComponent } from "./Company-Details/Company-Details.component";
import { MyProfileComponent } from "./my-profile/my-profile.component";

//import { FormW2Component } from "./form-W2/form-W2.component";
//import { StartNewFilingComponent } from "./start-new-filing/start-new-filing.component";

@NgModule({
  imports: [
    RouterModule.forChild([
      {
        path: "",
        children: [
          {
            path: "",
            redirectTo: "profile",
            // component: MyProfileComponent,
            pathMatch: "full",
            // data: { permisison: 'Pages.Tenant.Dashboard' },
          },
          {
            path: "profile",
            component: MyProfileComponent,
            // data: { permission: "Pages.Tenant.MyProfile.PersonalInformation" },
            // canActivate: [AppRouteGuard],
          },
          {
            path: "billing-information",
            component: BillingInformationComponent,
            // data: { permission: "Pages.Tenant.MyProfile.BillingInformation" },
            // canActivate: [AppRouteGuard],
          },
          // {
          //   path: "Companys-Details",
          //   component: CompanyDetailsComponent,
          //   // data: { permission: "Pages.Tenant.MyProfile.BillingInformation" },
          //   // canActivate: [AppRouteGuard],
          // },


        ],
      },
    ]),
  ],
  exports: [RouterModule],
})
export class MyProfileRoutingModule { }
