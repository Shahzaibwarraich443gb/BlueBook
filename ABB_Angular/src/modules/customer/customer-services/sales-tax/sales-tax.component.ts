import { Component, Injector, Input } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { EntityDto } from '@shared/paged-listing-component-base';
import { LegalStatus, LegalStatusServiceProxy, SalesTaxDto, SalesTaxServiceProxy, TenureForm, TenureServiceProxy } from '@shared/service-proxies/service-proxies';
import { Console } from 'console';
import * as moment from 'moment';
import { NgxSpinnerService } from 'ngx-spinner';

@Component({
  selector: 'app-sales-tax',
  templateUrl: './sales-tax.component.html',
  styleUrls: ['./sales-tax.component.scss']
})
export class SalesTaxComponent extends AppComponentBase {

  financialYearArr: string[] = [];

  @Input() customerId;

  selectedFinancialYear: string = null;

  selectedTenure: number = 1;
  selectedLegalStatus: any = null;

  totalMonthlyAmount: any = null;

  dateRange = null;

  taxableSales: any = null;

  salesTax: any = null;

  dueDate = null;

  nonTaxableAmount = null;

  salesRatePercentage: any = null;


  tenure: TenureForm[];

  legalStatus: LegalStatus[];

  fiscalMonthArr = [];


  constructor(private injector: Injector,
    private tenureService: TenureServiceProxy,
    private legalStatusService: LegalStatusServiceProxy,
    private salesTaxServiceProxy: SalesTaxServiceProxy,
    private spinner: NgxSpinnerService) {
    super(injector);
  }

  ngOnInit(): void {
    this.spinner.show();
    this.tenureService.getAllTenureForms().subscribe((res) => {
      this.tenure = res;
    });

    this.legalStatusService.getAllLegalStatus().subscribe((res) => {
      this.legalStatus = res;
    });

    this.generateFinancialYear();
    this.reRender();
    this.getSalesTax();
  }

  reRender(): void {
    this.generateFiscalDatesMain();
    this.generateFiscalMonths();
    let el = document.getElementsByClassName('monthInput');
    if (el) {
      for (var i = 0; i < el.length; i++) {
        (el[i] as HTMLInputElement).value = null;
      }
      this.selectedTenure = 1;
      this.selectedLegalStatus = null;
      this.taxableSales = null;
      this.salesTax = null;
      this.nonTaxableAmount = null;
      this.salesRatePercentage = null;
      this.calculateTotalMonthlyAmount();
    }
  }

  generateFiscalDatesMain() {
    let fiscalStartDate = new Date(new Date().setFullYear(parseInt(this.selectedFinancialYear.split('-')[0])));
    let fiscalEndDate = new Date(new Date().setFullYear(parseInt(this.selectedFinancialYear.split('-')[1])));
    fiscalStartDate.setDate(1);
    fiscalStartDate.setMonth(2); //jan is 0
    fiscalEndDate.setDate(28);
    fiscalEndDate.setMonth(1); //jan is 0
    this.dateRange = moment(fiscalStartDate).format('MM/DD/YYYY') + ' - ' + moment(fiscalEndDate).format('MM/DD/YYYY');
    this.dueDate = new Date(new Date().setFullYear(parseInt(this.selectedFinancialYear.split('-')[1])));
    this.dueDate.setDate(20);
    this.dueDate.setMonth(2); //jan is 0

    this.dueDate = moment(this.dueDate).format('MM/DD/YY');
  }

  generateFinancialYear(): void {
    let startDate = new Date(new Date().setFullYear(new Date().getFullYear() - 1));
    let endDate = new Date(new Date().setFullYear(startDate.getFullYear() - 5));


    for (var currentDate = startDate; currentDate > endDate; currentDate.setFullYear(currentDate.getFullYear() - 1)) {
      this.financialYearArr.push((currentDate.getFullYear() - 1).toString() + ' - ' + currentDate.getFullYear().toString());
    }

    this.selectedFinancialYear = this.financialYearArr[0];
  }

  generateFiscalMonths(): void {
    this.fiscalMonthArr = [];
    let startDate = new Date(this.dateRange.split('-')[0]);
    let endDate = new Date(this.dateRange.split('-')[1]);
    while (startDate <= endDate) {
      let obj = {
        value: 0,
        month: startDate.toLocaleString('default', { month: 'long' })
      }
      this.fiscalMonthArr.push(obj);
      startDate.setMonth(startDate.getMonth() + 1);
    }

  }


  calculateTotalMonthlyAmount(): void {
    let el = document.getElementsByClassName('monthInput');
    this.totalMonthlyAmount = null;
    for (var i = 0; i < el.length; i++) {
      if (!(el[i] as HTMLInputElement).value) {
        this.totalMonthlyAmount = null;
        break;
      }
      this.fiscalMonthArr[i].value = parseFloat((el[i] as HTMLInputElement).value);
      this.totalMonthlyAmount += parseFloat((el[i] as HTMLInputElement).value);

    }

    if (this.totalMonthlyAmount) {
      this.taxableSales = 0;
      this.salesTax = 0;
    }

  }

  calculateTaxableSales(): void {
    this.taxableSales = (this.totalMonthlyAmount - (this.nonTaxableAmount ? this.nonTaxableAmount : 0));

  }

  calculateSalesTax(): void {
    this.salesTax = ((this.taxableSales * this.salesRatePercentage) / 100).toFixed(2);
  }


  getSalesTax(): void {
    let salesTaxDto = new SalesTaxDto();
    salesTaxDto.customerId = this.customerId;
    salesTaxDto.financialYear = this.selectedFinancialYear
    this.salesTaxServiceProxy.salesTaxGet(salesTaxDto).subscribe((res) => {

      if (res.taxDataMonthly) {
        this.fiscalMonthArr = JSON.parse(res.taxDataMonthly);
      }
      else {
        this.generateFiscalMonths();
      }

      let el = document.getElementsByClassName('monthInput');
      if (el) {
        setTimeout(() => {
          for (var i = 0; i < el.length; i++) {
            (el[i] as HTMLInputElement).value = this.fiscalMonthArr[i].value;
          }

          this.selectedTenure = res.tenureForm == 0 ? 1 : res.tenureForm;
          this.selectedLegalStatus = res.legalStatus;
          this.totalMonthlyAmount = res.totalMonthlyAmount;
          this.nonTaxableAmount = res.nonTaxableAmount;
          this.salesTax = res.salesTaxAmount;
          this.salesRatePercentage = res.salesRatePercentage;
          this.calculateTotalMonthlyAmount();
          this.calculateTaxableSales();
          this.calculateSalesTax();
          this.spinner.hide();
        }, 800);




        this.generateFiscalDatesMain();

      }
    },
      ({ error }) => [
        this.notify.error("Cannot retrieve Sales Data")
      ]);
  }

  saveSalesTax(): void {

    if (!this.selectedFinancialYear || !this.selectedTenure) {
      this.notify.error("Please fill all the required fields");
      return;
    }

    let salesTaxDto = new SalesTaxDto();
    salesTaxDto.financialYear = this.selectedFinancialYear;
    salesTaxDto.tenureForm = this.selectedTenure;
    salesTaxDto.legalStatus = this.selectedLegalStatus;
    salesTaxDto.totalMonthlyAmount = this.totalMonthlyAmount;
    salesTaxDto.nonTaxableAmount = this.nonTaxableAmount;
    salesTaxDto.taxableSales = this.taxableSales;
    salesTaxDto.salesTaxAmount = this.salesTax;
    salesTaxDto.salesRatePercentage = this.salesRatePercentage;
    salesTaxDto.customerId = this.customerId;
    salesTaxDto.taxDataMonthly = JSON.stringify(this.fiscalMonthArr);

    this.salesTaxServiceProxy.saveSalesTax(salesTaxDto).subscribe(res => {
      this.notify.success("Sales Tax Saved");
      this.getSalesTax();
    },
      ({ error }) => {
        this.notify.error("Cannot Save");
      });

  }


}
