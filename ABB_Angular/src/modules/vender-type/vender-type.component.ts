import { GeneralEntityTypeDto, VenderTypeDto, VenderTypeServiceProxy } from "./../../shared/service-proxies/service-proxies";

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
import { CreateorEditVenderTypeComponent } from "./create-or-edit-vender-type/create-or-edit-vender-type.component";
 
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
  selector: "app-vender-type",
  templateUrl: "./vender-type.component.html",
  styleUrls: ["./vender-type.component.css"],
})
export class VenderTypeComponent extends AppComponentBase implements OnInit {
  totalRecords = 0;
  venderTypeDto: VenderTypeDto[] = [];
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
    public _venderTypeServiceProxy: VenderTypeServiceProxy
  ) {
    super(injector);
  }

  ngOnInit() {
    this.list();
  }

  list() {
    this._venderTypeServiceProxy
      .getAll()
      .subscribe((arg) => (this.venderTypeDto = arg));
  }

  protected delete(venderTypeDto: VenderTypeDto): void { 
    abp.message.confirm(
      this.l("Are you sure to delete  entity type"),
      undefined,
      (result: boolean) => {
        if (result) {
          this._venderTypeServiceProxy
            .delete(venderTypeDto.id)
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
      const temp = this.venderTypeDto.filter(function (d) {
        return d.name.toLowerCase().indexOf(val) !== -1;
      });
      this.venderTypeDto = temp;
    }
  }

  editVenderType(languageDto: LanguageDto): void {
    this.showCreateOrEditVenderTypeDialog(languageDto.id);
  }

  createVenderTypeDto(id?: number): void {
    this.showCreateOrEditVenderTypeDialog(id);
  }

  private showCreateOrEditVenderTypeDialog(id?: number): void {
    const dialogRef = this._dialog.open(CreateorEditVenderTypeComponent, {
      data: { id: id },
    }); 
    dialogRef.afterClosed().subscribe((result) => {
      dialogRef.close();
      this.list();
    });
  }
}
