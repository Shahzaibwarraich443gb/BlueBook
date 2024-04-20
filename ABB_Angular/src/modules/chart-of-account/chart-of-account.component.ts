import { Component, Injector, OnInit } from "@angular/core";
import { ThemePalette } from "@angular/material/core";
import { MatDialog } from "@angular/material/dialog";
import { PagedRequestDto } from "@shared/paged-listing-component-base";
import {
  AccountNature,
  ChartOfAccountDto,
  ChartOfAccountsServiceProxy,
  EntityDto,
} from "@shared/service-proxies/service-proxies";
import { AppComponentBase } from "shared/app-component-base";
import { CreateChartOfAccountComponent } from "./create-chart-of-account/create-chart-of-account.component";
import { MatTreeFlatDataSource, MatTreeFlattener } from '@angular/material/tree';
import { FlatTreeControl } from '@angular/cdk/tree';
import { NgxSpinnerService } from "ngx-spinner";
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

interface TreeFlatNode {
  id: number;
  expandable: boolean;
  account: string,
  type: string,
  nature: string,
  balance: string,
  action: string,
  level: number
}

interface treeDataCoa {
  id: number,
  mainHeadId: number,
  account: string,
  type: string,
  nature: string,
  balance: string,
  actionData: ChartOfAccountDto,
}

interface treeNode {
  data: any
  children: any[]
}


@Component({
  selector: "app-chart-of-account",
  templateUrl: "./chart-of-account.component.html",
  styleUrls: ["./chart-of-account.component.css"],
})
export class ChartOfAccountComponent extends AppComponentBase implements OnInit {
  coaCols: any[] = [
    { field: 'account', header: "Account" },
    { field: 'accountType', header: "Type" },
    { field: 'nature', header: "Nature" },
    { field: 'balance', header: "Balance" },
    { field: 'actionData', header: "Action" }
  ];
  totalRecords = 0;
  chartOfAccountDto: ChartOfAccountDto[] = [];
  natureOfAccount: AccountNature;
  allComplete: boolean = false;
  filteredDataSource: any;
  treeDataArr: treeNode[] = [];
  cols: any[] = [];
  pageSize = 10; // Number of items to show per page
  currentPage = 1; // Current page number
  totalItems: number; // Total number of items

  treeData: any[] = [];

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
    // this.list(req, event.pageNo, undefined);
  }

  constructor(
    injector: Injector,
    public _dialog: MatDialog,
    public _chartOfAccountsServiceProxy: ChartOfAccountsServiceProxy,
    public _spinnerService: NgxSpinnerService
  ) {
    super(injector);
  }

  ngOnInit() {
    this.list();

  }

  protected list() {
    this._spinnerService.show();
    this._chartOfAccountsServiceProxy
      .getChartOfAccountList()
      .subscribe((arg) => {
        this.chartOfAccountDto = arg;
        this.createTree();
        this._spinnerService.hide();
      });
  }


  createChartOfAccount(id?: number): void {
    this.showCreateOrEditChartOfAccountDialog(id);
  }

  protected delete(chartOfAccount: ChartOfAccountDto): void {
    ;
    abp.message.confirm(
      this.l("Are you sure to delete chart of account"),
      undefined,
      (result: boolean) => {
        if (result) {
          this._chartOfAccountsServiceProxy
            .deletChartOfAccounts(new EntityDto({ id: chartOfAccount.id }))
            .subscribe((res) => {
              if (res != "Deleted Successfully") {
                abp.message.error(res, "Cannot Delete");
              }
              else {
                abp.notify.success(this.l("SuccessfullyDeleted"));
                this.list();
              }
            });
        }
      }
    );
  }

  editChartOfAccount(chartOfAccount: ChartOfAccountDto): void {
    this.showCreateOrEditChartOfAccountDialog(chartOfAccount.id);
  }

  private showCreateOrEditChartOfAccountDialog(id?: number): void {
    const dialogRef = this._dialog.open(CreateChartOfAccountComponent, {
      data: { id: id },
    });

    dialogRef.afterClosed().subscribe((result) => {
      dialogRef.close();
      this.list();
    });
  }


  search(event: any) {
    const val = event.target.value.toLowerCase();
    if (val == "") {
      this.list();
    } else {
      const temp = this.chartOfAccountDto.filter(function (d) {
        return d.accountDescription.toLowerCase().indexOf(val) !== -1;

        //    || d.packageName.toLowerCase().indexOf(val) !== -1 ||
        // d.dishName.toLowerCase().indexOf(val) !== -1 || !val
      });
      this.chartOfAccountDto = temp;
    }
  }


  createTree(): void {
    this.treeDataArr = [];
    for (var data of this.chartOfAccountDto) {

      if (!this.treeDataArr.some(x => x.data.mainHeadId == data.mainHeadId)) {
        let treeObj: treeNode = {
          data: {
            id: 0,
            mainHeadId: data.mainHeadId,
            account: `${data.accountNatureId.toString().length > 1 ? data.accountNatureId.toString() : "0" + data.accountNatureId.toString()}-${data.accountTypeCode.toString().length > 1 ? data.accountTypeCode.toString() : "0" + data.accountTypeCode.toString()}-${data.mainHeadCode.toString().length > 1 ? data.mainHeadCode.toString() : "0" + data.mainHeadCode.toString()} ${data.mainHead}`,
            type: data.accountType,
            nature: data.accountNature,
            balance: '',
            actionData: data,
            level: 0
          },
          children: []
        }
        for (var subHeadData of this.chartOfAccountDto.filter(x => x.mainHeadId == data.mainHeadId)) {
          let childTreeObj: treeNode = {
            data: {
              id: subHeadData.id,
              mainHeadId: data.mainHeadId,
              account: `${data.accountNatureId.toString().length > 1 ? data.accountNatureId.toString() : "0" + data.accountNatureId.toString()}-${data.accountTypeCode.toString().length > 1 ? data.accountTypeCode.toString() : "0" + data.accountTypeCode.toString()}-${data.mainHeadCode.toString().length > 1 ? data.mainHeadCode.toString() : "0" + data.mainHeadCode.toString()}-${subHeadData.accountCode.toString().length > 1 ? subHeadData.accountCode.toString() : "0" + subHeadData.accountCode.toString()} ${subHeadData.accountDescription}`,
              type: data.accountType,
              nature: data.accountNature,
              balance: '$' + this.chartOfAccountDto.find(x => x.id == subHeadData.id).balance,
              actionData: subHeadData,
              level: 1
            },
            children: []
          }

          treeObj.children.push(childTreeObj);
        }

        this.treeDataArr.push(treeObj);
      }
    }

  }


}
