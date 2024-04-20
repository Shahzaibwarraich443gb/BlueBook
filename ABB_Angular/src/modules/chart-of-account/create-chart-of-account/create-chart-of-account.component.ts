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
import {
  CreateOrEditChartOfAccountInputDto,
  ChartOfAccountsServiceProxy,
  AccountTypeDto,
  MainHeadDto,
  CreateOrEditBankDto,
  CreateOrEditAddressDto,
  CreateOrEditBankAddressDto,
  BankServiceProxy
} from "@shared/service-proxies/service-proxies";
import { finalize } from "rxjs";
import { CreateAccountTypeComponent } from "./../create-account-type/create-account-type.component";
import { CreateMainHeadComponent } from "./../create-main-head/create-main-head.component";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { NgxSpinnerService } from "ngx-spinner";

export interface DialogData {
  id: number;
  mode: string;
  setAssets: string;
}

@Component({
  selector: "app-create-chart-of-account",
  templateUrl: "./create-chart-of-account.component.html",
  styleUrls: ["./create-chart-of-account.component.scss"],
  animations: [appModuleAnimation()],
})
export class CreateChartOfAccountComponent
  extends AppComponentBase
  implements OnInit {
  @ViewChild("createAccountType", { static: true })
  createAccountType: CreateAccountTypeComponent;
  @ViewChild("createMainHead", { static: true })
  createMainHead: CreateMainHeadComponent;
  lastEntry: number = 0;
  onlyAssets: boolean = false;
  showBankAdd: boolean = false;
  disableFields: boolean = false;
  createCoaForm: FormGroup;
  addBankForm: FormGroup;
  mainHeadIdCurrent: number = 0;
  createBankDto: CreateOrEditBankDto = new CreateOrEditBankDto();
  @Output() onSave = new EventEmitter<any>();

  createOrEditChartOfAccountInputDto = new CreateOrEditChartOfAccountInputDto();
  accountTypeDto: AccountTypeDto[] = [];
  mainHeadDto: MainHeadDto[] = [];
  constructor(
    injector: Injector,
    public dialog: MatDialog,
    public dialogRef: MatDialogRef<any>,
    public _chartOfAccountAppService: ChartOfAccountsServiceProxy,
    private _bankService: BankServiceProxy,
    private spinner: NgxSpinnerService,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    super(injector);
  }

  ngOnInit() {
    this.createOrEditChartOfAccountInputDto.accountNature = 1;

    if (this.data.id) {
      this.spinner.show();
      this._chartOfAccountAppService
        .getChartOfAccounts(this.data.id)
        .pipe(finalize(() => { }))
        .subscribe((result) => {
          this.createOrEditChartOfAccountInputDto.accountTypeId =
            result.accountTypeId;
          this.createOrEditChartOfAccountInputDto.accountDescription =
            result.accountDescription;
          this.createOrEditChartOfAccountInputDto.id = result.id;
          this.createCoaForm.controls.accountNature.setValue(result.accountNatureId);
          this.getAccountType(result.accountTypeId);
          this.mainHeadIdCurrent = result.mainHeadId;
          this.createCoaForm.controls.subHeadName.setValue(result.accountDescription);
        });
    }
    else {
      this.createOrEditChartOfAccountInputDto =
        new CreateOrEditChartOfAccountInputDto();
    }

    this.createCoaForm = new FormGroup({
      accountNature: new FormControl('', [Validators.required]),
      accountType: new FormControl('', [Validators.required]),
      mainHead: new FormControl('', [Validators.required]),
      subHeadName: new FormControl('', [Validators.required]),
      openingBalance: new FormControl(''),
      date: new FormControl(new Date(), Validators.required)
    });

    if (this.data.mode && this.data.mode == "bank") {
      this.createCoaForm.controls.accountNature.setValue(1);
      this.createCoaForm.controls.accountNature.disable();
      this.createCoaForm.controls.accountType.disable();
      this.createOrEditChartOfAccountInputDto.accountNature = 1;
      this.getAccountType(1);
    }



    this.addBankForm = new FormGroup({
      accountTitle: new FormControl('', [Validators.required]),
      bankName: new FormControl('', [Validators.required]),
      code: new FormControl('', [Validators.required]),
      accountNo: new FormControl('', [Validators.required]),
      routingNo: new FormControl('', [Validators.required]),
      startingChequeNo: new FormControl('', [Validators.required]),
      openBalance: new FormControl('', [Validators.required]),
      address: new FormControl('', [Validators.required]),
      state: new FormControl('', [Validators.required]),
      city: new FormControl('', [Validators.required]),
      country: new FormControl('', [Validators.required]),
      postCode: new FormControl('', [Validators.required]),
    });

    if (this.data.setAssets === "fixed") {
      this.createCoaForm.get("accountNature").setValue(1);
      this.createOrEditChartOfAccountInputDto.accountNature = 1;
      this.getAccountType(1);
      this.onlyAssets = true;
      this.createCoaForm.controls.accountNature.disable();
      this.createCoaForm.controls.accountType.disable();
    }

  }

  onAccountTypeChange(): void {
    if (this.data.mode && this.data.mode == "bank" || this.data.setAssets == "fixed") {
      this.getMainHead(this.data.mode && this.data.mode == "bank" || this.data.setAssets == "fixed" ? 1 : null);
    }
    else{
      this.getMainHead();
    }
    this.showBankAdd = this.createCoaForm.controls.accountNature.value == 1 && this.createCoaForm.controls.accountType.value == 1;
  }

  addMainHead() {
    if (!this.createOrEditChartOfAccountInputDto.accountTypeId) {
      this.message.error("Please select account type first!");
      return;
    }

    this._chartOfAccountAppService.getLastMainHeadByAccountType(this.createOrEditChartOfAccountInputDto.accountTypeId).subscribe((lastEntry) => {

      if (this.createOrEditChartOfAccountInputDto.accountTypeId) {
        const dialogRef = this.dialog.open(CreateMainHeadComponent, {
          data: {
            accountTypeId: this.createOrEditChartOfAccountInputDto.accountTypeId,
            lastEntry: lastEntry > 0 ? lastEntry.toString() : ""
          },
        });
        dialogRef.afterClosed().subscribe((result) => {
          this.getMainHead();
        });
      } else {
        this.message.error("Please select account type first!");
      }

    });


  }



  addAccountType() {



    this._chartOfAccountAppService.getLastAccountTypeCodeByNature(this.createOrEditChartOfAccountInputDto.accountNature).subscribe((lastEntry) => {

      if (this.createOrEditChartOfAccountInputDto.accountNature != null) {
        const dialogRef = this.dialog.open(CreateAccountTypeComponent, {
          data: {
            natureId: this.createOrEditChartOfAccountInputDto.accountNature,
            lastEntry: lastEntry > 0 ? lastEntry.toString() : ""
          }
        });
        dialogRef.afterClosed().subscribe((result) => {
          this.getAccountType();
        });
      } else {
        this.message.error("Please select nature of account first!");
      }

    });

  }


  checkForm(): boolean {
    if (this.createCoaForm.valid && this.addBankForm.valid) {
      return false;
    }
    else {
      return true;
    }
  }

  getAccountType(accountTypeId = null) {

    this.accountTypeDto = [];

    if (this.createOrEditChartOfAccountInputDto.accountNature != null) {
      this._chartOfAccountAppService
        .getAllAccountTypeByAccountNature(
          this.createOrEditChartOfAccountInputDto.accountNature
        )
        .pipe(finalize(() => { }))
        .subscribe((result) => {
          this.accountTypeDto = result;
          this.createOrEditChartOfAccountInputDto.accountTypeId = accountTypeId;
          this.mainHeadDto = [];
          this.createCoaForm.controls.accountType.setValue(accountTypeId);
          this.getMainHead(this.mainHeadIdCurrent);
        });

    }
  }

  getMainHead(mainHeadId = null) {
    ;
    if (this.createOrEditChartOfAccountInputDto.accountTypeId)
      this._chartOfAccountAppService
        .gatAllMainHeadByAccountType(
          this.createOrEditChartOfAccountInputDto.accountTypeId
        )
        .pipe(finalize(() => { }))
        .subscribe((result) => {
          this.mainHeadDto = result;
          this.createOrEditChartOfAccountInputDto.mainHeadId = mainHeadId;
          if (this.data.id && this.data.id > 0) {
            this.createCoaForm.controls.accountNature.disable();
            this.createCoaForm.controls.accountType.disable();
            this.createCoaForm.controls.mainHead.disable();
          }
          this.spinner.hide();
        });
  }

  save() {
    this._chartOfAccountAppService
      .createOrEditChartOfAccount(this.createOrEditChartOfAccountInputDto)
      .subscribe((arg) => {

        if (this.showBankAdd) {

          this.createBankDto.titleofAccount = this.addBankForm.controls.accountTitle.value,
            this.createBankDto.bankName = this.addBankForm.controls.bankName.value,
            this.createBankDto.swiftCode = this.addBankForm.controls.code.value,
            this.createBankDto.accountNumber = this.addBankForm.controls.accountNo.value,
            this.createBankDto.routing = this.addBankForm.controls.routingNo.value,
            this.createBankDto.startingCheque = this.addBankForm.controls.startingChequeNo.value,
            this.createBankDto.openBalance = this.addBankForm.controls.openBalance.value,
            this.createBankDto.address = new CreateOrEditBankAddressDto();
          this.createBankDto.address.address = this.addBankForm.controls.address.value,
            this.createBankDto.address.city = this.addBankForm.controls.city.value,
            this.createBankDto.address.state = this.addBankForm.controls.state.value,
            this.createBankDto.address.country = this.addBankForm.controls.country.value,
            this.createBankDto.address.postCode = this.addBankForm.controls.postCode.value,
            this.createBankDto.coaMainHeadId = this.createCoaForm.controls.mainHead.value,
            this._bankService.createOrEdit(this.createBankDto).subscribe((res) => {
              this.dialogRef.close();
              this.notify.info(this.l("SavedSuccessfully"));
              this.onSave.emit();
            });
        }
        else {
          this.dialogRef.close();
          this.notify.info(this.l("SavedSuccessfully"));
          this.onSave.emit();
        }
      });
  }

  

  hideDialog() {
    this.onSave.emit();
    this.dialogRef.close();
  }
}
