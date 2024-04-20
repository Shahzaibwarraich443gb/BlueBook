import { GeneralEntityTypeDto, GeneralPaymentMethodDto, GeneralUsersGroupDto, PaymentMethodServiceProxy, UserGroupsServiceProxy } from "./../../shared/service-proxies/service-proxies";
import { ElementRef, Renderer2 } from '@angular/core';

import {
  ContactPersonTypeDto,
  EntityDto,
  ContactPersonTypeServiceProxy,
  LanguageServiceProxy,
  // LanguageDto,

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


import { CreateEntityTypeComponent } from "modules/entity-type/create-entity-type/create-entity-type.component";
import { CreatePaymentMethodComponent } from "modules/Payement-Method/create-Payment-Method/create-Payement-Method.component";
import { CreateUsersGroupComponent } from "./create-Users-Group/create-Users-Group.component";

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
  selector: "app-Users-Group",
  templateUrl: "./Users-Group.component.html",
  styleUrls: ["./Users-Group.component.css"],
})
export class UsersGroupComponent extends AppComponentBase implements OnInit {
  totalRecords = 0;

  // gneralPayementMethodDto: GeneralPaymentMethodDto[] = [];e
  Dto: GeneralUsersGroupDto[] = [];
  allComplete: boolean = false;
  task: Task = {
    name: "",
    completed: false,
    color: "primary",
  };
  keyword = "";
  isActive: boolean | null;
  isPaymentSuccessful: boolean = true; // Set it based on your data

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
    // public _generalEntityTypeServiceProxy: GeneralEntityTypeServiceProxy,
    // public _paymentService: PaymentMethodServiceProxy,
    public _UserGroup: UserGroupsServiceProxy

  ) {
    super(injector);
  }


  ngOnInit() {
    this.list();
  }

  list() {
    this._UserGroup
      .getAll()
      .subscribe((arg) => (this.Dto = arg));
  }

  protected delete(item: GeneralUsersGroupDto): void {
    abp.message.confirm(
      this.l("Are you sure to delete "),
      undefined,
      (result: boolean) => {
        if (result) {
          this._UserGroup
            .delete(item.id)
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
      const temp = this.Dto.filter(function (d) {
        return d.name.toLowerCase().indexOf(val) !== -1;
      });
      this.Dto = temp;
    }
  }

  editGeneralUsergroupDto(dto: GeneralUsersGroupDto): void {
    this.showCreateOrEditGeneralUserGroupDialog(dto.id);
  }

  createusergroup(id?: number): void {
    this.showCreateOrEditGeneralUserGroupDialog(id);
  }

  private showCreateOrEditGeneralUserGroupDialog(id?: number): void {
    const dialogRef = this._dialog.open(CreateUsersGroupComponent, {
      data: { id: id },
    });


    dialogRef.afterClosed().subscribe((result) => {
      dialogRef.close();
      this.list();
    });
  }

}
