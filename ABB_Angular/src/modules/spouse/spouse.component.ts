import { SalesPersonTypeServiceProxy, SourceReferralTypeDto, SourceReferralTypeServiceProxy, SpouseDto, SpouseServiceProxy } from "./../../shared/service-proxies/service-proxies";

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
import { CreateSpouseComponent } from "./create-spouse/create-spouse.component";
 
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
  selector: "app-spouse",
  templateUrl: "./spouse.component.html",
  styleUrls: ["./spouse.component.css"],
})
export class  SpouseComponent extends AppComponentBase implements OnInit { 
  totalRecords = 0;
  spouseDto: SpouseDto[] = [];
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
    public _spouseServiceProxy: SpouseServiceProxy
  ) {
    super(injector);
  }

  ngOnInit() {
    this.list();
  }
  protected list() { 
    this._spouseServiceProxy
      .getAll()
      .subscribe((arg) => (this.spouseDto = arg));
      console.log(this.spouseDto)
  }

  protected delete( spouseDto: SpouseDto): void { 
    abp.message.confirm(
      this.l("Are you sure to delete  spouse"),
      undefined,
      (result: boolean) => {
        if (result) {
          this._spouseServiceProxy
            .delete(spouseDto.id)
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
      const temp = this.spouseDto.filter(function (d) {
        return d.firstName.toLowerCase().indexOf(val) !== -1; 
      });
      this.spouseDto = temp;
    }
  }

  editSpouse( spouseDto: SpouseDto): void {
    this.showCreateOrEditSpouseDialog(spouseDto .id);
  }
  createSpouse(id?: number): void {
    this.showCreateOrEditSpouseDialog(id);
  }
  private showCreateOrEditSpouseDialog(id?: number): void {
    const dialogRef = this._dialog.open(CreateSpouseComponent, {
      data: { id: id },
    });
    dialogRef.afterClosed().subscribe((result) => {
      dialogRef.close();
      this.list();
    });
  }
}
