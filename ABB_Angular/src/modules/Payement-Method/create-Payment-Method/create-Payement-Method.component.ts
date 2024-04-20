
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
import { CreateOrEditGeneralPaymentMethodInputDto, PaymentMethodServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppSessionService } from "@shared/session/app-session.service";
import { finalize } from "rxjs";

export interface DialogData {
  id: number;
}

@Component({
  selector: "app-create-Payment-Method",
  templateUrl: "./create-Payment-Method.component.html",
  styleUrls: ["./create-Payment-Method.component.scss"],
  animations: [appModuleAnimation()],
})
export class CreatePaymentMethodComponent
  extends AppComponentBase
  implements OnInit {
  @ViewChild('paymentMethodInput') paymentMethodInput1: ElementRef;

  @Output() onSave = new EventEmitter<any>();
  createoreditInputTypedto = new CreateOrEditGeneralPaymentMethodInputDto();

  constructor(
    injector: Injector,
    public dialog: MatDialog,
    public dialogRef: MatDialogRef<any>,
    public _paymentService: PaymentMethodServiceProxy,

    @Inject(MAT_DIALOG_DATA) public data: DialogData,
    private _sessionService: AppSessionService,
    public validation: ValidationService
  ) {
    super(injector);
  }

  ngOnInit() {
    if (this.data.id) {
      if (this.data.id) {
        this._paymentService
          .get(this.data.id)
          .pipe(finalize(() => { }))
          .subscribe((result) => {
            this.createoreditInputTypedto = result;
          });
      } else
        this.createoreditInputTypedto =
          new CreateOrEditGeneralPaymentMethodInputDto();
    }
  }

  ngAfterViewInit() {
    if(this.paymentMethodInput1!== undefined){
      this.paymentMethodInput1.nativeElement.focus();
    }
  }
  
  save() {
    this._paymentService
      .createOrEdit(this.createoreditInputTypedto)
      .subscribe((arg) => {
        abp.notify.success('save Succesfully')
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
