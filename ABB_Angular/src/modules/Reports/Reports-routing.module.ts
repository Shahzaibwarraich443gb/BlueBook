import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { EmployeeActivitiesComponent } from './Employee-Activities/Employee-Activities.component';
import { DailyReceiptComponent } from './Daily-Recepit/daily-recepit.component';
import { GeneralLedgerComponent } from './General-Ledger/General-Ledger.component';
import { CustomerLedgerComponent } from './Customer-Ledger/Customer-Ledger.component';
import { TrialBalanceComponent } from './Trial-Balance/Trial-Balance.component';
@NgModule({
  imports: [
    RouterModule.forChild([
      {
        path: 'employee-activities',
        component: EmployeeActivitiesComponent,
      },
      {
        path: 'daily-receipt',
        component: DailyReceiptComponent,
      },
      {
        path: "general-ledger",
        component: GeneralLedgerComponent,
      },
      {
        path: "customer-ledger",
        component: CustomerLedgerComponent
      },
      {
        path: "trial-balance",
        component: TrialBalanceComponent
      },

      // {
      //   path: "Source-Referal",
      //   component: SourceReferalComponent,

      // }
    ])
  ],
  exports: [RouterModule]
})
export class ReportRoutingModule { }
