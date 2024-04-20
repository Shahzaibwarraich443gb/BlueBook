import { CreateOrEditSpouseDto, EthnicitiesServiceProxy, EthnicityDto, JobTitleDto, JobTitleServiceProxy, LanguageDto, SourceReferralTypeServiceProxy, SpouseServiceProxy } from '../../../shared/service-proxies/service-proxies';
import { CreateOrEditSourceReferralTypeDto, Phone } from "../../../shared/service-proxies/service-proxies";
import { ValidationService } from "../../../shared/Services/validation.service";
import { DatePipe } from '@angular/common';

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

import * as _moment from 'moment';
import { Moment } from 'moment';

const moment = _moment;
export interface DialogData {
  id: number;

}

@Component({
  selector: "app-create-spouse",
  templateUrl: "./create-spouse.component.html",
  styleUrls: ["./create-spouse.component.scss"],
  animations: [appModuleAnimation()],
})
export class CreateSpouseComponent
  extends AppComponentBase
  implements OnInit
{
  @Output() onSave = new EventEmitter<any>();
  jobTitleDto: JobTitleDto[] = [];
  ethnicityDto: EthnicityDto[] = [];
  languageDto: LanguageDto[] = [];
  createOrEditSpouseDto = new CreateOrEditSpouseDto();
  selectedDate:Moment ;
 
  constructor(
    injector: Injector,
    public dialog: MatDialog,
    public dialogRef: MatDialogRef<any>,
    public _spouseServiceProxy: SpouseServiceProxy,
    public _ethnicityServiceProxy: EthnicitiesServiceProxy,
    public _jobTitleServiceProxy: JobTitleServiceProxy,
    public _languageServiceProxy: LanguageServiceProxy,
    @Inject(MAT_DIALOG_DATA) public data: DialogData,
 
    public validation: ValidationService
  ) {
    super(injector);
  }
  customerDto: any[] = [];
  ngOnInit() {
    this.jobList();
    this.ethnicityList();
    this.languageList()
    if (this.data.id) {
      if (this.data.id) { 
        this._spouseServiceProxy
          .get(this.data.id)
          .pipe(finalize(() => {}))
          .subscribe((result) => { 
            this.createOrEditSpouseDto = result; 
      
          });
      } else 
      
      this.createOrEditSpouseDto =
        new CreateOrEditSpouseDto();
        this.createOrEditSpouseDto.isActive = true; 
    }
    if (this.data.id == null) {
      this.createOrEditSpouseDto.isActive = true;
    }
  }

  save() {
    console.log(this.createOrEditSpouseDto)
    this._spouseServiceProxy
      .createOrEdit(this.createOrEditSpouseDto)
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

  ethnicityList(){
    this._ethnicityServiceProxy
    .getAll()
    .subscribe((arg) => (this.ethnicityDto = arg));
  }

  jobList(){
    this._jobTitleServiceProxy
    .getAll()
    .subscribe((arg) => (this.jobTitleDto = arg));
  }

  languageList(){
    this._languageServiceProxy
    .getAll()
    .subscribe((arg) => (this.languageDto = arg));
  }

 
}
