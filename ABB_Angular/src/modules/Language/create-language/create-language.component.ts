import { ValidationService } from './../../../shared/Services/validation.service';
import { LanguageServiceProxy } from './../../../shared/service-proxies/service-proxies';
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
  selector: "app-create-language",
  templateUrl: "./create-language.component.html",
  styleUrls: ["./create-language.component.scss"],
  animations: [appModuleAnimation()],
})
export class CreatelanguageComponent
  extends AppComponentBase
  implements OnInit {
  @Output() onSave = new EventEmitter<any>();
  createOrEditLanguageInputDto = new CreateOrEditLanguageInputDto();

  constructor(
    injector: Injector,
    public dialog: MatDialog,
    public dialogRef: MatDialogRef<any>,
    public _languageServiceProxy: LanguageServiceProxy,
    @Inject(MAT_DIALOG_DATA) public data: DialogData,
    public validation: ValidationService
  ) {
    super(injector);
  }

  ngOnInit() {
    if (this.data.id) {
      if (this.data.id) {
        this._languageServiceProxy
          .get(this.data.id)
          .pipe(finalize(() => { }))
          .subscribe((result) => {
            this.createOrEditLanguageInputDto = result;
          });
      } else
        this.createOrEditLanguageInputDto = new CreateOrEditLanguageInputDto();
    }
  }

  save() {
    this._languageServiceProxy
      .createOrEdit(this.createOrEditLanguageInputDto)
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
