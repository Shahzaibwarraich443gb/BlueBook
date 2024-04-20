import { CompanyServiceProxy, CreateOrEditCompanyDto, CreateOrEditGeneralEntityTypeInputDto, CreateOrEditJobTitleInputDto, CreateOrEditProductCategoryDto, GeneralEntityTypeServiceProxy, JobTitleServiceProxy, ProductCategoryServiceProxy } from './../../../shared/service-proxies/service-proxies';
import { ValidationService } from '../../../shared/Services/validation.service';
import { LanguageServiceProxy } from '../../../shared/service-proxies/service-proxies';
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
import { AppSessionService } from "@shared/session/app-session.service";

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
  selector: "app-create-company",
  templateUrl: "./create-company.component.html",
  styleUrls: ["./create-company.component.scss"],
  animations: [appModuleAnimation()],
})
export class CreateCompanyComponent
  extends AppComponentBase
  implements OnInit {
  @Output() onSave = new EventEmitter<any>();

  createOrEditCompanyDto = new CreateOrEditCompanyDto();

  constructor(
    injector: Injector,
    public dialog: MatDialog,
    public dialogRef: MatDialogRef<any>,
    public _companyServiceProxy: CompanyServiceProxy,
    @Inject(MAT_DIALOG_DATA) public data: DialogData,
    private _sessionService: AppSessionService,
    public validation: ValidationService
  ) {
    super(injector);
  }

  customerDto: any[] = [];

  ngOnInit() {
    if (this.data.id) {
      if (this.data.id) {
        this._companyServiceProxy
          .get(this.data.id)
          .pipe(finalize(() => { }))
          .subscribe((result) => {
            this.createOrEditCompanyDto = result;
          });
      } else
        this.createOrEditCompanyDto =
          new CreateOrEditCompanyDto();
    }

  }

  save() {
    this._companyServiceProxy
      .createOrEdit(this.createOrEditCompanyDto)
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
