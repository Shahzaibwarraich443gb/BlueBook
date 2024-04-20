import { ValidationService } from './../../shared/Services/validation.service';
import { EthnicitiesServiceProxy, EthnicityDto } from './../../shared/service-proxies/service-proxies';

import { Component, Inject, Injector, OnInit } from "@angular/core";
import { ThemePalette } from "@angular/material/core";
import { MatDialog } from "@angular/material/dialog";
import { PagedRequestDto } from "@shared/paged-listing-component-base";
import {
  AccountNature,
  ChartOfAccountDto,
} from "@shared/service-proxies/service-proxies";
import { AppComponentBase } from "shared/app-component-base";
import { CreateChartOfAccountComponent } from "modules/chart-of-account/create-chart-of-account/create-chart-of-account.component";
import { CreateEthnicityComponent } from "./create-ethnicity/create-ethnicity.component";

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
  selector: "app-ethnicity",
  templateUrl: "./ethnicity.component.html",
  styleUrls: ["./ethnicity.component.css"],
})
export class EthnicityComponent extends AppComponentBase implements OnInit {
  totalRecords = 0;
  ethnicityDto: EthnicityDto[] = [];
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
     public validator:ValidationService,
    injector: Injector,
    public _dialog: MatDialog,
    public _ethnicityServiceProxy: EthnicitiesServiceProxy
  ) {
    super(injector);
  }

  ngOnInit() {
    this.list();
  }

  protected list() {
    this._ethnicityServiceProxy
      .getAll()
      .subscribe((arg) => (this.ethnicityDto = arg));
  }

  protected delete(ethnicityDto: EthnicityDto): void {
    abp.message.confirm(
      this.l("Are you sure to delete ethnicity"),
      undefined,
      (result: boolean) => {
        if (result) {
          this._ethnicityServiceProxy.delete(ethnicityDto.id).subscribe(() => {
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
      const temp = this.ethnicityDto.filter(function (d) {
        return d.name.toLowerCase().indexOf(val) !== -1;
      });
      this.ethnicityDto = temp;
    }
  }

  editEthnicity(ethnicityDto: EthnicityDto): void {
    this.showCreateOrEditEthnicityDialog(ethnicityDto.id);
  }
  createEthnicity(id?: number): void {
    this.showCreateOrEditEthnicityDialog(id);
  }
  private showCreateOrEditEthnicityDialog(id?: number): void {
    const dialogRef = this._dialog.open(CreateEthnicityComponent, {
      data: { id: id },
    });
    dialogRef.afterClosed().subscribe((result) => {
      dialogRef.close();
      this.list();
    });
  }
}
