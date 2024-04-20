import { CompanyDto, CompanyServiceProxy, GeneralEntityTypeDto } from "./../../shared/service-proxies/service-proxies";

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
import { CreateChartOfAccountComponent } from "modules/chart-of-account/create-chart-of-account/create-chart-of-account.component";
import { CreateCompanyComponent } from "./create-company/create-company.component";
 
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
  selector: "app-company",
  templateUrl: "./company.component.html",
  styleUrls: ["./company.component.css"],
})
export class   CompanyComponent extends AppComponentBase implements OnInit {
  totalRecords = 0;
  companyDto:  CompanyDto[] = [];
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
  }

  constructor(
    injector: Injector,
    public _dialog: MatDialog,
    public _companyServiceProxy: CompanyServiceProxy
  ) {
    super(injector);
  }

  ngOnInit() {
    this.list();
  }

  list() {
    this._companyServiceProxy
      .getAll()
      .subscribe((arg) => (this.companyDto = arg));
  }

  protected delete(companyDto: CompanyDto): void {
    ;
    abp.message.confirm(
      this.l("Are you sure to delete company"),
      undefined,
      (result: boolean) => {
        if (result) {
          this._companyServiceProxy
            .delete(companyDto.id)
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
      const temp = this.companyDto.filter(function (d) {
        return d.name.toLowerCase().indexOf(val) !== -1;
      });
      this.companyDto = temp;
    }
  }

  editCompany( companyDto: CompanyDto): void {
    this.showCreateOrEditCompanyDialog(companyDto.id);
  }

  createCompany(id?: number): void {
    this.showCreateOrEditCompanyDialog(id);
  }
  
  private showCreateOrEditCompanyDialog(id?: number): void {
    const dialogRef = this._dialog.open(CreateCompanyComponent, {
      data: { id: id },
    });

    dialogRef.afterClosed().subscribe((result) => {
      dialogRef.close();
      this.list();
    });
  }

}
