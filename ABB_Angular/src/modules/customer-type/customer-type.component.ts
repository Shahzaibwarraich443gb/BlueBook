import { CustomerTypeDto, CustomerTypesServiceProxy, GeneralEntityTypeDto, JobTitleDto, JobTitleServiceProxy } from "./../../shared/service-proxies/service-proxies";

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
import { CreateorEditCustomerTypeComponent } from "modules/customer-type/create-or-edit-customer-type/create-or-edit-customer-type.component";
import { AppComponentBase } from "shared/app-component-base";


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
  selector: "app-customer-type",
  templateUrl: "./customer-type.component.html",
  styleUrls: ["./customer-type.component.css"],
})
export class CustomerTypeComponent extends AppComponentBase implements OnInit {
  totalRecords = 0;
  customerTypeDto: CustomerTypeDto[] = [];
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
    public _customerTypesServiceProxy: CustomerTypesServiceProxy,
  ) {
    super(injector);
  }

  ngOnInit() {
    this.list();
  }

  list() {
    this._customerTypesServiceProxy
      .getAll()
      .subscribe((arg) => (this.customerTypeDto = arg));
  }

  protected delete(languageDto: ContactPersonTypeDto): void {
    abp.message.confirm(
      this.l("Are you sure to delete  job title"),
      undefined,
      (result: boolean) => {
        if (result) {
          this._customerTypesServiceProxy
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
      const temp = this.customerTypeDto.filter(function (d) {
        return d.name.toLowerCase().indexOf(val) !== -1;
      });
      this.customerTypeDto = temp;
    }
  }

  editCustomerType(customerType: JobTitleDto): void {
    this.showCreateOrEditCustomerTypeDialog(customerType.id);
  }

  createCustomerType(id?: number): void {
    this.showCreateOrEditCustomerTypeDialog(id);
  }

  private showCreateOrEditCustomerTypeDialog(id?: number): void {
    const dialogRef = this._dialog.open(CreateorEditCustomerTypeComponent, {
      data: { id: id },
    });
    dialogRef.afterClosed().subscribe((result) => {
      dialogRef.close();
      this.list();
    });
  }
}
