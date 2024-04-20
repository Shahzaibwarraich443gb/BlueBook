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
import { CreateUserDialogComponent } from "@app/users/create-user/create-user-dialog.component";
import { appModuleAnimation } from "@shared/animations/routerTransition";
import { AppComponentBase } from "@shared/app-component-base";
import {
  CreateOrEditChartOfAccountInputDto,
  ChartOfAccountsServiceProxy,
  AccountTypeDto,
  MainHeadDto,
  UserDtoPagedResultDto,
} from "@shared/service-proxies/service-proxies";
import { finalize } from "rxjs";
import { CreateAccountTypeComponent } from "./../create-account-type/create-account-type.component";
import { CreateMainHeadComponent } from "./../create-main-head/create-main-head.component";

export interface DialogData {
  id: number;
}

@Component({
  selector: "app-create-chart-of-account",
  templateUrl: "./create-chart-of-account.component.html",
  styleUrls: ["./create-chart-of-account.component.scss"],
  animations: [appModuleAnimation()],
})
export class CreateChartOfAccountComponent
  extends AppComponentBase
  implements OnInit
{
  @ViewChild("createAccountType", { static: true })
  createAccountType: CreateAccountTypeComponent;
  @ViewChild("createMainHead", { static: true })
  createMainHead: CreateMainHeadComponent;

  @Output() onSave = new EventEmitter<any>();

  createOrEditChartOfAccountInputDto = new CreateOrEditChartOfAccountInputDto();
  accountTypeDto: AccountTypeDto[] = [];
  mainHeadDto: MainHeadDto[] = [];
  constructor(
    injector: Injector,
    public dialog: MatDialog,
    public dialogRef: MatDialogRef<any>,
    public _chartOfAccountAppService: ChartOfAccountsServiceProxy,
    @Inject(MAT_DIALOG_DATA) public data: DialogData
  ) {
    super(injector);
  }

  ngOnInit() {
    this.createOrEditChartOfAccountInputDto.accountNature = 1;
    if (this.data.id) {
      ;
      this._chartOfAccountAppService
        .getChartOfAccounts(this.data.id)
        .pipe(finalize(() => {}))
        .subscribe((result) => {
          this.createOrEditChartOfAccountInputDto.accountTypeId =
            result.accountTypeId;
          this.createOrEditChartOfAccountInputDto.mainHeadId =
            result.mainHeadId;
          this.createOrEditChartOfAccountInputDto.accountDescription =
            result.accountDescription;
          this.createOrEditChartOfAccountInputDto.id = result.id;
          this.getMainHead();
          this.getAccountType();
        });
    } else
      this.createOrEditChartOfAccountInputDto =
        new CreateOrEditChartOfAccountInputDto();
  }

  addMainHead() {
    if (this.createOrEditChartOfAccountInputDto.accountTypeId) {
      const dialogRef = this.dialog.open(CreateMainHeadComponent, {
        data: {
          accountTypeId: this.createOrEditChartOfAccountInputDto.accountTypeId,
        },
      });
      dialogRef.afterClosed().subscribe((result) => {
        this.getMainHead();
      });
    } else {
      this.message.error("Please select account type first!");
    }
  }

  addAccountType() {
    if (this.createOrEditChartOfAccountInputDto.accountNature != null) {
      const dialogRef = this.dialog.open(CreateAccountTypeComponent, {
        data: {
          natureId: this.createOrEditChartOfAccountInputDto.accountNature,
        },
      });
      dialogRef.afterClosed().subscribe((result) => {
        this.getAccountType();
      });
    } else {
      this.message.error("Please select nature of account first!");
    }
  }

  getAccountType() {
    ;
    if (this.createOrEditChartOfAccountInputDto.accountNature != null)
      this._chartOfAccountAppService
        .getAllAccountTypeByAccountNature(
          this.createOrEditChartOfAccountInputDto.accountNature
        )
        .pipe(finalize(() => {}))
        .subscribe((result) => {
          this.accountTypeDto = result;
        });
  }

  getMainHead() {
    ;
    if (this.createOrEditChartOfAccountInputDto.accountTypeId)
      this._chartOfAccountAppService
        .gatAllMainHeadByAccountType(
          this.createOrEditChartOfAccountInputDto.accountTypeId
        )
        .pipe(finalize(() => {}))
        .subscribe((result) => {
          this.mainHeadDto = result;
        });
  }

  save() {
    this._chartOfAccountAppService
      .createOrEditChartOfAccount(this.createOrEditChartOfAccountInputDto)
      .subscribe((arg) => {
        this.dialogRef.close();
        this.notify.info(this.l("SavedSuccessfully"));
        this.onSave.emit();
      });
  }

  hideDialog() {
    this.onSave.emit();
    this.dialogRef.close();
  }
}
