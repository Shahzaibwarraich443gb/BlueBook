import { CreateCustomerComponent } from './create-customer/create-customer.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CustomerComponent } from './customer.component';
import { ThemeModules } from './../../mat_component_modules/theme.modules';
import { MenuComponent } from './../../app/layout/menu/menu.component';
import { CustomerRoutingModule } from './customer-routing.module';
import { CreateOrEditCustomerComponent } from './create-or-edit-customer/create-or-edit-customer.component';
import { AttachmentComponent } from './view-customer/attachment/attachment.component';
import { DairyComponent } from './view-customer/dairy/dairy.component';
import { CustomerDetailComponent } from './view-customer/customer-detail/customer-detail.component';
import { CustomerTodoListComponent } from './create-customer/customer-todo-list/customer-todo-list.component';
import { CrmComponent } from './view-customer/crm/crm.component';
import { ViewCustomerComponent } from 'modules/customer/view-customer/view-customer.component';
import { CustomerDocumentComponent } from './view-customer/customer-document/customer-document.component';
import { LicensesComponent } from './view-customer/licenses/licenses.component';
import { ServiceListViewComponent } from './view-customer/service-list-view/service-list-view.component';
import { CustomerInfoComponent } from './customer-info/customer-info.component';
import { CustomerLicenseComponent } from './customer-license/customer-license.component';
import {CustomerPasswordsComponent} from './create-customer/customer-passwords/customer-passwords.component';
import { DetailCustomerComponent } from './create-customer/detail-customer/detail-customer.component';
import { ContactInfoComponent } from './create-customer/contact-info/contact-info.component';
import { CustomerCRMComponent } from './create-customer/customer-crm/customer-crm.component';
import { AddressComponent } from './create-customer/address/address.component';
import { FillingDetailComponent } from './create-customer/filling-detail/filling-detail.component';
import { UserInfoComponent } from './create-customer/user-info/user-info.component';
import {CustomerAttachments} from './create-customer/customer-attachment/customer-attachment.component';
import { CustomerDetailViewComponent } from './customer-detail/customer-detail-view.component';
import { CustomerInfoViewComponent } from './customer-detail/customer-info-view/customer-info-view.component';
import { CustomerDtlViewComponent } from './customer-detail/customer-dtl-view/customer-dtl-view.component'; 
import { CustomerContactInfoViewComponent } from '../customer/customer-detail/customer-contact-info-view/customer-contact-info-view.component'; 
import { CustomerLicenseViewComponent } from './customer-detail/customer-license-view/customer-license-view.component';
import { CustomerPasswordsViewComponent } from './customer-detail/customer-passwords-view/customer-passwords-view.component';
import {CustomerAddressViewComponent} from './customer-detail/customer-address-view/customer-address-view.component'
import {CustomerTodoListViewComponent} from './customer-detail/customer-todo-list-view/customer-todo-list-view.component';
import {CustomerAttachmentsView} from './customer-detail/customer-attachments-view/customer-attachment-view.component';
import {CustomerCRMViewComponent} from './customer-detail/customer-crm-view/customer-crm-view.component';
import {CustomerDiaryViewComponent} from './customer-detail/customer-diary-view/customer-diary-view.component'
import {CustomerDiaryComponent} from './create-customer/customer-diary/customer-diary.component';
import {CustomerEmailAttachmentComponent} from './create-customer/customer-email-attachment/customer-email-attachment.component';
import {SalesTaxComponent} from './customer-services/sales-tax/sales-tax.component';
import {PersonalTaxComponent} from './customer-services/personal-tax/personal-tax.component';
import {CorporateTaxComponent} from './customer-services/corporate-tax/corporate-tax.component';
import { SharedModule } from '@shared/shared.module';
import { NgxSpinnerModule } from 'ngx-spinner';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import {MatDividerModule} from '@angular/material/divider';
import { MatDialogModule, MAT_DIALOG_DATA, MatDialogRef, MAT_DIALOG_DEFAULT_OPTIONS } from '@angular/material/dialog';
@NgModule({
  declarations: [
    ServiceListViewComponent,
    CustomerDocumentComponent,
    LicensesComponent,
    CrmComponent,
    DairyComponent,
    CustomerTodoListComponent,
    CustomerDetailComponent,
    AttachmentComponent,
    ViewCustomerComponent,
    CustomerComponent,
    CreateOrEditCustomerComponent,
    CreateCustomerComponent,
    CustomerInfoComponent,
    CustomerLicenseComponent,
    DetailCustomerComponent,
    ContactInfoComponent,
    AddressComponent,
    FillingDetailComponent,
    CustomerPasswordsComponent,
    UserInfoComponent,
    CustomerDetailViewComponent,
    CustomerInfoViewComponent,
    CustomerDetailViewComponent,
    CustomerDtlViewComponent,
    CustomerLicenseViewComponent,
    CustomerPasswordsViewComponent,
    CustomerContactInfoViewComponent,
    CustomerAddressViewComponent,
    CustomerTodoListViewComponent,
    CustomerCRMComponent,
    CustomerCRMViewComponent,
    CustomerAttachments,
    CustomerAttachmentsView,
    CustomerDiaryComponent,
    CustomerEmailAttachmentComponent,
    CustomerDiaryViewComponent,
    SalesTaxComponent,
    PersonalTaxComponent,
    CorporateTaxComponent
  ],
  imports: [
    CommonModule,
    ThemeModules,
    CustomerRoutingModule,
    MatTableModule,
    MatSortModule,
    MatDividerModule,
    MatPaginatorModule,
    SharedModule,
    MatDialogModule
  ]
})
export class CustomerModule { }
