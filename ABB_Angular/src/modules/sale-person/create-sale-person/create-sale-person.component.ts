import { Phone } from './../../../shared/service-proxies/service-proxies';
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
import {
  ContactPersonTypeServiceProxy,
  CreateOrEditContactPersonTypeInputDto,
  CreateOrEditLanguageInputDto,
} from "../../../shared/service-proxies/service-proxies";
export interface DialogData {
  id: number;
}

@Component({
  selector: "app-create-sale-person",
  templateUrl: "./create-sale-person.component.html",
  styleUrls: ["./create-sale-person.component.scss"],
  animations: [appModuleAnimation()],
})
export class CreateSalePersonComponent
  extends AppComponentBase
  implements OnInit
{
  @Output() onSave = new EventEmitter<any>();

  createOrEditSalesPersonTypeDto = new CreateOrEditSalesPersonTypeDto();
  createOrUpdateAddress = new CreateOrEditAddressDto();
  createOrUpdatePhoneDto = new CreateOrEditPhoneDto();
  constructor(
    injector: Injector,
    public dialog: MatDialog,
    public dialogRef: MatDialogRef<any>,
    public _salesPersonTypeServiceProxy: SalesPersonTypeServiceProxy,
    @Inject(MAT_DIALOG_DATA) public data: DialogData,

    public validation: ValidationService
  ) {
    super(injector);
  }
  customerDto: any[] = [];
  ngOnInit() {
    if (this.data.id) {
      if (this.data.id) {
        ;
        this._salesPersonTypeServiceProxy
          .get(this.data.id)
          .pipe(finalize(() => {}))
          .subscribe((result) => { 
            this.createOrUpdateAddress = result.address;
             this.createOrUpdatePhoneDto = result.phone;
            this.createOrEditSalesPersonTypeDto = result;
          });
      } else 
      this.createOrEditSalesPersonTypeDto =
        new CreateOrEditSalesPersonTypeDto();
      this.createOrEditSalesPersonTypeDto.address =
        new CreateOrEditAddressDto();
      this.createOrEditSalesPersonTypeDto.phone = new CreateOrEditPhoneDto();
    }
    if (this.data.id == null) {
      this.createOrEditSalesPersonTypeDto.isActive = true;
    }
  }

  save() { 
    this.createOrEditSalesPersonTypeDto.address = this.createOrUpdateAddress;
    this.createOrEditSalesPersonTypeDto.phone = this.createOrUpdatePhoneDto;
    this._salesPersonTypeServiceProxy
      .createOrEdit(this.createOrEditSalesPersonTypeDto)
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