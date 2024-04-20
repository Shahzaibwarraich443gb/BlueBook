import { mat_component_modules } from "./../mat_component_modules/mat_component_modules";
import { LanguagedropdownComponent } from './layout/languagedropdown/languagedropdown.component';
import { QuickdropdownComponent } from './layout/quickdropdown/quickdropdown.component';
import { EmaildropdownComponent } from './layout/emaildropdown/emaildropdown.component';
import { NotifydropdownComponent } from './layout/notifydropdown/notifydropdown.component';
import { LogouticonComponent } from './layout/logouticon/logouticon.component';
import { LanguageiconComponent } from './layout/languageicon/languageicon.component';
import { AppearenceiconComponent } from './layout/appearenceicon/appearenceicon.component';
import { EditprofileiconComponent } from './layout/editprofileicon/editprofileicon.component';
import { ToolsiconComponent } from './layout/toolsicon/toolsicon.component';
import { ReporticonComponent } from './layout/reporticon/reporticon.component';
import { PeopleiconComponent } from './layout/peopleicon/peopleicon.component';

import { SmsiconComponent } from './layout/smsicon/smsicon.component';
import { BilliconComponent } from './layout/billicon/billicon.component';
import { LedgericonComponent } from './layout/ledgericon/ledgericon.component';
import { VouchericonComponent } from './layout/vouchericon/vouchericon.component';
import { PaymenticonComponent } from './layout/paymenticon/paymenticon.component';
import { BankiconComponent } from './layout/bankicon/bankicon.component';
import { DashboardiconComponent } from './layout/dashboardicon/dashboardicon.component';
import { MenuComponent } from './layout/menu/menu.component';
import { SettingiconComponent } from './layout/settingicon/settingicon.component';
import { EmailiconComponent } from './layout/emailicon/emailicon.component';
import { PiechartComponent } from './layout/piechart/piechart.component';
import { RecentclientsComponent } from './layout/recentclients/recentclients.component';
import { RecentvoucherComponent } from './layout/recentvoucher/recentvoucher.component';
import { SplinechartComponent } from './layout/splinechart/splinechart.component';
import { NotifyiconComponent } from './layout/notifyicon/notifyicon.component';
import { QuickviewComponent } from './layout/quickview/quickview.component';
import { CopyrightComponent } from './layout/copyright/copyright.component';
import { BariconComponent } from './layout/baricon/baricon.component';
import { BarchartComponent } from './layout/barchart/barchart.component';
import { BalancealertComponent } from './layout/balancealert/balancealert.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientJsonpModule } from '@angular/common/http';
import { HttpClientModule } from '@angular/common/http';
import { ModalModule } from 'ngx-bootstrap/modal';
// import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { CollapseModule } from "ngx-bootstrap/collapse";
import { TabsModule } from "ngx-bootstrap/tabs";
import { NgxPaginationModule } from "ngx-pagination";
import { AppRoutingModule } from "./app-routing.module";
import { AppComponent } from "./app.component";
import { ServiceProxyModule } from "@shared/service-proxies/service-proxy.module";
import { SharedModule } from "@shared/shared.module";
import { HomeComponent } from "@app/home/home.component";
import { AboutComponent } from "@app/about/about.component";
// tenants
import { TenantsComponent } from "@app/tenants/tenants.component";
import { CreateTenantDialogComponent } from "./tenants/create-tenant/create-tenant-dialog.component";
import { EditTenantDialogComponent } from "./tenants/edit-tenant/edit-tenant-dialog.component";
// roles
import { RolesComponent } from "@app/roles/roles.component";
import { PermissionTreeComponent } from '@app/roles/permission-tree.component';
import { CreateRoleDialogComponent } from "./roles/create-role/create-role-dialog.component";
import { EditRoleDialogComponent } from "./roles/edit-role/edit-role-dialog.component";
// users
import { UsersComponent } from "@app/users/users.component";
import { CreateUserDialogComponent } from "@app/users/create-user/create-user-dialog.component";
import { EditUserDialogComponent } from "@app/users/edit-user/edit-user-dialog.component";
import { ChangePasswordComponent } from "./users/change-password/change-password.component";
import { ResetPasswordDialogComponent } from "./users/reset-password/reset-password.component";
// layout
import { HeaderComponent } from "./layout/header.component";
import { HeaderLeftNavbarComponent } from "./layout/header-left-navbar.component";
import { HeaderLanguageMenuComponent } from "./layout/header-language-menu.component";
import { HeaderUserMenuComponent } from "./layout/header-user-menu.component";
import { FooterComponent } from "./layout/footer.component";
import { SidebarComponent } from "./layout/sidebar.component";
import { SidebarLogoComponent } from "./layout/sidebar-logo.component";
import { SidebarUserPanelComponent } from "./layout/sidebar-user-panel.component";
import { SidebarMenuComponent } from "./layout/sidebar-menu.component";

import { SwiperModule } from "swiper/angular";
import { MetismenuAngularModule } from "@metismenu/angular";
import { ClipboardModule } from 'ngx-clipboard';
import { NgApexchartsModule } from 'ng-apexcharts';
import { RouterModule } from '@angular/router';
import { NgxSliderModule } from '@angular-slider/ngx-slider';
import { ChartOfAccountComponent } from 'modules/chart-of-account/chart-of-account.component';
import { MatFormFieldModule } from '@angular/material/form-field';
import { CreateChartOfAccountComponent } from 'modules/chart-of-account/create-chart-of-account/create-chart-of-account.component';
import { CreateAccountTypeComponent } from 'modules/chart-of-account/create-account-type/create-account-type.component';
import { CreateMainHeadComponent } from 'modules/chart-of-account/create-main-head/create-main-head.component';
import { ThemeModules } from './../mat_component_modules/theme.modules';
import { ContactPersonTypeComponent } from "modules/contact-person-type/contact-person-type.component";
import { CreateContactTypePersonComponent } from "modules/contact-person-type/create-contact-person-type/create-contact-person-type.component";
import { languageComponent } from "modules/Language/language.component";
import { CreatelanguageComponent } from "modules/Language/create-language/create-language.component";
import { EthnicityComponent } from "modules/ethnicity/ethnicity.component";
import { SalePersonComponent } from "modules/sale-person/sale-person.component";
import { CreateSalePersonComponent } from "modules/sale-person/create-sale-person/create-sale-person.component";
import { CreateEthnicityComponent } from "modules/ethnicity/create-ethnicity/create-ethnicity.component";
import { CreateSourecReferalComponent } from "modules/source-referal/create-source-referal/create-source-referal.component";
import { SourceReferalComponent } from "modules/source-referal/source-referal.component";
import { EntityTypeComponent } from "modules/entity-type/entity-type.component";
import { CreateEntityTypeComponent } from "modules/entity-type/create-entity-type/create-entity-type.component";
import { JobTitleComponent } from "modules/job-title/job-title.component";
import { InvoicesDetailComponent } from "modules/invoices-Detail/invoice-detail.component";
import { CreateJoTitleComponent } from "modules/job-title/create-job-title/create-job-title.component";
import { DropdownModule } from 'primeng/dropdown';
import { CreateSpouseComponent } from "modules/spouse/create-spouse/create-spouse.component";
import { SpouseComponent } from "modules/spouse/spouse.component";
import { CreateProductCategoryComponent } from "modules/product-category/create-product-category/create-product-category.component";
import { ProductCategoryComponent } from "modules/product-category/product-category.component";
import { CreateCompanyComponent } from "modules/company/create-company/create-company.component";
import { CompanyComponent } from "modules/company/company.component";
import { VendorComponent } from "modules/vendor/vendor.component";
import { VendorAttachments } from "modules/vendor/vendor-attachment/vendor-attachment.component";
import { EmailAttachmentComponent } from "modules/vendor/email-attachment-modal/email-attachment.component";
import { VenderTypeComponent } from "modules/vender-type/vender-type.component"
import { CreateOrEditVendorComponent } from "modules/vendor/create-or-edit-vendor/create-or-edit-vendor.component";
import { CreateorEditVenderTypeComponent } from "modules/vender-type/create-or-edit-vender-type/create-or-edit-vender-type.component";
import { ProductServiceComponent } from "modules/product-service/product-service.component";
import { CreateOrEditProductServiceComponent } from "modules/product-service/create-or-edit-product-service/create-or-edit-product-service.component"
import { TextMaskModule } from 'angular2-text-mask';
import { BankComponent } from "modules/bank/bank.component";
import { CreateOrEditBankComponent } from "modules/bank/create-or-edit-bank/create-or-edit-bank.component"
import { CustomerTypeComponent } from "modules/customer-type/customer-type.component";
import { CreateorEditCustomerTypeComponent } from "modules/customer-type/create-or-edit-customer-type/create-or-edit-customer-type.component";
import { CostPriceOrSalePriceHistoryComponent } from "modules/product-service/cost-price-or-sale-price-history/cost-price-or-sale-price-history.component";
import { PaymentMethodComponent } from "modules/Payement-Method/Payment-Method.component";
import { CreatePaymentMethodComponent } from "modules/Payement-Method/create-Payment-Method/create-Payement-Method.component"
import { UsersGroupComponent } from "modules/Users-Group/Users-Group.component"
import { CreateUsersGroupComponent } from "modules/Users-Group/create-Users-Group/create-Users-Group.component";
import { MerchantComponent } from "modules/Merchant/Merchant.component"
import { CreateMerchantComponent } from "modules/Merchant/create-Merchant/create-Merchant.component";
import { LedgerComponent } from "modules/ledger/ledger.component"
import { NgxSpinnerModule } from "ngx-spinner";
// ng prime 
import { TreeModule } from "primeng/tree"

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    // CustomerType
    CustomerTypeComponent,
    CreateorEditCustomerTypeComponent,
    //Bank
    BankComponent,
    CreateOrEditBankComponent,
    // product services
    CostPriceOrSalePriceHistoryComponent,
    ProductServiceComponent,
    CreateOrEditProductServiceComponent,
    //Vender Type
    VenderTypeComponent,
    CreateorEditVenderTypeComponent,
    AboutComponent,
    //Vendor 
    VendorComponent,
    CreateOrEditVendorComponent,
    VendorAttachments,
    EmailAttachmentComponent,
    // ProductCategory
    CreateProductCategoryComponent,
    ProductCategoryComponent,
    //Company Type 
    CreateCompanyComponent,
    CompanyComponent,
    //JobTitle
    JobTitleComponent,
    InvoicesDetailComponent,
    CreateJoTitleComponent,
    // EntityType
    EntityTypeComponent,
    CreateEntityTypeComponent,
    // PaymentMethod
    PaymentMethodComponent,
    CreatePaymentMethodComponent,

    //Users-Group
    UsersGroupComponent,
    CreateUsersGroupComponent,
    // Merchant
    MerchantComponent,
    CreateMerchantComponent,

    // SourceReferalComponent
    SourceReferalComponent,
    CreateSourecReferalComponent,
    // Sale Person
    SalePersonComponent,
    CreateSalePersonComponent,
    //spuse
    CreateSpouseComponent,
    SpouseComponent,
    //language
    languageComponent,
    CreatelanguageComponent,
    // tenants
    TenantsComponent,
    CreateTenantDialogComponent,
    EditTenantDialogComponent,
    // roles
    RolesComponent,
    PermissionTreeComponent,
    CreateRoleDialogComponent,
    EditRoleDialogComponent,
    // users
    UsersComponent,
    CreateContactTypePersonComponent,
    ContactPersonTypeComponent,
    CreateUserDialogComponent,
    EditUserDialogComponent,
    ChangePasswordComponent,
    ResetPasswordDialogComponent,
    // layout
    HeaderComponent,
    HeaderLeftNavbarComponent,
    HeaderLanguageMenuComponent,
    HeaderUserMenuComponent,
    FooterComponent,
    SidebarComponent,
    SidebarLogoComponent,
    SidebarUserPanelComponent,
    SidebarMenuComponent,
    QuickdropdownComponent,
    BalancealertComponent,
    BarchartComponent,
    BariconComponent,
    CopyrightComponent,
    QuickdropdownComponent,
    LanguagedropdownComponent,
    QuickdropdownComponent,
    QuickviewComponent,
    NotifydropdownComponent,
    NotifyiconComponent,
    EmaildropdownComponent,
    SplinechartComponent,
    RecentvoucherComponent,
    RecentclientsComponent,
    PiechartComponent,
    EmailiconComponent,
    SettingiconComponent,
    MenuComponent,
    DashboardiconComponent,
    BankiconComponent,
    PaymenticonComponent,
    VouchericonComponent,
    LedgericonComponent,
    BilliconComponent,
    SmsiconComponent,
    PeopleiconComponent,
    ReporticonComponent,
    ToolsiconComponent,
    EditprofileiconComponent,
    AppearenceiconComponent,
    LanguageiconComponent,
    LogouticonComponent,
    //Ethnicity
    EthnicityComponent,
    CreateEthnicityComponent,
    LedgerComponent
  ],
  imports: [
    MatFormFieldModule,
    TreeModule,
    SwiperModule,
    RouterModule,
    NgxSliderModule,
    FormsModule,
    MetismenuAngularModule,
    ClipboardModule,
    ReactiveFormsModule,
    NgApexchartsModule,
    CommonModule,
    HttpClientModule,
    HttpClientJsonpModule,
    ModalModule.forChild(),
    // BsDropdownModule,
    CollapseModule,
    TabsModule,
    AppRoutingModule,
    ServiceProxyModule,
    SharedModule,
    NgxPaginationModule,
    NgxSpinnerModule,
    ThemeModules,
    DropdownModule,
    TextMaskModule,
  ],
  providers: [],
})
export class AppModule { }
