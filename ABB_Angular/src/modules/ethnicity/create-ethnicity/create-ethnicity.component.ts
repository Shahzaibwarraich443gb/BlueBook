import { CreateOrEditEthnicityDto, EthnicitiesServiceProxy, LanguageServiceProxy } from '../../../shared/service-proxies/service-proxies';
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
import { ValidationService } from '@shared/Services/validation.service';
export interface DialogData {
  id: number;
}

@Component({
  selector: "app-create-ethnicity",
  templateUrl: "./create-ethnicity.component.html",
  styleUrls: ["./create-ethnicity.component.scss"],
  animations: [appModuleAnimation()],
})
export class CreateEthnicityComponent
  extends AppComponentBase
  implements OnInit {
  @Output() onSave = new EventEmitter<any>();
  createOrEditEthnicityInputDto = new CreateOrEditEthnicityDto();

  constructor(
    injector: Injector,
    public dialog: MatDialog,
    public dialogRef: MatDialogRef<any>,
    public _ethnicityServiceProxy: EthnicitiesServiceProxy,
    @Inject(MAT_DIALOG_DATA) public data: DialogData,
    private _sessionService: AppSessionService,
    public validation: ValidationService,
  ) {
    super(injector);
  }

  ngOnInit() {
    if (this.data.id) {
      if (this.data.id) {
        this._ethnicityServiceProxy
          .get(this.data.id)
          .pipe(finalize(() => { }))
          .subscribe((result) => {
            this.createOrEditEthnicityInputDto = result;
          });
      } else
        this.createOrEditEthnicityInputDto = new CreateOrEditEthnicityDto();
    }
  }

  save() {
    this._ethnicityServiceProxy
      .createOrEdit(this.createOrEditEthnicityInputDto)
      .subscribe((arg) => {
        this.dialogRef.close();
        this.onSave.emit();
      });
  }

  hideDialog() {
    this.onSave.emit();
    this.dialogRef.close();
  }

  letterOnly(event) {
    return this.validation.letterOnlyWithSpaceAllowed(event);
  }

  numberOnly(event) {
    return this.validation.numberOnlyWith(event);
  }

  firstSpaceNotAllowed(event) {
    this.validation.letterOnlyWithSpaceAllowed(event);
  }

}
