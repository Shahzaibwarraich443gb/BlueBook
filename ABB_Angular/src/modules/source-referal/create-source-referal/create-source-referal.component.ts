import { CreateOrEditEmailInputDto, SourceReferralTypeDto, SourceReferralTypeServiceProxy } from '../../../shared/service-proxies/service-proxies';
import { CreateOrEditSourceReferralTypeDto, Phone } from "../../../shared/service-proxies/service-proxies";
import { ValidationService } from "../../../shared/Services/validation.service";
import {
  CreateOrEditAddressDto,
  CreateOrEditPhoneDto,
  CreateOrEditSalesPersonTypeDto,
  LanguageServiceProxy,
  SalesPersonTypeServiceProxy,
} from "../../../shared/service-proxies/service-proxies";
import {
  Component,
  EventEmitter,
  Inject,
  Injector,
  OnInit,
  Output,
  ViewChild,
} from "@angular/core";
import {
  MatDialog,
  MatDialogRef,
  MAT_DIALOG_DATA,
} from "@angular/material/dialog";
import { appModuleAnimation } from "@shared/animations/routerTransition";
import { AppComponentBase } from "@shared/app-component-base";

import { finalize } from "rxjs";

export interface DialogData {
  id: number;
}

@Component({
  selector: "app-create-source-referal",
  templateUrl: "./create-source-referal.component.html",
  styleUrls: ["./create-source-referal.component.scss"],
  animations: [appModuleAnimation()],
})
export class CreateSourecReferalComponent
  extends AppComponentBase
  implements OnInit {
  @Output() onSave = new EventEmitter<any>();

  createOrEditSourceReferralTypeDto = new CreateOrEditSourceReferralTypeDto(); 
  createOrUpdateAddress = new CreateOrEditAddressDto();
  createOrUpdatePhoneDto = new CreateOrEditPhoneDto();
  email = new CreateOrEditEmailInputDto();
  constructor(
    injector: Injector,
    public dialog: MatDialog,
    public dialogRef: MatDialogRef<any>,
    public _sourceReferralTypeServiceProxy: SourceReferralTypeServiceProxy,
    @Inject(MAT_DIALOG_DATA) public data: DialogData, 
    public validation: ValidationService,
  ) {
    super(injector);
  } 


     ngOnInit() {
    if (this.data.id) {
      if (this.data.id) { 
        this._sourceReferralTypeServiceProxy
          .get(this.data.id)
          .pipe(finalize(() => { }))
          .subscribe((result) => {
            ;
       this.createOrEditSourceReferralTypeDto = result;
       this.createOrUpdateAddress = this.createOrEditSourceReferralTypeDto.address;
       this.createOrUpdatePhoneDto = this.createOrEditSourceReferralTypeDto.phone;
        this.email = this.createOrEditSourceReferralTypeDto.email; 
          });
      } else  
      this.createOrEditSourceReferralTypeDto =
        new CreateOrEditSourceReferralTypeDto();
    } 
  }

    save() {
    this.MapEntities();
    this.AddRelevantInfo();
    this._sourceReferralTypeServiceProxy
      .createOrEdit(this.createOrEditSourceReferralTypeDto)
      .subscribe((arg) => {
        this.dialogRef.close(); 
        this.onSave.emit();
      });

  }

  MapEntities() {
    this.createOrEditSourceReferralTypeDto.email = this.email;
    this.createOrEditSourceReferralTypeDto.address = this.createOrUpdateAddress;
    this.createOrEditSourceReferralTypeDto.phone = this.createOrUpdatePhoneDto;
  }

  AddRelevantInfo() {
    this.email.isPrimary = true;
    this.email.typeEmail = 1;
    this.createOrEditSourceReferralTypeDto.address.type = 1;
    this.createOrEditSourceReferralTypeDto.address.isPrimary = true;
    this.createOrEditSourceReferralTypeDto.phone.isPrimary = true;
    this.createOrEditSourceReferralTypeDto.phone.type = 1;
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
