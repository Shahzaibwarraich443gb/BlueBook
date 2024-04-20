import { Component, EventEmitter, Inject, OnInit, Output } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { CreatelanguageComponent } from 'modules/Language/create-language/create-language.component';
import { CreateOrEditCustomerDto, EthnicitiesServiceProxy, EthnicityDto, LanguageDto, LanguageServiceProxy, ContactPersonTypeServiceProxy, ContactPersonTypeDto, SourceReferralTypeDto, SalesPersonTypeDto, SalesPersonTypeServiceProxy, GeneralEntityTypeServiceProxy, GeneralEntityTypeDto, EmailDto, PhoneDto, AddressDto, CustomerServiceProxy, SourceReferralTypeServiceProxy, CreateOrEditEmailInputDto, CreateOrEditAddressDto, CreateOrEditPhoneDto, VenderServiceProxy, VenderTypeServiceProxy, VenderTypeDto, CreateOrEditVenderDto, VendorContactInfoServiceProxy, CreateOrEditVendorContactInfoDto, ContactPersonType, ContactTypeEnum, CreateOrEditBankDto, CreateOrEditBankAddressDto, BankServiceProxy } from './../../../shared/service-proxies/service-proxies';
import { CreateEthnicityComponent } from 'modules/ethnicity/create-ethnicity/create-ethnicity.component';
import { CreateContactTypePersonComponent } from 'modules/contact-person-type/create-contact-person-type/create-contact-person-type.component';
import { CreateSalePersonComponent } from 'modules/sale-person/create-sale-person/create-sale-person.component';
import { CreateEntityTypeComponent } from 'modules/entity-type/create-entity-type/create-entity-type.component';
import { finalize } from 'rxjs';
import {   CreateSourecReferalComponent } from 'modules/source-referal/create-source-referal/create-source-referal.component';
import { AppConsts } from '@shared/AppConsts';
import { ValidationService } from '@shared/Services/validation.service';

export interface DialogData {
  id: number;
}

@Component({
  selector: 'app-create-or-edit-bank',
  templateUrl: './create-or-edit-bank.component.html',
  styleUrls: ['./create-or-edit-bank.component.scss']
})

export class CreateOrEditBankComponent implements OnInit {
  @Output() onSave = new EventEmitter<any>();
 
  createOrEditbankDto = new CreateOrEditBankDto(); 
  createOrEditBankAddressDto = new CreateOrEditBankAddressDto();
 
   
  selectedIndex: number = 0;
  constructor(
    public validation: ValidationService,
    public _dialog: MatDialog,
    public dialogRef: MatDialogRef<any>,
    public _bankServiceProxy:  BankServiceProxy  ,
    @Inject(MAT_DIALOG_DATA) public data: DialogData) { }

  ngOnInit() {
   
    if (this.data.id) {
      if (this.data.id) { 
        this._bankServiceProxy
          .get(this.data.id)
          .pipe(finalize(() => { }))
          .subscribe((result) => {
            ;
       this.createOrEditbankDto = result;
       this.createOrEditBankAddressDto = this.createOrEditbankDto.address; 
          });
      } else ;
   
      this.createOrEditbankDto = new CreateOrEditBankDto();
    }
  }

  MapEntities() {
    this.createOrEditbankDto.address = this.createOrEditBankAddressDto;
  }
  saveBank() {
    this.MapEntities();
     
    this._bankServiceProxy
      .createOrEdit(this.createOrEditbankDto)
      .subscribe((arg) => {
        this.dialogRef.close();

        this.onSave.emit();
      });

  }

  

  hideDialog() {
    this.onSave.emit();
    this.dialogRef.close();
  }

  numberOnly(event) {
    return this.validation.numberOnlyWith(event);
  }

  letterOnly(event) {
    return this.validation.letterOnlyWithSpaceAllowed(event);
  }

  firstSpaceNotAllowed(event) {
    this.validation.letterOnlyWithSpaceAllowed(event);
  }


   


  
}
 

