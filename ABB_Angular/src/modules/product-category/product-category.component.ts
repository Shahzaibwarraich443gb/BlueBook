import { GeneralEntityTypeDto, JobTitleDto, JobTitleServiceProxy, ProductCategoryDto, ProductCategoryServiceProxy } from "./../../shared/service-proxies/service-proxies";

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
import { CreateProductCategoryComponent } from "./create-product-category/create-product-category.component";

 
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
  selector: "app-product-category",
  templateUrl: "./product-category.component.html",
  styleUrls: ["./product-category.component.css"],
})
export class ProductCategoryComponent extends AppComponentBase implements OnInit {
  totalRecords = 0;
  productCategoryDto:  ProductCategoryDto[] = [];
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
    public _productCategoryServiceProxy: ProductCategoryServiceProxy
  ) {
    super(injector);
  }

  ngOnInit() {
    this.list();
  }

  list() {
    this._productCategoryServiceProxy
      .getAll()
      .subscribe((arg) => (this.productCategoryDto = arg));
  }

  protected delete(productCategoryDto: ProductCategoryDto): void {
    abp.message.confirm(
      this.l("Are you sure to delete product category"),
      undefined,
      (result: boolean) => {
        if (result) {
          this._productCategoryServiceProxy
            .delete(productCategoryDto.id)
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
      const temp = this.productCategoryDto.filter(function (d) {
        return d.name.toLowerCase().indexOf(val) !== -1;
      });
      this.productCategoryDto = temp;
    }
  }

  editProductCategory( productCategoryDto: ProductCategoryDto): void {
    this.showCreateOrEditProductCategoryDialog(productCategoryDto.id);
  }

  createProductCategory(id?: number): void {
    this.showCreateOrEditProductCategoryDialog(id);
  }

  private showCreateOrEditProductCategoryDialog(id?: number): void {
    const dialogRef = this._dialog.open(CreateProductCategoryComponent, {
      data: { id: id },
    });

    dialogRef.afterClosed().subscribe((result) => {
      dialogRef.close();
      this.list();
    });
  }
  
}
