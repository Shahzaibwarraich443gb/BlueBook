
import { ValidationService } from '../../../shared/Services/validation.service';
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
import { appModuleAnimation } from "@shared/animations/routerTransition";
import { AppComponentBase } from "@shared/app-component-base";
import { ChargeCardDto, VenderServiceProxy } from '@shared/service-proxies/service-proxies';
import { NgxSpinnerService } from 'ngx-spinner';
import { finalize } from 'rxjs';

export interface DialogData {
  items: number;
}
@Component({
  selector: "app-email-attachment",
  templateUrl: "./email-attachment.component.html",
  styleUrls: ["./email-attachment.component.scss"],
  animations: [appModuleAnimation()],
})
export class EmailAttachmentComponent extends AppComponentBase implements OnInit {
  emailBody: string;
  subject: string
  attachmentList: any;
  vendorId: number;
  constructor(
    injector: Injector,
    public _vendorService: VenderServiceProxy,
    public dialog: MatDialog,
    public dialogRef: MatDialogRef<ChargeCardDto>,
    private el: ElementRef,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private spinner: NgxSpinnerService,
    public validation: ValidationService,
  ) {
    super(injector);
  }

  ngOnInit() {
    if (this.data.id) {
      this.vendorId = this.data.id;
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
    if(!this.subject){
      return this.notify.error("please enter subject");
    }
    if(!this.emailBody){
      return this.notify.error("please enter email body");
    }
    if(this.attachmentList.length === 0){
      return this.notify.error("please select attachment");
    }
    this.spinner.show();
    //this.subject, this.emailBody, this.attachmentList, this.vendorId;
    const attachmant = this.attachmentList.map(item => item.fileName);
    this._vendorService.sendEmailToVendor(this.subject, this.emailBody, attachmant, this.vendorId).pipe(
      finalize(() => { })).subscribe((result) => {
        this.spinner.hide();
        this.dialogRef.close();
        this.notify.success("email send successfully");
      });
  }

  hideDialog() {
    this.dialogRef.close();
  }

}
