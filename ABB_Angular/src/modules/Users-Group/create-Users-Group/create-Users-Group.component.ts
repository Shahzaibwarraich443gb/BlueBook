
import { ValidationService } from '../../../shared/Services/validation.service';
import {
  Component,
  EventEmitter,
  Inject,
  Injector,
  OnInit,
  Output,
  ViewChild,
  ElementRef
} from "@angular/core";
import {
  MatDialog,
  MatDialogRef,
  MAT_DIALOG_DATA,

} from "@angular/material/dialog";
import { appModuleAnimation } from "@shared/animations/routerTransition";
import { AppComponentBase } from "@shared/app-component-base";
import { CreateOrEditGeneralPaymentMethodInputDto, CreateOrEditGeneralUserGroupInputDto, CreateOrEditUserDto, PaymentMethodServiceProxy, UserGroupsServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppSessionService } from "@shared/session/app-session.service";
import { finalize } from "rxjs";

export interface DialogData {
  id: number;
}

@Component({
  selector: "app-create-Users-Group",
  templateUrl: "./create-Users-Group.component.html",
  styleUrls: ["./create-Users-Group.component.scss"],
  animations: [appModuleAnimation()],
})
export class CreateUsersGroupComponent
  extends AppComponentBase
  implements OnInit {
  @ViewChild('UserGroupInputInput') usergroupInput1: ElementRef;

  @Output() onSave = new EventEmitter<any>();
  createoreditInputTypedto = new CreateOrEditGeneralPaymentMethodInputDto();
  Dto = new CreateOrEditGeneralUserGroupInputDto();

  ipAddressList: string[] = [];


  constructor(
    injector: Injector,
    public dialog: MatDialog,
    public dialogRef: MatDialogRef<any>,
    // public _paymentService: PaymentMethodServiceProxy,
    public _usergroup: UserGroupsServiceProxy,

    @Inject(MAT_DIALOG_DATA) public data: DialogData,
    private _sessionService: AppSessionService,
    public validation: ValidationService
  ) {
    super(injector);
  }

  ngOnInit() {
    if (this.data.id) {
      if (this.data.id) {
        this._usergroup
          .get(this.data.id)
          .pipe(finalize(() => { }))
          .subscribe((result) => {
            this.Dto = result;

          });
      } else
        this.Dto =
          new CreateOrEditGeneralUserGroupInputDto();
    }
  }

  ngAfterViewInit() {
    if (this.usergroupInput1 !== undefined) {
      this.usergroupInput1.nativeElement.focus();
    }
  }

  save() {
    this._usergroup
      .createOrEdit(this.Dto)
      .subscribe((arg) => {
        abp.notify.success('save Succesfully')
        this.dialogRef.close();

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
