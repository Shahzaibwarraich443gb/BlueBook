
import { ValidationService } from '../../../shared/Services/validation.service';
import {
  Component,
  EventEmitter,
  Inject,
  Injector,
  OnInit,
  Output,
  ViewChild,
  ElementRef,
  HostListener
} from "@angular/core";
import {
  MatDialog,
  MatDialogRef,
  MAT_DIALOG_DATA,

} from "@angular/material/dialog";
import { appModuleAnimation } from "@shared/animations/routerTransition";
import { AppComponentBase } from "@shared/app-component-base";
import { SaveReceivedPayment, ReceivedPaymentServiceServiceProxy, ChargeCardDto, CustomerTransactionDto } from '@shared/service-proxies/service-proxies';

export interface DialogData {
  items: number;
}

@Component({
  selector: "app-email-modal",
  templateUrl: "./email.component.html",
  styleUrls: ["./email.component.scss"],
  animations: [appModuleAnimation()],
})
export class EmailComponent extends AppComponentBase implements OnInit {
  email: string;
  constructor(
    injector: Injector,
    public dialog: MatDialog,
    public dialogRef: MatDialogRef<ChargeCardDto>,
    private el: ElementRef,
    @Inject(MAT_DIALOG_DATA) public data: any,
    public validation: ValidationService,
    private _receviedPaymentServiceProxy: ReceivedPaymentServiceServiceProxy,
  ) {
    super(injector);
  }

  ngOnInit() {
    if (this.data.items) {
      console.log("dataList:", this.data.items);
      this.email = this.data.items.email;
    }
  }

  save() {
    this._receviedPaymentServiceProxy.emailReceivePayment(this.email, this.data.items).subscribe((res) => {
      this.notify.info(this.l("Send Email Successfully"));
      this.hideDialog();
    })
  }

  hideDialog() {
    this.dialogRef.close();
  }

}
