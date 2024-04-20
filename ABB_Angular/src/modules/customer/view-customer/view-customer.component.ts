import { Component, EventEmitter, Inject, OnInit, Output } from '@angular/core';
import { MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ActivatedRoute } from '@angular/router';
import { CreateOrEditCustomerDto, CreateOrEditEmailInputDto, CreateOrEditPhoneDto, LanguageDto, EthnicityDto, ContactPersonTypeDto, SourceReferralTypeDto, SalesPersonTypeDto, GeneralEntityTypeDto, LanguageServiceProxy, CustomerServiceProxy, ContactPersonTypeServiceProxy, GeneralEntityTypeServiceProxy, EthnicitiesServiceProxy, SourceReferralTypeServiceProxy, SalesPersonTypeServiceProxy, CreateOrEditAddressDto, PhoneDto, EmailDto } from '@shared/service-proxies/service-proxies';
import { CreateContactTypePersonComponent } from 'modules/contact-person-type/create-contact-person-type/create-contact-person-type.component';
import { CreateEntityTypeComponent } from 'modules/entity-type/create-entity-type/create-entity-type.component';
import { CreateEthnicityComponent } from 'modules/ethnicity/create-ethnicity/create-ethnicity.component';
import { CreatelanguageComponent } from 'modules/Language/create-language/create-language.component';
import { CreateSalePersonComponent } from 'modules/sale-person/create-sale-person/create-sale-person.component';
import { CreateSourecReferalComponent } from 'modules/source-referal/create-source-referal/create-source-referal.component';
import { finalize } from 'rxjs';
import { DialogData } from '../create-or-edit-customer/create-or-edit-customer.component';

@Component({
  selector: 'app-view-customer',
  templateUrl: './view-customer.component.html',
  styleUrls: ['./view-customer.component.scss']
})
export class ViewCustomerComponent implements OnInit {
  customer = new CreateOrEditCustomerDto();
  email = new CreateOrEditEmailInputDto();
  phone = new CreateOrEditPhoneDto();
  address = new CreateOrEditAddressDto();

  languages: LanguageDto[] = [];
  ethnicities: EthnicityDto[] = [];
  contactPersonTypes: ContactPersonTypeDto[] = [];
  sourceReferralTypes: SourceReferralTypeDto[] = [];
  salesPersonTypes: SalesPersonTypeDto[] = [];
  generalEntityTypes: GeneralEntityTypeDto[] = [];
  emails: CreateOrEditEmailInputDto[] = [];
  phones: CreateOrEditPhoneDto[] = [];
  addresses: CreateOrEditAddressDto[] = [];

  constructor(
    public _dialog: MatDialog,
    private _activatedRoute: ActivatedRoute,
    public _languageServiceProxy: LanguageServiceProxy,
    public _customerServiceProxy: CustomerServiceProxy,
    public _contactPersonTypeServiceProxy: ContactPersonTypeServiceProxy,
    public _generalEntityTypeServiceProxy: GeneralEntityTypeServiceProxy,
    public _ethnicitiesServiceProxy: EthnicitiesServiceProxy,
    public _sourceReferralTypeServiceProxy: SourceReferralTypeServiceProxy,
    public _salesPersonTypeServiceProxy: SalesPersonTypeServiceProxy
  ) { }

  ngOnInit() {
    this._activatedRoute.params.subscribe(parms => {
      if (parms.id) {
        this.getRelevantData();
        if (parms.id) {
          this._customerServiceProxy
            .getCustomerForEdit(parms.id)
            .pipe(finalize(() => { }))
            .subscribe((result) => {
              this.customer = result;
              if (result.addresses.length > 0) {
                this.addresses = result.addresses;
              }
              if (result.emails.length > 0) {
                this.emails = result.emails;
              }
              if (result.phoneNumbers.length > 0) {
                this.phones = result.phoneNumbers;
              }
            });

        } else {
          this.customer = new CreateOrEditCustomerDto();
          this.emails = [];
          this.phones = [];
          this.addresses = [];
          this.email.typeEmail = 0;
          this.phone.type = 0;
          this.address.type = 0;
        }
      }
    });

  }

  getRelevantData() {
    this.getGeneralEntityTypes();
    this.getLanguages();
    this.getSourceReferralTypes();
    this.getEthnicities();
    this.getContactPersonTypes();
    this.getSalesPersonTypes();
    this.getGeneralEntityTypes();
  }

  save() {
    this.customer.addresses = this.addresses;
    this.customer.phoneNumbers = this.phones;
    this.customer.emails = this.emails;
    this._customerServiceProxy.createOrEdit(this.customer).pipe(
      finalize(() => {
      })
    )
      .subscribe(() => {
      });
  }

  addAddress() {
    var index = this.addresses.findIndex(x => x == this.address);
    if (index !== -1) this.addresses[index] = this.address;
    else this.addresses.push(this.address);
    this.address = new CreateOrEditAddressDto();
    this.address.type = 0;
  }

  updateAddress(address: CreateOrEditAddressDto) {
    var index = this.addresses.findIndex(x => x == address);
    if (index !== -1) this.address = this.addresses[index];
  }

  deleteAddress(address: CreateOrEditAddressDto) {
    var index = this.addresses.findIndex(x => x == address);
    if (index !== -1) this.addresses.splice(index, 1);
  }

  addPhoneNumber() {
    if (this.phone.number != null) {
      var index = this.phones.findIndex(x => x == this.phone);
      if (index !== -1) this.phones[index] = this.phone;
      else this.phones.push(this.phone);
      this.phone = new PhoneDto();
      this.phone.type = 0;
    }
  }

  updatePhoneNumber(phone: PhoneDto) {
    var index = this.phones.findIndex(x => x == phone);
    if (index !== -1) this.phone = this.phones[index];
  }

  deletePhoneNumber(phone: PhoneDto) {
    var index = this.phones.findIndex(x => x == phone);
    if (index !== -1) this.phones.splice(index, 1);
  }

  addEmail() {
    if (this.email.emailAddress != null) {
      var index = this.emails.findIndex(x => x == this.email);
      if (index !== -1) this.emails[index] = this.email;
      else this.emails.push(this.email);
      this.email = new EmailDto();
      this.email.typeEmail = 0;
    }
  }

  updateEmail(email: EmailDto) {
    var index = this.emails.findIndex(x => x == email);
    if (index !== -1) this.email = this.emails[index];
  }

  deleteEmail(email: EmailDto) {
    var index = this.emails.findIndex(x => x == email);
    if (index !== -1) this.emails.splice(index, 1);
  }

  protected getGeneralEntityTypes() {
    this._generalEntityTypeServiceProxy
      .getAll()
      .subscribe((arg) => (this.generalEntityTypes = arg));
  }

  public openCreateSalesPersonTypeDialog(id?: number): void {
    const dialogRef = this._dialog.open(CreateSalePersonComponent, {
      data: { id: id },
    });

    dialogRef.afterClosed().subscribe((result) => {
      dialogRef.close();
      this.getSalesPersonTypes();
    });
  }

  protected getSalesPersonTypes() {
    this._salesPersonTypeServiceProxy
      .getAll()
      .subscribe((arg) => (this.salesPersonTypes = arg));
  }

  public openCreateGeneralEntityTypesDialog(id?: number): void {
    const dialogRef = this._dialog.open(CreateEntityTypeComponent, {
      data: { id: id },
    });

    dialogRef.afterClosed().subscribe((result) => {
      dialogRef.close();
      this.getGeneralEntityTypes();
    });
  }

  protected getSourceReferralTypes() {
    this._sourceReferralTypeServiceProxy
      .getAll()
      .subscribe((arg) => (this.sourceReferralTypes = arg));
  }

  public openCreateSourceReferralTypeDialog(id?: number): void {
    const dialogRef = this._dialog.open(CreateSourecReferalComponent, {
      data: { id: id },
    });

    dialogRef.afterClosed().subscribe((result) => {
      dialogRef.close();
      this.getSourceReferralTypes();
    });
  }

  protected getContactPersonTypes() {
    this._contactPersonTypeServiceProxy
      .getAll()
      .subscribe((arg) => (this.contactPersonTypes = arg));
  }

  public openCreateContactPersonTypeDialog(id?: number): void {
    const dialogRef = this._dialog.open(CreateContactTypePersonComponent, {
      data: { id: id },
    });

    dialogRef.afterClosed().subscribe((result) => {
      dialogRef.close();
      this.getContactPersonTypes();
    });
  }

  protected getEthnicities() {
    this._ethnicitiesServiceProxy
      .getAll()
      .subscribe((arg) => (this.ethnicities = arg));
  }

  public openCreateEthnicityDialog(id?: number): void {
    const dialogRef = this._dialog.open(CreateEthnicityComponent, {
      data: { id: id },
    });

    dialogRef.afterClosed().subscribe((result) => {
      dialogRef.close();
      this.getEthnicities();
    });
  }

  protected getLanguages() {
    this._languageServiceProxy
      .getAll()
      .subscribe((arg) => (this.languages = arg));
  }

  public openCreateLanguageDialog(id?: number): void {
    const dialogRef = this._dialog.open(CreatelanguageComponent, {
      data: { id: id },
    });

    dialogRef.afterClosed().subscribe((result) => {
      dialogRef.close();
      this.getLanguages();
    });
  }
}
