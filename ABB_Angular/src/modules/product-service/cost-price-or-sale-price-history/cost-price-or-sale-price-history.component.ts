import { ChartOfAccountDto, CreateOrEditEmailInputDto, CreateOrEditProductServiceInputDto, ProductCategoryDto, ProductCategoryServiceProxy, ProductServiceDto, ProductServiceServiceProxy, SourceReferralTypeDto, SourceReferralTypeServiceProxy, VenderServiceProxy, VendorDto } from '../../../shared/service-proxies/service-proxies';
import { CreateOrEditSourceReferralTypeDto, Phone } from "../../../shared/service-proxies/service-proxies";
import { ValidationService } from "../../../shared/Services/validation.service";
import {
  CreateOrEditAddressDto,
  CreateOrEditPhoneDto,
  CreateOrEditSalesPersonTypeDto,
  LanguageServiceProxy,
  SalesPersonTypeServiceProxy,
} from "../../../shared/service-proxies/service-proxies";
import {
  Component,
  EventEmitter,
  Inject,
  Injector,
  OnInit,
  Output,
  ViewChild,
} from "@angular/core";
import {
  MatDialog,
  MatDialogRef,
  MAT_DIALOG_DATA,
} from "@angular/material/dialog";
import { appModuleAnimation } from "@shared/animations/routerTransition";
import { AppComponentBase } from "@shared/app-component-base";

import { finalize } from "rxjs";
import { CreateProductCategoryComponent } from 'modules/product-category/create-product-category/create-product-category.component';

export interface DialogData {
  data: any;
}

@Component({
  selector: "app-cost-price-or-sale-price-history.-service",
  templateUrl: "./cost-price-or-sale-price-history.component.html",
  styleUrls: ["./cost-price-or-sale-price-history.component.scss"],
  animations: [appModuleAnimation()],
})
export class CostPriceOrSalePriceHistoryComponent
  extends AppComponentBase
  implements OnInit {
  @Output() onSave = new EventEmitter<any>();
  productCategoryDto:  ProductCategoryDto[] = [];
  chartOfAccountExpenseDto: ChartOfAccountDto[] = [];
  chartOfAccountIncomeDto: ChartOfAccountDto[] = [];
   venderDto:  VendorDto[] = [];
  createOrEditProductServiceInputDto = new CreateOrEditProductServiceInputDto();
  productServiceDto: ProductServiceDto[] = [];
  
  email = new CreateOrEditEmailInputDto();
  constructor(
    injector: Injector,
    public dialog: MatDialog,
    public dialogRef: MatDialogRef<any>,
    public _sourceReferralTypeServiceProxy: SourceReferralTypeServiceProxy,
    @Inject(MAT_DIALOG_DATA) public data: DialogData,
    public _productCategoryServiceProxy: ProductCategoryServiceProxy,
    public _dialog: MatDialog,
    public _vendorServiceProxy: VenderServiceProxy,
    public validation: ValidationService,
    public _productServiceServiceProxy: ProductServiceServiceProxy,
  ) {
    super(injector);
  }
  customerDto: any[] = [];
  ngOnInit() {
    this.list();
 if(this.data.data == "SalePrice"){
  this.list();
 }
 else if(this.data.data == "CostPrice"){
  this.list();
 }
  }

  
 

  
  hideDialog() {
    this.onSave.emit();
    this.dialogRef.close();
  }

   
 
  protected list() {
    this._productServiceServiceProxy
      .getAll()
      .subscribe((arg) => (this.productServiceDto = arg));
  }

}
