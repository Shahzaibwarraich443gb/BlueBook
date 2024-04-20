import { GeneralEntityTypeDto, GeneralPaymentMethodDto, PaymentMethodServiceProxy } from "./../../shared/service-proxies/service-proxies";
import {  ElementRef, Renderer2 } from '@angular/core';

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

import { CreatePaymentMethodComponent } from "./create-Payment-Method/create-Payement-Method.component";
import { CreateEntityTypeComponent } from "modules/entity-type/create-entity-type/create-entity-type.component";

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
  selector: "app-Payment-Method",
  templateUrl: "./Payment-Method.component.html",
  styleUrls: ["./Payment-Method.component.css"],
})
export class PaymentMethodComponent extends AppComponentBase implements OnInit {
  totalRecords = 0;
  
generalPayementMethodDto:GeneralPaymentMethodDto[]=[];
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
    public _paymentService: PaymentMethodServiceProxy,
    
  ) {
    super(injector);
  }

  
  ngOnInit() {
    this.list();
  }

  list() {
    this._paymentService
      .getAll()
      .subscribe((arg) => (this.generalPayementMethodDto = arg));
  }

  protected delete(item: GeneralPaymentMethodDto): void { 
    abp.message.confirm(
      this.l("Are you sure to delete  entity type"),
      undefined,
      (result: boolean) => {
        if (result) {
          this._paymentService
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
      const temp = this.generalPayementMethodDto.filter(function (d) {
        return d.name.toLowerCase().indexOf(val) !== -1;
      });
      this.generalPayementMethodDto = temp;
    }
  }

  editGeneralPaymentMethodDto(PaymentMethodDto: GeneralPaymentMethodDto): void {
    this.showCreateOrEditGeneralPaymentMethodDialog(PaymentMethodDto.id);
  }

  createPaymentMethod(id?: number): void {
    this.showCreateOrEditGeneralPaymentMethodDialog(id);
  }

  private showCreateOrEditGeneralPaymentMethodDialog(id?: number): void {
    const dialogRef = this._dialog.open(CreatePaymentMethodComponent, {
      data: { id: id },
    });

    
    dialogRef.afterClosed().subscribe((result) => {
      dialogRef.close();
      this.list();
    });
  }

}
