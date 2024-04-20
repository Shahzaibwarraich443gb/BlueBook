
import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { MatTooltipModule } from "@angular/material/tooltip";
import { ThemeModules } from "./../../mat_component_modules/theme.modules";
import { ReceivedPaymentsComponent } from './receieved-payments/received-payments.component';
import { SalesTransactionComponent } from './sales-transaction/sales-transation.component';
import { CreatePaymentMethodComponent } from './create-Payment-Method/create-Payement-Method.component';
import { SalesReceiptComponent } from './SalesReceipt/Sales-Receipt.component';
import { CreditNoteComponent } from './Credit-Note/Credit-Note.component';
import { ViewHistoryComponent } from './view-history/view-history.component';
import { AllSalesRoutingModule } from './all-sales-routing.module'
import { SharedModule } from 'shared/shared.module'
import { CreditCardComponent } from './credit-card-modal/credit-card.component';
import { MatSortModule } from "@angular/material/sort";
import { PurchaseReceiptComponent } from "./Purchase-Receipt/Purchase-Receipt.component";
// import { NgxPrintElementModule } from 'ngx-print-element';
import { PrintReceivedPaymentComponent } from './print-received-payment/print-received-payment.component';
import { PrintInvoicesComponent } from './print-invoices/print-invoices.component';
import { PrintCreditNoteComponent } from './print-Credit-Note/print-credit-note.component';
import { MatDividerModule } from '@angular/material/divider';
import { EmailComponent } from './email-modal/email.component';
import { JournalVoucherComponent } from './journal-voucher/journal-voucher.component';
import { JournalVoucherListComponent } from './journal-voucher-list/journal-voucher-list.component';

@NgModule({
  imports: [
    CommonModule,
    ThemeModules,
    MatSortModule,
    MatDividerModule,
    AllSalesRoutingModule,
    MatTooltipModule,
    SharedModule,
    // NgxPrintElementModule
  ],
  declarations: [
    ReceivedPaymentsComponent,
    SalesTransactionComponent,
    ViewHistoryComponent,
    CreatePaymentMethodComponent,
    SalesReceiptComponent,
    CreditCardComponent,
    PurchaseReceiptComponent,
    CreditNoteComponent,
    CreditCardComponent,
    JournalVoucherComponent,
    JournalVoucherListComponent,
    EmailComponent,
    PrintInvoicesComponent,
    PrintReceivedPaymentComponent,
    PrintCreditNoteComponent,

  ],
})
export class AllSalesModule { }
