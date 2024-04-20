import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { PurchaseInvoiceComponent } from './Purchase-Invoice/Purchase-Invoice.component';
import { AddCheckComponent } from './Check/Add-Check.component';
import { CheckComponent } from './Check/Check.component';
import { AppRouteGuard } from '@shared/auth/auth-route-guard';
import { CheckSetupComponent } from './Check/Check-Setup.component';
import { PurchaseReceiptComponent } from 'modules/all-sales/Purchase-Receipt/Purchase-Receipt.component';
import { PurchasePaymentComponent } from './Purchase-Payment/purchase-payment.component';

@NgModule({
    imports: [
        RouterModule.forChild([
            {
                path: "purchase-receipt",
                component: PurchaseReceiptComponent,
                canActivate: [AppRouteGuard],
                data: { permission: "Pages.AllExpense.PurchaseReceipt" },
            },
            {
                path: "purchase-invoice",
                component: PurchaseInvoiceComponent,
                canActivate: [AppRouteGuard],
                data: { permission: "Pages.AllExpense.PurchaseInvoice" },
            },
            {
                path: "purchase-invoice/:id",
                component: PurchaseInvoiceComponent,
                canActivate: [AppRouteGuard],
                data: { permission: "Pages.AllExpense.PurchaseInvoice" },
            },
            {
                path: "purchase-payment",
                component: PurchasePaymentComponent,
                canActivate: [AppRouteGuard],
                data: { permission: "Pages.AllExpense.PurchasePayment" },
            },
            {
                path: "addCheck/:id",
                component: AddCheckComponent,
                canActivate: [AppRouteGuard],
                data: { permission: "Pages.AllExpense.Check" },
            },
            {
                path: "addCheck",
                component: AddCheckComponent,
                canActivate: [AppRouteGuard],
                data: { permission: "Pages.AllExpense.Check" },
            },
            {
                path: "check",
                component: CheckComponent,
                canActivate: [AppRouteGuard],
                data: { permission: "Pages.AllExpense.Check" },
            },
            {
                path: "checkSetup",
                component: CheckSetupComponent,
                canActivate: [AppRouteGuard],
                data: { permission: "Pages.AllExpense.Check" },
            },
        ])
    ],
    exports: [RouterModule]
})
export class PurchaseInvoiceRoutingModule { }
