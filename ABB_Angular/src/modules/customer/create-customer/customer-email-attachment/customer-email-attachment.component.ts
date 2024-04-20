
import {
  Component,
  Inject,
  Injector,
  OnInit,
  ElementRef,
} from "@angular/core";
import {
  MatDialog,
  MatDialogRef,
  MAT_DIALOG_DATA,

} from "@angular/material/dialog";
import { ValidationService } from "@shared/Services/validation.service";
import { appModuleAnimation } from "@shared/animations/routerTransition";
import { AppComponentBase } from "@shared/app-component-base";
import { ChargeCardDto, CustomerServiceProxy, VenderServiceProxy } from '@shared/service-proxies/service-proxies';
import { NgxSpinnerService } from 'ngx-spinner';
import { finalize } from 'rxjs';

export interface DialogData {
  items: number;
}
@Component({
  selector: "app-customer-email-attachment",
  templateUrl: "./customer-email-attachment.component.html",
  styleUrls: ["./customer-email-attachment.component.scss"],
  animations: [appModuleAnimation()],
})
export class CustomerEmailAttachmentComponent extends AppComponentBase implements OnInit {
  emailBody: string;
  subject: string
  attachmentList: any;
  customerId: number;
  constructor(
    injector: Injector,
    public _customerService: CustomerServiceProxy,
    public dialog: MatDialog,
    public dialogRef: MatDialogRef<CustomerEmailAttachmentComponent>,
    private el: ElementRef,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private spinner: NgxSpinnerService,
    public validation: ValidationService,
  ) {
    super(injector);
  }

  ngOnInit() {
    if (this.data.id) {
      this.customerId = this.data.id;
    }
    if (this.data.selectedEmails) {
      this.attachmentList = this.data.selectedEmails;
    }
  }

  remove(item: any) {
    this.attachmentList = this.attachmentList.filter((obj) => {
      return obj.fileNameAlt !== item.fileNameAlt
    });
  }

  save() {
    if (!this.subject) {
      return this.notify.error("please enter subject");
    }
    if (!this.emailBody) {
      return this.notify.error("please enter email body");
    }
    if (this.attachmentList.length === 0) {
      return this.notify.error("please select attachment");
    }
    this.spinner.show();
    const attachmant = this.attachmentList.map(item => item.fileName);
    this._customerService.sendEmailToCustomer(this.subject, this.emailBody, attachmant, this.customerId).pipe(
      finalize(() => {
        this.spinner.hide();
      })).subscribe((result) => {
        this.dialogRef.close();
        this.notify.success("email send successfully");
      });
  }

  hideDialog() {
    this.dialogRef.close();
  }

}
