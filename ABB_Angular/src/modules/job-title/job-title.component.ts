import { GeneralEntityTypeDto, JobTitleDto, JobTitleServiceProxy } from "./../../shared/service-proxies/service-proxies";

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
import { CreateJoTitleComponent } from "./create-job-title/create-job-title.component";

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
  selector: "app-job-title",
  templateUrl: "./job-title.component.html",
  styleUrls: ["./job-title.component.css"],
})
export class JobTitleComponent extends AppComponentBase implements OnInit {
  totalRecords = 0;
  jobTitleDto: JobTitleDto[] = [];
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
    public _jobTitleServiceProxy: JobTitleServiceProxy
  ) {
    super(injector);
  }

  ngOnInit() {
    this.list();
  }
  list() {
    this._jobTitleServiceProxy
      .getAll()
      .subscribe((arg) => (this.jobTitleDto = arg));
  }

  protected delete(languageDto: ContactPersonTypeDto): void {
    abp.message.confirm(
      this.l("Are you sure to delete  job title"),
      undefined,
      (result: boolean) => {
        if (result) {
          this._jobTitleServiceProxy
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
      const temp = this.jobTitleDto.filter(function (d) {
        return d.name.toLowerCase().indexOf(val) !== -1;
      });
      this.jobTitleDto = temp;
    }
  }

  editJobTitle(jobTitleDto: JobTitleDto): void {
    this.showCreateOrEditJobTitleDialog(jobTitleDto.id);
  }

  createJobTitle(id?: number): void {
    this.showCreateOrEditJobTitleDialog(id);
  }

  private showCreateOrEditJobTitleDialog(id?: number): void {
    const dialogRef = this._dialog.open(CreateJoTitleComponent, {
      data: { id: id },
    });

    dialogRef.afterClosed().subscribe((result) => {
      dialogRef.close();
      this.list();
    });
  }
}
