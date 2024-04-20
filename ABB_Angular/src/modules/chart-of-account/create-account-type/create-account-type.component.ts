import { Component, EventEmitter, Inject, Injector, Input, OnInit, Output } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { CreateUserDialogComponent } from '@app/users/create-user/create-user-dialog.component';
import { AppComponentBase } from '@shared/app-component-base';
import { CreateOrEditAccountTypeInputDto, AccountTypeServiceProxy } from './../../../shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';
import { FormControl, FormGroup, Validators } from '@angular/forms';

export interface DialogData {
  natureId: number;
  lastEntry: string;
}

@Component({
  selector: 'app-create-account-type',
  templateUrl: './create-account-type.component.html',
  styleUrls: ['./create-account-type.component.scss']
})

export class CreateAccountTypeComponent extends AppComponentBase {
  @Output() onSave = new EventEmitter<any>();

  createAccountForm: FormGroup;

  createAccountType = new CreateOrEditAccountTypeInputDto();
  constructor(
    injector: Injector,
    public _accountTypeServiceProxy: AccountTypeServiceProxy,
    public _dialogRef: MatDialogRef<CreateAccountTypeComponent>,
    @Inject(MAT_DIALOG_DATA) public data: DialogData
  ) {
    super(injector);
  }

  ngOnInit(): void{
    this.createAccountForm = new FormGroup({
      typeName: new FormControl('', [Validators.required])
    });
  }

  show() {
    
    this.createAccountType = new CreateOrEditAccountTypeInputDto();
  }

  save() {
    this.createAccountType.accountNature = this.data.natureId;
    this._accountTypeServiceProxy.createOrEditAccountType(this.createAccountType).pipe(finalize(() => {
    })).subscribe((result) => {
      this.notify.info(this.l('Acoount Type Added Successfully'));
      this.hideDialog();
    });
  }
  
  hideDialog() {
    this._dialogRef.close();
  }
}
