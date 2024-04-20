
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
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/app-component-base';
import * as moment from 'moment';

export interface DialogData {
  item: any;
}

@Component({
  selector: "app-view-history",
  templateUrl: "./view-history.component.html",
  styleUrls: ["./view-history.component.scss"],
  animations: [appModuleAnimation()],
})
export class ViewHistoryComponent extends AppComponentBase implements OnInit {

  viewHistory: any;
  constructor(
    injector: Injector,
    public dialog: MatDialog,
    public dialogRef: MatDialogRef<any>,
    private el: ElementRef,
    @Inject(MAT_DIALOG_DATA) public data: any,
    public validation: ValidationService
  ) {
    super(injector);
  }
  addedBy: string;
  addedDate:any;
  modifyBy: string;
  modifyLate: string;

  ngOnInit() {
    if (this.data.item) {
      this.addedBy = this.data.item.addedBy;
      this.addedDate = moment(this.data.item.creationTime).format('MM/DD/YYYY');
      this.modifyBy = this.data.item.lastModifierUserId;
      this.modifyLate = moment(this.data.item.lastModificationTime).format('MM/DD/YYYY');
    }
  }

  save() {

  }


}
