import { NgModule } from "@angular/core";
import { RouterModule } from "@angular/router";
import { SalesReceiptComponent } from './SalesReceipt/Sales-Receipt.component'
import { ReceivedPaymentsComponent } from './receieved-payments/received-payments.component'
import { CreditNoteComponent } from "./Credit-Note/Credit-Note.component";
import { AppRouteGuard } from "@shared/auth/auth-route-guard";
import { PrintReceivedPaymentComponent } from "./print-received-payment/print-received-payment.component";
import { PrintInvoicesComponent } from "./print-invoices/print-invoices.component";
import { SalesTransactionComponent } from "./sales-transaction/sales-transation.component";
import { JournalVoucherComponent } from "./journal-voucher/journal-voucher.component";


@NgModule({

  imports: [
    RouterModule.forChild([
      {
        path: "sales-transation",
        component: SalesTransactionComponent,
        canActivate: [AppRouteGuard],
        data: { Permissions: "Pages.AllSales.SalesTransaction" }
      },
      // {
      //   path: "invoices",
      //   component: InvoicesComponent,
      //   canActivate: [AppRouteGuard],
      //   data: { Permissions: "Pages.AllSales.Invoice" }
      // },
      {
        path: "print-invoice",
        component: PrintInvoicesComponent,
        canActivate: [AppRouteGuard],
        data: { Permissions: "Pages.AllSales.Invoice" }
      },
      {
        path: "print-invoice/:id",
        component: PrintInvoicesComponent,
        canActivate: [AppRouteGuard],
        data: { Permissions: "Pages.AllSales.Invoice" }
      },
      {
        path: 'received-payment',
        component: ReceivedPaymentsComponent,
        canActivate: [AppRouteGuard],
        data: { Permissions: "Pages.AllSales.ReceivedPayment" }
      },
      {
        path: "received-payment/:id",
        component: ReceivedPaymentsComponent,
        canActivate: [AppRouteGuard],
        data: { Permissions: "Pages.AllSales.ReceivedPayment" }
      },
      {
        path: "print-received-payment",
        component: PrintReceivedPaymentComponent,
        canActivate: [AppRouteGuard],
        data: { Permissions: "Pages.AllSales.ReceivedPayment" }
      },
      {
        path: 'sales-receipt',
        component: SalesReceiptComponent,
        canActivate: [AppRouteGuard],
        data: { Permissions: "Pages.AllSales.SalesReceipt" }
      },
      {
        path: 'credit-note',
        component: CreditNoteComponent,
        canActivate: [AppRouteGuard],
        data: { Permissions: "Pages.AllSales.CreditNote" }
      },
      // {
      //   path: 'Estimate',
      //   component: EstimateComponent,
      //   canActivate: [AppRouteGuard],
      //   data: { Permissions: "Pages.AllSales.Estimate" }
      // },
      // {
      //   path: 'Estimate/:id',
      //   component: EstimateComponent,
      //   canActivate: [AppRouteGuard],
      //   data: { Permissions: "Pages.AllSales.Estimate" }
      // },
      {
        path: "journal-voucher",
        component: JournalVoucherComponent,
        canActivate: [AppRouteGuard],
        data: { Permissions: "Pages.JournalVoucher" }
      },
    ]),
  ],
  exports: [RouterModule],
})
export class AllSalesRoutingModule { }
