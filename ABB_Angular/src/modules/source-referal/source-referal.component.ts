import { SalesPersonTypeServiceProxy, SourceReferralTypeDto, SourceReferralTypeServiceProxy } from "./../../shared/service-proxies/service-proxies";

import {
  ContactPersonTypeDto,
  EntityDto,
  ContactPersonTypeServiceProxy,
  LanguageServiceProxy,
  LanguageDto,
  SalesPersonTypeDto,
} from "../../shared/service-proxies/service-proxies";
import { Component, Injector, OnInit } from "@angular/core";
import { ThemePalette } from "@angular/material/core";
import { MatDialog } from "@angular/material/dialog";
import { PagedRequestDto } from "@shared/paged-listing-component-base";

import { AppComponentBase } from "shared/app-component-base"; 
import { CreateSourecReferalComponent } from "./create-source-referal/create-source-referal.component";

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
  selector: "app-source-referal",
  templateUrl: "./source-referal.component.html",
  styleUrls: ["./source-referal.component.css"],
})
export class SourceReferalComponent extends AppComponentBase implements OnInit {
  displayedColumns: string[] = ["name", "customerName"];
  totalRecords = 0;
  sourceReferralTypeDto: SourceReferralTypeDto[] = [];
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
    // this.list(req, event.pageNo, undefined);
    console.log("user", event);
  }

  constructor(
    injector: Injector,
    public _dialog: MatDialog,
    public _sourceReferralTypeService: SourceReferralTypeServiceProxy
  ) {
    super(injector);
  }

  ngOnInit() {
    this.list();
  }
  protected list() {
     ;
    this._sourceReferralTypeService
      .getAll()
      .subscribe((arg) => (this.sourceReferralTypeDto = arg
       
        
        ));
        console.log(this.sourceReferralTypeDto);
  }

  protected delete(sourceReferralTypeDto: SourceReferralTypeDto): void {
    ;
    abp.message.confirm(
      this.l("Are you sure to delete  source"),
      undefined,
      (result: boolean) => {
        if (result) {
          this._sourceReferralTypeService
            .delete(sourceReferralTypeDto.id)
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
      const temp = this.sourceReferralTypeDto.filter(function (d) {
        return d.name.toLowerCase().indexOf(val) !== -1;

        //    || d.packageName.toLowerCase().indexOf(val) !== -1 ||
        // d.dishName.toLowerCase().indexOf(val) !== -1 || !val
      });
      this.sourceReferralTypeDto = temp;
    }
  }

  editSourceReferral(sourceReferralTypeDto: SourceReferralTypeDto): void {
    this.showCreateOrEditSourceReferralDialog(sourceReferralTypeDto.id);
  }
  createSourceReferral(id?: number): void {
    this.showCreateOrEditSourceReferralDialog(id);
  }
  private showCreateOrEditSourceReferralDialog(id?: number): void {
    const dialogRef = this._dialog.open(CreateSourecReferalComponent, {
      data: { id: id },
    });

    dialogRef.afterClosed().subscribe((result) => {
      dialogRef.close();
      this.list();
    });
  }
}
