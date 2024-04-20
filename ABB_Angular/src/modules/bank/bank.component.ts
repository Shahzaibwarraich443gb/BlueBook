import { BankDto, BankServiceProxy, GeneralEntityTypeDto, JobTitleDto, JobTitleServiceProxy } from "./../../shared/service-proxies/service-proxies";

import {
  ContactPersonTypeDto,
  EntityDto,
  ContactPersonTypeServiceProxy,
  LanguageServiceProxy,
  LanguageDto,
  GeneralEntityTypeServiceProxy,
} from "../../shared/service-proxies/service-proxies";
import { Component, Injector, OnInit } from "@angular/core";
import { ThemePalette } from "@angular/material/core";
import { MatDialog } from "@angular/material/dialog";
import { PagedRequestDto } from "@shared/paged-listing-component-base";
import {
  AccountNature,
  ChartOfAccountDto,
} from "@shared/service-proxies/service-proxies";
import { AppComponentBase } from "shared/app-component-base";
import { CreateOrEditBankComponent } from "./create-or-edit-bank/create-or-edit-bank.component";
import { CreateChartOfAccountComponent } from "modules/chart-of-account/create-chart-of-account/create-chart-of-account.component";

 
class PagedUsersRequestDto extends PagedRequestDto {
  keyword: string;
  isActive: boolean | null;
}

//for checkbox
export interface Task {
  name: string;
  completed: boolean;
  color: ThemePalette;
  subtasks?: Task[];
}
@Component({
  selector: "app-bank",
  templateUrl: "./bank.component.html",
  styleUrls: ["./bank.component.css"],
})
export class  BankComponent extends AppComponentBase implements OnInit {
  totalRecords = 0;
  bankDto: BankDto[] = [];
  allComplete: boolean = false;
  task: Task = {
    name: "",
    completed: false,
    color: "primary",
  };
  keyword = "";
  isActive: boolean | null;

  someComplete(): boolean {
    if (this.task.subtasks == null) {
      return false;
    }
    return (
      this.task.subtasks.filter((t) => t.completed).length > 0 &&
      !this.allComplete
    );
  }

  updateRows(event) {
    let req = new PagedUsersRequestDto();
    req.maxResultCount = event.maxResultCount;
    req.skipCount = event.skipCount;
    console.log("user", event);
  }

  constructor(
    injector: Injector,
    public _dialog: MatDialog,
    public _bankServiceProxy:  BankServiceProxy  ,
  ) {
    super(injector);
  }

  ngOnInit() {
    this.list();
  }
  
  list() {
    this._bankServiceProxy
      .getAll()
      .subscribe((arg) => (this.bankDto = arg));
  }

  protected delete(languageDto: ContactPersonTypeDto): void {
    ;
    abp.message.confirm(
      this.l("Are you sure to delete  job title"),
      undefined,
      (result: boolean) => {
        if (result) {
          this._bankServiceProxy
            .delete(languageDto.id)
            .subscribe(() => {
              abp.notify.success(this.l("SuccessfullyDeleted"));
              this.list();
            });
        }
      }
    );
  }

  search(event: any) {
    const val = event.target.value.toLowerCase();
    if (val == "") {
      this.list();
    } else {
      const temp = this.bankDto.filter(function (d) {
        return d.bankName.toLowerCase().indexOf(val) !== -1;
      });
      this.bankDto = temp;
    }
  }

  editBank(bankDto: JobTitleDto): void {
    this.showCreateOrEditBankDialog(bankDto.id);
  }
  
  createBank(id?: number): void {
    this.showCreateOrEditBankDialog(id);
  }

  private showCreateOrEditBankDialog(id?: number): void {
    const dialogRef = this._dialog.open(CreateChartOfAccountComponent, {
      data: { mode: "bank" },
    });

    dialogRef.afterClosed().subscribe((result) => {
      dialogRef.close();
      this.list();
    });
  }
}
