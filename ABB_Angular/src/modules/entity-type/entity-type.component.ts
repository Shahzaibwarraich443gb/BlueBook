import { GeneralEntityTypeDto } from "./../../shared/service-proxies/service-proxies";

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
import { CreateEntityTypeComponent } from "./create-entity-type/create-entity-type.component";
class PagedUsersRequestDto extends PagedRequestDto {
  keyword: string;
  isActive: boolean | null;
}

export interface Task {
  name: string;
  completed: boolean;
  color: ThemePalette;
  subtasks?: Task[];
}
@Component({
  selector: "app-entity-type",
  templateUrl: "./entity-type.component.html",
  styleUrls: ["./entity-type.component.css"],
})
export class EntityTypeComponent extends AppComponentBase implements OnInit {
  totalRecords = 0;
  generalEntityTypeDto: GeneralEntityTypeDto[] = [];
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
    public _generalEntityTypeServiceProxy: GeneralEntityTypeServiceProxy
  ) {
    super(injector);
  }

  ngOnInit() {
    this.list();
  }

  list() {
    this._generalEntityTypeServiceProxy
      .getAll()
      .subscribe((arg) => (this.generalEntityTypeDto = arg));
  }

  protected delete(languageDto: ContactPersonTypeDto): void { 
    abp.message.confirm(
      this.l("Are you sure to delete  entity type"),
      undefined,
      (result: boolean) => {
        if (result) {
          this._generalEntityTypeServiceProxy
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
      const temp = this.generalEntityTypeDto.filter(function (d) {
        return d.name.toLowerCase().indexOf(val) !== -1;
      });
      this.generalEntityTypeDto = temp;
    }
  }

  editGeneralEntityType(languageDto: LanguageDto): void {
    this.showCreateOrEditGeneralEntityTypeDialog(languageDto.id);
  }

  createGeneralEntityType(id?: number): void {
    this.showCreateOrEditGeneralEntityTypeDialog(id);
  }

  private showCreateOrEditGeneralEntityTypeDialog(id?: number): void {
    const dialogRef = this._dialog.open(CreateEntityTypeComponent, {
      data: { id: id },
    });
    dialogRef.afterClosed().subscribe((result) => {
      dialogRef.close();
      this.list();
    });
  }

}
