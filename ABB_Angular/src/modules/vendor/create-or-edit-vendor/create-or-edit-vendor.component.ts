import { Component, EventEmitter, Inject, Injector, OnInit, Output } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { CreatelanguageComponent } from 'modules/Language/create-language/create-language.component';
import { CreateOrEditCustomerDto, EthnicitiesServiceProxy, EthnicityDto, LanguageDto, LanguageServiceProxy, ContactPersonTypeServiceProxy, ContactPersonTypeDto, SourceReferralTypeDto, SalesPersonTypeDto, SalesPersonTypeServiceProxy, GeneralEntityTypeServiceProxy, GeneralEntityTypeDto, EmailDto, PhoneDto, AddressDto, CustomerServiceProxy, SourceReferralTypeServiceProxy, CreateOrEditEmailInputDto, CreateOrEditAddressDto, CreateOrEditPhoneDto, VenderServiceProxy, VenderTypeServiceProxy, VenderTypeDto, CreateOrEditVenderDto, VendorContactInfoServiceProxy, CreateOrEditVendorContactInfoDto, ContactPersonType, ContactTypeEnum, CreateOrEditVendorAddressDto } from './../../../shared/service-proxies/service-proxies';
import { CreateEthnicityComponent } from 'modules/ethnicity/create-ethnicity/create-ethnicity.component';
import { CreateContactTypePersonComponent } from 'modules/contact-person-type/create-contact-person-type/create-contact-person-type.component';
import { CreateSalePersonComponent } from 'modules/sale-person/create-sale-person/create-sale-person.component';
import { CreateEntityTypeComponent } from 'modules/entity-type/create-entity-type/create-entity-type.component';
import { finalize } from 'rxjs';
import { CreateSourecReferalComponent } from 'modules/source-referal/create-source-referal/create-source-referal.component';
import { AppConsts } from '@shared/AppConsts';
import { AppComponentBase } from '@shared/app-component-base';
import { NgxSpinnerService } from 'ngx-spinner';
import * as moment from 'moment';

export interface DialogData {
  id: number;
}

@Component({
  selector: 'app-create-or-edit-vendor',
  templateUrl: './create-or-edit-vendor.component.html',
  styleUrls: ['./create-or-edit-vendor.component.scss']
})

export class CreateOrEditVendorComponent extends AppComponentBase implements OnInit {
  @Output() onSave = new EventEmitter<any>();

  selectedTab = "INFO"
  selectedTabAddress: "Address"
  customer = new CreateOrEditCustomerDto();
  email = new CreateOrEditEmailInputDto();
  phone = new CreateOrEditPhoneDto();
  address = new CreateOrEditAddressDto();
  mask = {
    guide: true,
    showMask: true,
    mask: [/\d/, /\d/, '-', /\d/, /\d/, '-', /\d/, /\d/, /\d/, /\d/]
  };

  // waheed Dto
  contactPersonTypeDto: ContactPersonTypeDto[] = [];

  lastTab: boolean = false;
  vendor = new CreateOrEditVenderDto();
  venderTypeDto: VenderTypeDto[] = [];
  createOrEditVendorContactInfoDto = new CreateOrEditVendorContactInfoDto();
  vendorAddressDto = new CreateOrEditVendorAddressDto();

  // End Dto
  ethnicities: EthnicityDto[] = [];
  contactPersonTypes: ContactPersonTypeDto[] = [];
  sourceReferralTypes: SourceReferralTypeDto[] = [];
  salesPersonTypes: SalesPersonTypeDto[] = [];
  generalEntityTypes: GeneralEntityTypeDto[] = [];
  emails: CreateOrEditEmailInputDto[] = [];
  phones: CreateOrEditPhoneDto[] = [];
  addresses: CreateOrEditAddressDto[] = [];
  selectedIndex: number = 0;
  vendorList: any;
  vendorId: number;
  AltDateOfBirth: Date;
  constructor(
    private injector: Injector,
    public _dialog: MatDialog,
    public dialogRef: MatDialogRef<any>,
    public _vendorServiceProxy: VenderServiceProxy,
    public _venderTypeServiceProxy: VenderTypeServiceProxy,
    public _vendorContactInfoServiceProxy: VendorContactInfoServiceProxy,
    public _contactPersonTypeServiceProxy: ContactPersonTypeServiceProxy,
    private spinner: NgxSpinnerService,
    @Inject(MAT_DIALOG_DATA) public data: DialogData) {
    super(injector);
  }

  ngOnInit() {
    this.getRelevantData();
    if (this.data.id) {
      this.spinner.show();
      this.vendorId = this.data.id;
      // get vendor 
      this._vendorServiceProxy
        .getVendor(this.data.id)
        .pipe(finalize(() => { }))
        .subscribe((result) => {
          this.vendor = result;
          this.AltDateOfBirth = new Date(result?.dateOfBirth.format('YYYY-MM-DD'));
        });
      // get vendor ContactInfo
      this._vendorContactInfoServiceProxy
        .get(this.data.id)
        .pipe(finalize(() => { }))
        .subscribe((result) => {
          this.createOrEditVendorContactInfoDto = result;
          this.spinner.hide();
        });
      // get vendor Address
      this._vendorContactInfoServiceProxy
        .getAddress(this.data.id)
        .pipe(finalize(() => { }))
        .subscribe((result) => {
          this.vendorAddressDto = result;
          this.spinner.hide();
        });
    }
    this.getVendorTypeList();
  }

  getRelevantData() {
    this.contactPersonType();
  }

  vendorInfoValidation(): boolean {
    if (!this.vendor.businessName || !this.vendor.vendorName) {
      return false;
    }
    else {
      return true;
    }
  }

  saveVendorInfo() {
    if (this.AltDateOfBirth) {
      this.vendor.dateOfBirth = moment(new Date(this.AltDateOfBirth.getFullYear(), this.AltDateOfBirth.getMonth(), this.AltDateOfBirth.getDate(), this.AltDateOfBirth.getHours(), this.AltDateOfBirth.getMinutes() - this.AltDateOfBirth.getTimezoneOffset()).toISOString());
    }
    this._vendorServiceProxy
      .createOrEdit(this.vendor)
      .subscribe((newVendorId) => {
        this.vendorId = newVendorId; 
        this.notify.success("vendor info save successfully");
        if (this.lastTab) {
          this.lastTab = false;
          this.notify.success("vendor info save successfully");
        }
      });
  }

  saveVendorContactInfo() {
    if (!this.vendorId) {
      this.moveToSelectedTab("INFO");
      return this.notify.error("add first vendor infomation");
    }
    if (!this.createOrEditVendorContactInfoDto.contactPersonName) {
      return this.notify.error("please add contact person name");
    }
    if (!this.createOrEditVendorContactInfoDto.emailAddress) {
      return this.notify.error("please add email address");
    }
    this.createOrEditVendorContactInfoDto.vendorId = this.vendorId;
    this._vendorContactInfoServiceProxy.createOrEdit(this.createOrEditVendorContactInfoDto)
      .subscribe((arg) => {
        this.notify.success("contact info save successfully");
      })
  }

  saveVendorAddress() {
    if (!this.vendorId) {      
      this.moveToSelectedTab("Contact Info");
      return this.notify.error("add first contact infomation");
    }
    if (!this.vendorAddressDto.completeAddress) {
      return this.notify.error("please add your address");
    }
    this.vendorAddressDto.vendorId = this.vendorId;
    this._vendorContactInfoServiceProxy.createOrEditAddress(this.vendorAddressDto)
      .subscribe((arg) => {
        this.notify.success("address info save successfully");
        this.hideDialog();
      });
  }

  protected getVendorTypeList() {
    this._venderTypeServiceProxy.getAll().subscribe((arg) => (this.venderTypeDto = arg));
  }

  protected contactPersonType() {
    this._contactPersonTypeServiceProxy
      .getAll()
      .subscribe((arg) => (this.contactPersonTypeDto = arg));
  }


  public openCreateLanguageDialog(id?: number): void {
    const dialogRef = this._dialog.open(CreatelanguageComponent, {
      data: { id: id },
    });

    dialogRef.afterClosed().subscribe((result) => {
      dialogRef.close();

    });
  }

  hideDialog() {
    this.onSave.emit();
    this.dialogRef.close();
  }


  moveToSelectedTab(tabName: string) {
    switch (tabName) {
      case "INFO":
      break;
      case "Contact Info":
        // if (!this.vendorInfoValidation()) {
        //   return abp.message.error('Please Add required fields', 'Field Empty!');
        // }
        break;
      case "Address":
        this.lastTab = true;
        break;
    }

    for (let i = 0; i < document.querySelectorAll('.mat-tab-label-content').length; i++) {
      if ((<HTMLElement>document.querySelectorAll('.mat-tab-label-content')[i]).innerText == tabName) {
        (<HTMLElement>document.querySelectorAll('.mat-tab-label')[i]).click();
      }
    }
  }

}


