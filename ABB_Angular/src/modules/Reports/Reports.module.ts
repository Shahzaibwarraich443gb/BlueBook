import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ThemeModules } from './../../mat_component_modules/theme.modules';
import { ReportRoutingModule } from './Reports-routing.module';
import { EmployeeActivitiesComponent } from './Employee-Activities/Employee-Activities.component';
import { DailyReceiptComponent } from './Daily-Recepit/daily-recepit.component';
import { GeneralLedgerComponent } from './General-Ledger/General-Ledger.component'
import { SharedModule } from 'primeng/api';
import { NgxPrintModule } from 'ngx-print';
import {CustomerLedgerComponent} from './Customer-Ledger/Customer-Ledger.component';
import {TrialBalanceComponent} from './Trial-Balance/Trial-Balance.component';

@NgModule({
  imports: [
    CommonModule,
    ThemeModules,
    ReportRoutingModule,
    SharedModule,
    NgxPrintModule
  ],
  exports: [],
  declarations: [
    DailyReceiptComponent,
    EmployeeActivitiesComponent,
    GeneralLedgerComponent,
    CustomerLedgerComponent,
    TrialBalanceComponent
  ],
  providers: [],
})
export class ReportModule { }
