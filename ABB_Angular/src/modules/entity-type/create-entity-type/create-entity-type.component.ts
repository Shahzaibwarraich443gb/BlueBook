import { CreateOrEditGeneralEntityTypeInputDto, GeneralEntityTypeServiceProxy } from './../../../shared/service-proxies/service-proxies';
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
  selector: "app-create-entity-type",
  templateUrl: "./create-entity-type.component.html",
  styleUrls: ["./create-entity-type.component.scss"],
  animations: [appModuleAnimation()],
})
export class CreateEntityTypeComponent
  extends AppComponentBase
  implements OnInit
{
  
  @Output() onSave = new EventEmitter<any>();

  createOrEditGeneralEntityTypeInputDto =
    new CreateOrEditGeneralEntityTypeInputDto();

  constructor(
    injector: Injector,
    public dialog: MatDialog,
    public dialogRef: MatDialogRef<any>,
    public _generalEntityTypeServiceProxy: GeneralEntityTypeServiceProxy,
    @Inject(MAT_DIALOG_DATA) public data: DialogData,
    private _sessionService: AppSessionService,
    public validation: ValidationService
  ) {
    super(injector);
  }

  ngOnInit() {
    if (this.data.id) {
      if (this.data.id) {
        this._generalEntityTypeServiceProxy
          .get(this.data.id)
          .pipe(finalize(() => {}))
          .subscribe((result) => {
            this.createOrEditGeneralEntityTypeInputDto = result;
          });
      } else 
      this.createOrEditGeneralEntityTypeInputDto =
        new CreateOrEditGeneralEntityTypeInputDto();
    }
  }

  save() { 
    this._generalEntityTypeServiceProxy
      .createOrEdit(this.createOrEditGeneralEntityTypeInputDto)
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
