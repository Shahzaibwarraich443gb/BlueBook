import { Component, EventEmitter, Inject, Injector, OnInit, Output } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { CreateUserDialogComponent } from '@app/users/create-user/create-user-dialog.component';
import { AppComponentBase } from '@shared/app-component-base';
import { finalize } from 'rxjs';
import { CreateOrEditMainHeadingInputDto, MainHeadServiceProxy } from './../../../shared/service-proxies/service-proxies';
import { FormControl, FormGroup, Validators } from '@angular/forms';

export interface DialogData {
  accountTypeId: number;
  lastEntry: string;
}

@Component({
  selector: 'app-create-main-head',
  templateUrl: './create-main-head.component.html',
  styleUrls: ['./create-main-head.component.scss']
})
export class CreateMainHeadComponent extends AppComponentBase {
  @Output() onSave = new EventEmitter<any>();

  createMainHeadForm: FormGroup;

  createMainHead = new CreateOrEditMainHeadingInputDto();
  constructor(
    injector: Injector,
    public _mainHeadServiceProxy: MainHeadServiceProxy,
    public _dialogRef: MatDialogRef<CreateOrEditMainHeadingInputDto>,
    @Inject(MAT_DIALOG_DATA) public data: DialogData
  ) {
    super(injector);
  }

  ngOnInit(): void{
    this.createMainHeadForm = new FormGroup({
      mainHeadName: new FormControl('', [Validators.required])
    });
  }

  show() {
    this.createMainHead = new CreateOrEditMainHeadingInputDto();
  }

  save() {
    this.createMainHead.accountTypeId = this.data.accountTypeId;
    this._mainHeadServiceProxy.createOrEditMainHead(this.createMainHead).pipe(finalize(() => {
    })).subscribe((result) => {
      this.notify.info(this.l('Acoount type Added Successfully'));
      this.hideDialog();
    });
  }
  
  hideDialog() {
    this._dialogRef.close();
  }
}
