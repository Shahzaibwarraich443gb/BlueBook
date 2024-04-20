import { CreateOrEditGeneralEntityTypeInputDto, CreateOrEditJobTitleInputDto, CreateOrEditProductCategoryDto, GeneralEntityTypeServiceProxy, JobTitleServiceProxy, ProductCategoryEnum, ProductCategoryServiceProxy } from './../../../shared/service-proxies/service-proxies';
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
  selector: "app-create-job-title",
  templateUrl: "./create-product-category.component.html",
  styleUrls: ["./create-product-category.component.scss"],
  animations: [appModuleAnimation()],
})
export class CreateProductCategoryComponent
  extends AppComponentBase
  implements OnInit {
  @Output() onSave = new EventEmitter<any>();

  createOrEditProductCategoryDto = new CreateOrEditProductCategoryDto();

  constructor(
    injector: Injector,
    public dialog: MatDialog,
    public dialogRef: MatDialogRef<any>,
    public _productCategoryServiceProxy: ProductCategoryServiceProxy,
    @Inject(MAT_DIALOG_DATA) public data: DialogData,
    private _sessionService: AppSessionService,
    public validation: ValidationService
  ) {
    super(injector);
  }

  ngOnInit() {
    if (this.data.id) {
      if (this.data.id) { 
        this._productCategoryServiceProxy
          .get(this.data.id)
          .pipe(finalize(() => { }))
          .subscribe((result) => {
            this.createOrEditProductCategoryDto = result;
          });
      } else
        this.createOrEditProductCategoryDto =
          new CreateOrEditProductCategoryDto();

    }

    if (this.data.id == null) {
      this.createOrEditProductCategoryDto.isActive = true;
      this.createOrEditProductCategoryDto.nature = 'service';
    }
  }

  save() {
    this._productCategoryServiceProxy
      .createOrEdit(this.createOrEditProductCategoryDto)
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
