import { SalesPersonTypeServiceProxy } from './../../shared/service-proxies/service-proxies';

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
import { CreateSalePersonComponent } from './create-sale-person/create-sale-person.component';

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
  selector: "app-sale-person",
  templateUrl: "./sale-person.component.html",
  styleUrls: ["./sale-person.component.css"],
})
export class SalePersonComponent extends AppComponentBase implements OnInit {
  displayedColumns: string[] = ["name", "customerName"];
  totalRecords = 0;
  salesPersonTypeDto: SalesPersonTypeDto[] = [];
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
    public _salesPersonTypeServiceProxy: SalesPersonTypeServiceProxy
  ) {
    super(injector);
  }

  ngOnInit() {
    this.list();
  }
  protected list() {
    this._salesPersonTypeServiceProxy
      .getAll()
      .subscribe((arg) => (this.salesPersonTypeDto = arg));
  }

  protected delete(languageDto: ContactPersonTypeDto): void {
    ;
    abp.message.confirm(
      this.l("Are you sure to delete  sale person"),
      undefined,
      (result: boolean) => {
        if (result) {
          this._salesPersonTypeServiceProxy
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
      const temp = this.salesPersonTypeDto.filter(function (d) {
        return d.name.toLowerCase().indexOf(val) !== -1; 
      });
      this.salesPersonTypeDto = temp;
    }
  }

  editSalesPersonType(salesPersonTypeDto: SalesPersonTypeDto): void {
    this.showCreateOrEditSalesPersonTypeDialog(salesPersonTypeDto.id);
  }

  createSalesPersonType(id?: number): void {
    this.showCreateOrEditSalesPersonTypeDialog(id);
  }
  
  private showCreateOrEditSalesPersonTypeDialog(id?: number): void {
    const dialogRef = this._dialog.open(CreateSalePersonComponent, {
      data: { id: id },
    });

    dialogRef.afterClosed().subscribe((result) => {
      dialogRef.close();
      this.list();
    });
  }
}
