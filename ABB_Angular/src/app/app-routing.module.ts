import { EntityTypeComponent } from 'modules/entity-type/entity-type.component';
import { SourceReferalComponent } from './../modules/source-referal/source-referal.component';
import { languageComponent } from './../modules/Language/language.component';
import { NgModule } from "@angular/core";
import { RouterModule } from "@angular/router";
import { AppComponent } from "./app.component";
import { AppRouteGuard } from "@shared/auth/auth-route-guard";
import { HomeComponent } from "./home/home.component";
import { AboutComponent } from "./about/about.component";
import { UsersComponent } from "./users/users.component";
import { TenantsComponent } from "./tenants/tenants.component";
import { RolesComponent } from "app/roles/roles.component";
import { ChangePasswordComponent } from "./users/change-password/change-password.component";
import { ChartOfAccountComponent } from "modules/chart-of-account/chart-of-account.component";
import { ContactPersonTypeComponent } from "modules/contact-person-type/contact-person-type.component";
import { EthnicityComponent } from 'modules/ethnicity/ethnicity.component';
import { SalePersonComponent } from 'modules/sale-person/sale-person.component';
import { JobTitleComponent } from 'modules/job-title/job-title.component';
import { SpouseComponent } from 'modules/spouse/spouse.component';
import { ProductCategoryComponent } from 'modules/product-category/product-category.component';
import { CompanyComponent } from 'modules/company/company.component';
import { VendorComponent } from 'modules/vendor/vendor.component';
import { ViewCustomerComponent } from 'modules/customer/view-customer/view-customer.component';
import { CreateCustomerComponent } from 'modules/customer/create-customer/create-customer.component';
import { VenderTypeComponent } from 'modules/vender-type/vender-type.component';
import { ProductServiceComponent } from 'modules/product-service/product-service.component';
import { BankComponent } from 'modules/bank/bank.component';
import { CustomerTypeComponent } from 'modules/customer-type/customer-type.component';
import { CustomerDetailComponent } from 'modules/customer/view-customer/customer-detail/customer-detail.component';
import { CustomerDetailViewComponent } from 'modules/customer/customer-detail/customer-detail-view.component';

import { PaymentMethodComponent } from "modules/Payement-Method/Payment-Method.component";
import { ReceivedPaymentsComponent } from 'modules/all-sales/receieved-payments/received-payments.component';
import { SalesReceiptComponent } from 'modules/all-sales/SalesReceipt/Sales-Receipt.component';
import { MerchantComponent } from 'modules/Merchant/Merchant.component';
import { SalesTransactionComponent } from 'modules/all-sales/sales-transaction/sales-transation.component';
import { InvoicesDetailComponent } from 'modules/invoices-Detail/invoice-detail.component';
import { PurchaseReceiptComponent } from 'modules/all-sales/Purchase-Receipt/Purchase-Receipt.component';
import { PrintReceivedPaymentComponent } from 'modules/all-sales/print-received-payment/print-received-payment.component';
import { CreditCardComponent } from 'modules/all-sales/credit-card-modal/credit-card.component';
import { CreditNoteComponent } from 'modules/all-sales/Credit-Note/Credit-Note.component';
import { PrintCreditNoteComponent } from 'modules/all-sales/print-Credit-Note/print-credit-note.component';
import { PurchasePaymentComponent } from 'modules/all-expenses/Purchase-Payment/purchase-payment.component';
import { AddCheckComponent } from 'modules/all-expenses/Check/Add-Check.component';
import { CheckComponent } from 'modules/all-expenses/Check/Check.component';
import { PrintInvoicesComponent } from 'modules/all-sales/print-invoices/print-invoices.component';
import { JournalVoucherComponent } from 'modules/all-sales/journal-voucher/journal-voucher.component';
import { CheckSetupComponent } from 'modules/all-expenses/Check/Check-Setup.component';
import { UsersGroupComponent } from 'modules/Users-Group/Users-Group.component';

@NgModule({
  imports: [
    RouterModule.forChild([
      {
        path: "",
        component: AppComponent,
        children: [
          {
            path: "customers",
            loadChildren: () =>
              import("modules/customer/customer.module").then(
                (m) => m.CustomerModule
              ), //Lazy load admin module
            data: { preload: true },
            // canLoad: [AppRouteGuard],
          },
          //account settings
          {
            path: "account-settings",
            loadChildren: () =>
              import("modules/my-profile/my-profile.module").then(
                (m) => m.MyProfileModule
              ), //Lazy load invoice module
            data: { preload: true },
            // canLoad: [AppRouteGuard],
          },

          {
            path: "chart-of-accounts",
            loadChildren: () =>
              import("modules/chart-of-account/chart-of-account.module").then(
                (m) => m.ChartOfAccountModule
              ), //Lazy load admin module
            data: { preload: true },
            // canLoad: [AppRouteGuard],
          },
          {
            path: "invoices",
            loadChildren: () =>
              import("modules/invoices/invoice.module").then(
                (m) => m.InvoicesModule
              ), //Lazy load invoice module
            data: { preload: true, Permissions: "Pages.AllSales.Invoice" },
            // canLoad: [AppRouteGuard],
          },
          {
            path: "invoice/detail/:id",
            component: InvoicesDetailComponent,
            canActivate: [AppRouteGuard],
            data: { Permissions: "Pages.AllSales.Invoice" },
          },
          {
            path: "invoices/:id",
            loadChildren: () =>
              import("modules/invoices/invoice.module").then(
                (m) => m.InvoicesModule
              ), //Lazy load invoice module
            data: { preload: true, Permissions: "Pages.AllSales.Invoice" },
            // canLoad: [AppRouteGuard],
          },
          {
            path: "Recurringinvoices",
            loadChildren: () =>
              import("modules/Recurring-Invoice/Recurringinvoice.module").then(
                (m) => m.RecurringInvoicesModule
              ), //Lazy load invoice module
            data: {
              preload: true,
              Permissions: "Pages.AllSales.RecurringInvoice"
            },
            // canLoad: [AppRouteGuard],
          },
          {
            path: "Recurringinvoices/:id",
            loadChildren: () =>
              import("modules/Recurring-Invoice/Recurringinvoice.module").then(
                (m) => m.RecurringInvoicesModule
              ), //Lazy load invoice module
            data: {
              preload: true,
              Permissions: "Pages.AllSales.RecurringInvoice"
            },
            // canLoad: [AppRouteGuard],
          },
          // {
          //   path: "Purchaseinvoice",
          //   loadChildren: () =>
          //     import("modules/all-expenses/PurchaseInvoice.module").then(
          //       (m) => m.PurchaseInvoicesModule
          //     ), //Lazy load invoice module
          //   data: { preload: true },
          // },
          // {
          //   path: "Purchaseinvoice/:id",
          //   loadChildren: () =>
          //     import("modules/all-expenses/PurchaseInvoice.module").then(
          //       (m) => m.PurchaseInvoicesModule
          //     ), //Lazy load invoice module
          //   data: { preload: true },

          // },

          /* using Lazy Loading */
          {
            path: "all-sales",
            loadChildren: () =>
              import("modules/all-sales/all-sales.module").then(
                (m) => m.AllSalesModule),
            canActivate: [AppRouteGuard],
            data: { preload: true }//permission: "Pages.AllSales" }
          },
          {
            path: "all-expenses",
            loadChildren: () =>
              import("modules/all-expenses/PurchaseInvoice.module").then(
                (m) => m.PurchaseInvoicesModule),
            data: { preload: true },
          },

          // {
          //   path: "addCheck/:id",
          //   component: AddCheckComponent,
          //   canActivate: [AppRouteGuard],
          //   data: { permission: "Pages.AllExpense.Check" },
          // },
          // {
          //   path: "addCheck",
          //   component: AddCheckComponent,
          //   canActivate: [AppRouteGuard],
          //   data: { permission: "Pages.AllExpense.Check" },
          // },
          // {
          //   path: "check",
          //   component: CheckComponent,
          //   canActivate: [AppRouteGuard],
          //   data: { permission: "Pages.AllExpense.Check" },
          // },
          // {
          //   path: "checkSetup",
          //   component: CheckSetupComponent,
          //   canActivate: [AppRouteGuard],
          //   data: { permission: "Pages.AllExpense.Check" },
          // },
          {
            path: "Estimate",
            loadChildren: () =>
              import("modules/Estimate/Estimate.module").then(
                (m) => m.EstimateModule),
            data: { preload: true, Permissions: "Pages.AllSales.Estimate" },
          },
          {
            path: "Estimate/:id",
            loadChildren: () =>
              import("modules/Estimate/Estimate.module").then(
                (m) => m.EstimateModule),
            data: { preload: true, Permissions: "Pages.AllSales.Estimate" },
          },      
          {
            path: "reports",
            loadChildren: () =>
              import("modules/Reports/Reports.module").then(
                (m) => m.ReportModule),
            canActivate: [AppRouteGuard],
            data: { preload: true, Permissions: "Pages.AllSales.Estimate" }
          },
          // {
          //   path: "Reports/EmployeeActivities",
          //   loadChildren: () =>
          //     import("modules/Reports/Reports.module").then(
          //       (m) => m.ReportModule),
          //   data: { preload: true, Permissions: "Pages.AllSales.Estimate" },
          // },
          // {
          //   path: "Reports/:id",
          //   loadChildren: () =>
          //     import("modules/Reports/Reports.module").then(
          //       (m) => m.ReportModule),
          //   data: { preload: true, Permissions: "Pages.AllSales.Estimate" },
          // },


          // {
          //   path: "sales-transation",
          //   component: SalesTransactionComponent,
          //   canActivate: [AppRouteGuard],
          //   data: { Permissions: "Pages.AllSales.SalesTransaction" }
          // },
          // {
          //   path: "Purchasepayment",
          //   component: PurchasePaymentComponent,
          //   canActivate: [AppRouteGuard],
          //   data: { Permissions: "Pages.AllSales.PurchasePayment" }
          // },
          {
            path: "journal-voucher",
            component: JournalVoucherComponent,
            canActivate: [AppRouteGuard],
            data: { Permissions: "Pages.JournalVoucher" }
          },
          {
            path: "received-payment",
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
          // {
          //   path: "print-Credit-Note",
          //   component: PrintCreditNoteComponent,
          //   canActivate: [AppRouteGuard],
          //   data: { Permissions: "Pages.AllSales.CreditNote" }
          // },
          // {
          //   path: "Sales-Receipt",
          //   component: SalesReceiptComponent,
          //   canActivate: [AppRouteGuard],
          //   data: { Permissions: "Pages.AllSales.SalesReceipt" }
          // },
          // {
          //   path: "purchase-receipt",
          //   component: PurchaseReceiptComponent,
          //   canActivate: [AppRouteGuard],            
          //   data: { Permissions: "Pages.AllSales.PurchaseReceipt" }
          // },
          // {
          //   path: "credit-note",
          //   component: CreditNoteComponent,
          //   canActivate: [AppRouteGuard],
          //   data: { Permissions: "Pages.AllSales.CreditNote" }
          // },
          // {
          //   path: "credit-note/:id",
          //   component: CreditNoteComponent,
          //   canActivate: [AppRouteGuard],
          //   data: { Permissions: "Pages.AllSales.CreditNote" }
          // },
          {
            path: "users",
            component: UsersComponent,
            data: { permission: "Pages.Users" },
            canActivate: [AppRouteGuard],
          },
          {
            path: "home",
            component: HomeComponent,
            canActivate: [AppRouteGuard],
          },
          {
            path: "customer-type",
            component: CustomerTypeComponent,
            canActivate: [AppRouteGuard],
          },
          {
            path: "bank",
            component: BankComponent,
            canActivate: [AppRouteGuard],
          },
          {
            path: "vendor",
            component: VendorComponent,
            data: { permission: "Pages.Users" },
            canActivate: [AppRouteGuard],
          },
          {
            path: "vender-type",
            component: VenderTypeComponent,
            data: { permission: "Pages.Users" },
            canActivate: [AppRouteGuard],
          },
          {
            path: "contact-person-type",
            component: ContactPersonTypeComponent,
            data: { permission: "Pages.Users" },
            canActivate: [AppRouteGuard],
          },

          {
            path: "product-service",
            component: ProductServiceComponent,
            data: { permission: "Pages.Users" },
            canActivate: [AppRouteGuard],
          },

          {
            path: "product-service1",
            component: ProductServiceComponent,
            data: { permission: "Pages.Users" },
            canActivate: [AppRouteGuard],
          },
          {
            path: "contact-person",
            component: ContactPersonTypeComponent,
            data: { permission: "Pages.Users" },
            canActivate: [AppRouteGuard],
          },
          {
            path: "job-title",
            component: JobTitleComponent,
            data: { permission: "Pages.Users" },
            canActivate: [AppRouteGuard],
          },
          {
            path: "company",
            component: CompanyComponent,
            data: { permission: "Pages.Users" },
            canActivate: [AppRouteGuard],
          },

          {
            path: "product-category",
            component: ProductCategoryComponent,
            data: { permission: "Pages.Users" },
            canActivate: [AppRouteGuard],
          },
          {
            path: "source-referal",
            component: SourceReferalComponent,
            data: { permission: "Pages.Users" },
            canActivate: [AppRouteGuard],
          },
          {
            path: "spouse",
            component: SpouseComponent,
            data: { permission: "Pages.Users" },
            canActivate: [AppRouteGuard],
          },
          {
            path: "ethnicity",
            component: EthnicityComponent,
            data: { permission: "Pages.Users" },
            canActivate: [AppRouteGuard],
          },
          {
            path: "entity-type",
            component: EntityTypeComponent,
            data: { permission: "Pages.Users" },
            canActivate: [AppRouteGuard],
          },
          {
            path: "payment-method",
            component: PaymentMethodComponent,
            data: { permission: "Pages.Users" },
            canActivate: [AppRouteGuard],
          },
          {
            path: "user-groups",
            component: UsersGroupComponent,
            data: { permission: "Pages.Users" },
            canActivate: [AppRouteGuard],
          },
          {
            path: "Merchnat",
            component: MerchantComponent,
            data: { permission: "Pages.Users" },
            canActivate: [AppRouteGuard],
          },
          {
            path: "language",
            component: languageComponent,
            data: { permission: "Pages.Users" },
            canActivate: [AppRouteGuard],
          },
          {
            path: "sale-person",
            component: SalePersonComponent,
            data: { permission: "Pages.Users" },
            canActivate: [AppRouteGuard],
          },
          {
            path: "roles",
            component: RolesComponent,
            data: { permission: "Pages.Roles" },
            canActivate: [AppRouteGuard],
          },
          {
            path: "tenants",
            component: TenantsComponent,
            data: { permission: "Pages.Tenants" },
            canActivate: [AppRouteGuard],
          },
          {
            path: "about",
            component: AboutComponent,
            canActivate: [AppRouteGuard],
          },
          {
            path: "update-password",
            component: ChangePasswordComponent,
            canActivate: [AppRouteGuard],
          },
          {
            path: 'customer-detail/:id',
            component: ViewCustomerComponent,
          }
          ,
          {
            path: 'create-customer/:id',
            component: CreateCustomerComponent,
          },
          {
            path: 'create-customer',
            component: CreateCustomerComponent,
          },
          {
            path: 'customer-detail-view/:id',
            component: CustomerDetailViewComponent
          }
        ],
      },
    ]),
  ],
  exports: [RouterModule],
})
export class AppRoutingModule { }
