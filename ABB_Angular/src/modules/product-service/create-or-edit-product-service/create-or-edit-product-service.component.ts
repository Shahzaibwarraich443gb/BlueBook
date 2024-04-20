import { ChartOfAccountDto, ChartOfAccountsServiceProxy, CreateOrEditEmailInputDto, CreateOrEditProductServiceInputDto, ProductCategoryDto, ProductCategoryServiceProxy, ProductServiceServiceProxy, SourceReferralTypeServiceProxy, VenderServiceProxy, VendorDto } from '../../../shared/service-proxies/service-proxies';
import { ValidationService } from "../../../shared/Services/validation.service";
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
import { CostPriceOrSalePriceHistoryComponent } from '../cost-price-or-sale-price-history/cost-price-or-sale-price-history.component';
import { CreateOrEditVendorComponent } from 'modules/vendor/create-or-edit-vendor/create-or-edit-vendor.component';

export interface DialogData {
  id: number;
}

@Component({
  selector: "app-create-or-edit-product-service",
  templateUrl: "./create-or-edit-product-service.component.html",
  styleUrls: ["./create-or-edit-product-service.component.scss"],
  animations: [appModuleAnimation()],
})
export class CreateOrEditProductServiceComponent
  extends AppComponentBase
  implements OnInit {
  @Output() onSave = new EventEmitter<any>();
  @ViewChild('createOrEditSourecReferalForm') createOrEditSourecReferalForm: any;
  productCategoryDto: ProductCategoryDto[] = [];
  chartOfAccountExpenseDto: ChartOfAccountDto[] = [];
  chartOfAccountIncomeDto: ChartOfAccountDto[] = [];
  venderDto: VendorDto[] = [];
  advanceSaleTaxAccountArr: ChartOfAccountDto[] = [];
  liabilityAccountArr: ChartOfAccountDto[] = [];
  createOrEditProductServiceInputDto = new CreateOrEditProductServiceInputDto();

  salePriceActive: boolean = false;
  costPriceActive: boolean = false;
  expenseEntryActive: boolean = false;
  liabilityActive: boolean = false;
  advanceSaleTaxActive: boolean = false;

  expenseEntryHelpText: string = "By checking this box, a purchase invoice will automatically be generated in related expense and vendor account once sale invoice is created";


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
    private _chartOfAccountService: ChartOfAccountsServiceProxy
  ) {
    super(injector);
  }
  customerDto: any[] = [];
  async ngOnInit() {
    await this.ListMethodAll();

  }

  addVendor(): void {
    const dialogRef = this._dialog.open(CreateOrEditVendorComponent, {
      data: {},
    });

    dialogRef.afterClosed().subscribe((result: any) => {
      // Handle the close event here
      this.verderList();
      // Perform any necessary actions after dialog close
    });
  }


  onExpenseEntryChange(): void {
    this.expenseEntryActive = this.createOrEditProductServiceInputDto.automaticExpense;
  }

  checkRequired(key: string): void {
    if (key == "income") {
      this.salePriceActive = this.createOrEditProductServiceInputDto.salePrice ? true : false;
    }
    else if (key == "expense") {
      this.costPriceActive = this.createOrEditProductServiceInputDto.costPrice ? true : false;
    }
    else if (key == "advancedSaleTax") {
      this.advanceSaleTaxActive = this.createOrEditProductServiceInputDto.saleTax ? true : false;
    }
    else if (key == "liability") {
      this.liabilityActive = this.createOrEditProductServiceInputDto.expenseSaleTax ? true : false;
    }
  }

  save() {

    this._productServiceServiceProxy
      .createOrEdit(this.createOrEditProductServiceInputDto)
      .subscribe((arg) => {
        this.dialogRef.close();
        this.onSave.emit();
      });

  }

  async ListMethodAll() {
    await this.productCaterogyList();
    await this.verderList();
    await this.getAllChartAccountExpense();
    await this.getAllChartAccountInCome();
    await this.getAllChartOfAcccountAdvanceSaleTax();
    await this.getAllChartOfAcccountLiability();
    await this.getProductServices();
  }


  async getAllChartOfAcccountAdvanceSaleTax(): Promise<void> {
    await this._chartOfAccountService.getAllAdvanceSaleTaxAccount().subscribe((res) => {
      this.advanceSaleTaxAccountArr = res;
    })
  }

  async getAllChartOfAcccountLiability(): Promise<void> {
    await this._chartOfAccountService.getAllLiabilitiesAccount().subscribe((res) => {
      this.liabilityAccountArr = res;
    })
  }


  async getAllChartAccountExpense() {
    await this._productServiceServiceProxy
      .getAllChartAccountExpense()
      .subscribe((arg) => (this.chartOfAccountExpenseDto = arg));
  }
  async getAllChartAccountInCome() {
    await this._productServiceServiceProxy
      .getAllChartAccountIncome()
      .subscribe((arg) => (this.chartOfAccountIncomeDto = arg));
  }
  async productCaterogyList() {
    await this._productCategoryServiceProxy
      .getAll()
      .subscribe((arg) => (this.productCategoryDto = arg));
  }

  async verderList() {
    await this._vendorServiceProxy
      .getAll()
      .subscribe((arg) => (this.venderDto = arg)

      );
  }


  async getProductServices(): Promise<void> {
    if (this.data.id) {
      await this._productServiceServiceProxy
        .get(this.data.id)
        .pipe(finalize(() => { }))
        .subscribe((result) => {
          this.createOrEditProductServiceInputDto = result;
        });
    }
    else {
      this.createOrEditProductServiceInputDto = new CreateOrEditProductServiceInputDto();
      this.createOrEditProductServiceInputDto.automaticExpense = false;
    }
  }

  hideDialog() {
    this.onSave.emit();
    this.dialogRef.close();
  }

  numberOnly(event) {
    return this.validation.numberOnlyWith(event);
  }

  letterOnly(event) {
    return this.validation.letterOnlyWithSpaceAllowed(event);
  }

  firstSpaceNotAllowed(event) {
    this.validation.letterOnlyWithSpaceAllowed(event);
  }


  createProductCategory(id?: number): void {
    const dialogRef = this._dialog.open(CreateProductCategoryComponent, {
      data: { id: id },
    });

    dialogRef.afterClosed().subscribe((result) => {
      this.productCaterogyList()
      dialogRef.close();

    });
  }


  CostOrSalePriceHistory(obj?: any): void {
    const dialogRef = this._dialog.open(CostPriceOrSalePriceHistoryComponent, {
      data: { data: obj },
    });

    dialogRef.afterClosed().subscribe((result) => {
      dialogRef.close();

    });
  }

}
