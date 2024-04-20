import { ProductServiceDto, ProductServiceServiceProxy, SalesPersonTypeServiceProxy, SourceReferralTypeDto, SourceReferralTypeServiceProxy } from "./../../shared/service-proxies/service-proxies";

import { Component, Injector, OnInit } from "@angular/core";
import { ThemePalette } from "@angular/material/core";
import { MatDialog } from "@angular/material/dialog";
import { PagedRequestDto } from "@shared/paged-listing-component-base";

import { AppComponentBase } from "shared/app-component-base"; 
import { CreateOrEditProductServiceComponent } from "./create-or-edit-product-service/create-or-edit-product-service.component";
 
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
  selector: "app-product-service",
  templateUrl: "./product-service.component.html",
  styleUrls: ["./product-service.component.css"],
})
export class ProductServiceComponent extends AppComponentBase implements OnInit {
  displayedColumns: string[] = ["name", "customerName"];
  totalRecords = 0;
  productServiceDto: ProductServiceDto[] = [];
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
    public _productServiceServiceProxy: ProductServiceServiceProxy
  ) {
    super(injector);
  }

  ngOnInit() {
      this.list();
  }
  protected list() {
    this._productServiceServiceProxy
      .getAll()
      .subscribe((arg) => (this.productServiceDto = arg));
  }

  protected delete(productServiceDto: ProductServiceDto): void {
    ;
    abp.message.confirm(
      this.l("Are you sure to delete chart of account"),
      undefined,
      (result: boolean) => {
        if (result) {
          this._productServiceServiceProxy
            .delete(productServiceDto.id)
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
      const temp = this.productServiceDto.filter(function (d) {
        return d.name.toLowerCase().indexOf(val) !== -1;

        //    || d.packageName.toLowerCase().indexOf(val) !== -1 ||
        // d.dishName.toLowerCase().indexOf(val) !== -1 || !val
      });
      this.productServiceDto = temp;
    }
  }

  editProductService(productServiceDto: ProductServiceDto): void {
    this.showCreateOrEditProductServiceDialog(productServiceDto.id);
  }
  createProductService(id?: number): void {
    this.showCreateOrEditProductServiceDialog(id);
  }
  private showCreateOrEditProductServiceDialog(id?: number): void {
    const dialogRef = this._dialog.open(CreateOrEditProductServiceComponent, {
      data: { id: id },
      maxWidth: '90vw',
      width: '90vw'
    });

    dialogRef.afterClosed().subscribe((result) => {
      dialogRef.close();
      this.list();
    });
  }

 

}
