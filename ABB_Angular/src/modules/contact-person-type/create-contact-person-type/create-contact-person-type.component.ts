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
import { ValidationService } from "@shared/Services/validation.service";
import { AppSessionService } from "@shared/session/app-session.service";

import { finalize } from "rxjs";
import {
  ContactPersonTypeServiceProxy,
  CreateOrEditContactPersonTypeInputDto,
} from "../../../shared/service-proxies/service-proxies";
export interface DialogData {
  id: number;
}

@Component({
  selector: "app-create-contact-person-type",
  templateUrl: "./create-contact-person-type.component.html",
  styleUrls: ["./create-contact-person-type.component.scss"],
  animations: [appModuleAnimation()],
})
export class CreateContactTypePersonComponent
  extends AppComponentBase
  implements OnInit
{
  @Output() onSave = new EventEmitter<any>();

  createOrEditContactPersonTypeInputDto =
    new CreateOrEditContactPersonTypeInputDto();

  constructor(
    injector: Injector,
    public dialog: MatDialog,
    public dialogRef: MatDialogRef<any>,
    public _contackPersonTypeServiceProxy: ContactPersonTypeServiceProxy,
    @Inject(MAT_DIALOG_DATA) public data: DialogData,
    private _sessionService: AppSessionService,
    public validation: ValidationService,
  ) {
    super(injector);
  }
  customerDto: any[] = [];
  ngOnInit() {
    if (this.data.id) {
      ;
      this._contackPersonTypeServiceProxy
        .get(this.data.id)
        .pipe(finalize(() => {}))
        .subscribe((result) => {
          (this.createOrEditContactPersonTypeInputDto = result)

        });
    } else
      
      this.createOrEditContactPersonTypeInputDto =
        new CreateOrEditContactPersonTypeInputDto();
  }

  save() {
    //  let companyId = this._sessionService.companyId;
    //this.createOrEditContactPersonTypeInputDto.companyId = companyId;
    this._contackPersonTypeServiceProxy
      .createOrEdit(this.createOrEditContactPersonTypeInputDto)
      .subscribe((arg) => {
        this.dialogRef.close(); 
        this.onSave.emit();
      });
  }

  firstSpaceNotAllowed(event) {
    this.validation.letterOnlyWithSpaceAllowed(event);
  }

  hideDialog() {
    this.onSave.emit();
    this.dialogRef.close();
  }
}
