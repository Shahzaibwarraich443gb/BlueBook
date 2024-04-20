import { Component, Injector, Input } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { EntityDto } from '@shared/paged-listing-component-base';
import { CorporateTaxDto, CorporateTaxServiceProxy, LegalStatus, LegalStatusServiceProxy, SalesTaxDto, SalesTaxServiceProxy, TenureForm, TenureServiceProxy } from '@shared/service-proxies/service-proxies';

import * as moment from 'moment';
import { NgxSpinnerService } from 'ngx-spinner';

@Component({
  selector: 'app-corporate-tax',
  templateUrl: './corporate-tax.component.html',
  styleUrls: ['./corporate-tax.component.scss']
})
export class CorporateTaxComponent extends AppComponentBase {

  financialYearArr: number[] = [];

  @Input() customerId;

  selectedFinancialYear: number = null;

  selectedTenure: number = 1;
  selectedLegalStatus: any = null;

  quaterObj: any;

  totalMonthlyAmount: number = 0;

  totalWithdrawalAmount: number = 0;

  totalBankCharges: number = 0;

  janReceipt: number = 0;

  febReceipt: number = 0;

  totalOtherIncome: number = 0;

  totalCostOfSale: number = 0;

  totalOtherExpense: number = 0;

  balanceAmount: number = 0;

  showSalesTax: any;

  dateRange = null;

  prevYearDisable: boolean = true;

  taxableSales: any = null;

  salesTax: any = null;

  dueDate = null;

  nonTaxableAmount = null;

  salesRatePercentage: any = null;


  tenure: TenureForm[];

  legalStatus: LegalStatus[];

  fiscalMonthArr = [];

  otherIncomeArr = [];

  otherExpenseArr = [];

  costOfSaleArr = [];

  taxMode: string = "Fiscal Year"


  constructor(private injector: Injector,
    private tenureService: TenureServiceProxy,
    private legalStatusService: LegalStatusServiceProxy,
    private corporateTaxService: CorporateTaxServiceProxy,
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
      this.getCorporateTax();
      setTimeout(() => {
        this.calculateTotalMonthlyAmount();
      }, 800);
      this.spinner.hide();
    }
  }

  generateFiscalDatesMain() {
    //jan is 0
    let fiscalStartDate = new Date(this.selectedFinancialYear, 0, 1);
    let fiscalEndDate = new Date(this.selectedFinancialYear, 11, 31);
    this.dateRange = moment(fiscalStartDate).format('MM/DD/YYYY') + ' - ' + moment(fiscalEndDate).format('MM/DD/YYYY');
    this.dueDate = moment(new Date(this.selectedFinancialYear, 0, 15)).format('MM/DD/YY');
  }

  generateFinancialYear(): void {
    let year = new Date().getFullYear() - 2;

    for (var i = 0; i < 5; i++) {
      this.financialYearArr.push(year);
      year -= 1;

    }

    this.selectedFinancialYear = this.financialYearArr[0];
  }


  toggleSalesTax(): void {
    this.spinner.show();
    if (this.showSalesTax) {
      this.getCorporateTax();
    }
    else {
      this.reRender();
    }
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
    this.totalMonthlyAmount = 0;
    for (var i = 0; i < el.length; i++) {
      if (i == 0) {
        this.janReceipt = parseFloat((el[i] as HTMLInputElement).value);
      }
      if (i == 1) {
        this.febReceipt = parseFloat((el[i] as HTMLInputElement).value);
      }
      if (!(el[i] as HTMLInputElement).value) {
        this.totalMonthlyAmount = null;
        break;
      }
      this.fiscalMonthArr[i].value = parseFloat((el[i] as HTMLInputElement).value);
      this.totalMonthlyAmount += parseFloat((el[i] as HTMLInputElement).value);

      document.getElementsByClassName('balanceSpecific')[i].textContent = (parseFloat((el[i] as HTMLInputElement).value) - parseFloat((document.getElementsByClassName('withdrawalInput')[i] as HTMLInputElement).value)).toString()


    }

    this.calculateBalance();
    this.calculateTaxMode();

    if (this.totalMonthlyAmount) {
      this.taxableSales = 0;
      this.salesTax = 0;
    }

  }


  calculateTotalWithdrawalAmount(): void {
    let el = document.getElementsByClassName('withdrawalInput');
    this.totalWithdrawalAmount = null;
    for (var i = 0; i < el.length; i++) {
      if (!(el[i] as HTMLInputElement).value) {
        this.totalWithdrawalAmount = null;
        break;
      }
      this.totalWithdrawalAmount += parseFloat((el[i] as HTMLInputElement).value);
      document.getElementsByClassName('balanceSpecific')[i].textContent = (parseFloat((document.getElementsByClassName('monthInput')[i] as HTMLInputElement).value) - parseFloat((el[i] as HTMLInputElement).value)).toString()

    }

    this.calculateBalance();

  }


  calculateTotalBankCharges(): void {
    let el = document.getElementsByClassName('bankChargesInput');
    this.totalBankCharges = null;
    for (var i = 0; i < el.length; i++) {
      if (!(el[i] as HTMLInputElement).value) {
        this.totalBankCharges = null;
        break;
      }
      this.totalBankCharges += parseFloat((el[i] as HTMLInputElement).value);

    }

  }

  calculateBalance(): void {
    this.balanceAmount = this.totalMonthlyAmount - this.totalWithdrawalAmount;
  }

  calculateTaxableSales(): void {
    this.taxableSales = (this.totalMonthlyAmount - (this.nonTaxableAmount ? this.nonTaxableAmount : 0));

  }

  calculateSalesTax(): void {
    this.salesTax = ((this.taxableSales * this.salesRatePercentage) / 100).toFixed(2);
  }


  getCorporateTax(): void {
    let corporateDto = new CorporateTaxDto();
    corporateDto.financialYear = this.selectedFinancialYear ? this.selectedFinancialYear : this.financialYearArr[0];
    corporateDto.customerId = this.customerId;

    this.corporateTaxService.corporateTaxGet(corporateDto).subscribe((res) => {
      this.selectedTenure = res.tenure == 0 ? 1 : res.tenure;
      this.selectedLegalStatus = res.legalStatus;
      if (res.monthlyData) {
        let monthlyDataArr = JSON.parse(res.monthlyData);
        for (var i = 0; i < monthlyDataArr.length; i++) {
          (document.getElementsByClassName('monthInput')[i] as HTMLInputElement).value = monthlyDataArr[i].bankDeposit;
          (document.getElementsByClassName('withdrawalInput')[i] as HTMLInputElement).value = monthlyDataArr[i].withdrawal;
          (document.getElementsByClassName('bankChargesInput')[i] as HTMLInputElement).value = monthlyDataArr[i].bankCharges;
        }
      }

      this.otherIncomeArr = JSON.parse(res.otherIncome);
      this.otherExpenseArr = JSON.parse(res.otherExpense);
      this.costOfSaleArr = JSON.parse(res.costOfSale);

      for(var data of this.otherIncomeArr){
        this.totalOtherIncome += parseFloat(data.Value);
      }

      for(var data of this.costOfSaleArr){
        this.totalCostOfSale += parseFloat(data.Value);
      }

      for(var data of this.otherExpenseArr){
        this.totalOtherExpense += parseFloat(data.Value);
      }
      this.spinner.hide();
    });


  }

  addOtherExpense(): void{
    this.otherExpenseArr.push({value: 0, name: ''});
  }

  addOtherIncome(): void{
    this.otherIncomeArr.push({value: 0, name: ''});
  }

  addCostOfSale(): void{
    this.costOfSaleArr.push({value: 0, name: ''});
  }

  calculateTotalCostOfSale(index): void{
    if(this.costOfSaleArr[index].Name && this.costOfSaleArr[index].Value){
      this.totalCostOfSale += parseFloat(this.costOfSaleArr[index].Value);
    }
  }

  calculateTotalOtherIncome(index): void{
    if(this.otherIncomeArr[index].Name && this.otherIncomeArr[index].Value){
      this.totalOtherIncome += parseFloat(this.otherIncomeArr[index].Value);
    }
  }

  calculateTotalOtherExpense(index): void{
    if(this.otherExpenseArr[index].Name && this.otherExpenseArr[index].Value){
      this.totalOtherExpense += parseFloat(this.otherExpenseArr[index].Value);
    }
  }


  getObjectKeys(obj: any): string[] {
    return Object.keys(obj);
  }

  calculateTaxMode(): void {

    let q1 = 0;
    let q2 = 0;
    let q3 = 0;
    let q4 = 0;

    let traverseCount = 1;

    let startIndex = 0, endIndex = 0;

    while (traverseCount <= 4) {
      let amount = 0;
      startIndex = traverseCount == 1 ? 0 : endIndex;
      endIndex = startIndex + 3;
      for (var i = startIndex; i < endIndex; i++) {
        amount += (document.getElementsByClassName('monthInput')[i] as HTMLInputElement) ? parseFloat((document.getElementsByClassName('monthInput')[i] as HTMLInputElement).value) : 0;
      }

      if (traverseCount == 1) {
        q1 = amount;
      }
      else if (traverseCount == 2) {
        q2 = amount;
      }
      else if (traverseCount == 3) {
        q3 = amount;
      }
      else {
        q4 = amount;
      }

      traverseCount++;
    }


    this.quaterObj = {
      q1: q1,
      q2: q2,
      q3: q3,
      q4: q4
    }


  }

  saveCorporateTax(): void {

    if (!this.selectedFinancialYear || !this.selectedTenure) {
      this.notify.error("Please fill all the required fields");
      return;
    }

    let corporateTaxDto = new CorporateTaxDto();
    corporateTaxDto.financialYear = this.selectedFinancialYear;
    corporateTaxDto.legalStatus = this.selectedLegalStatus ? this.selectedLegalStatus : 0;
    corporateTaxDto.tenure = this.selectedTenure;
    corporateTaxDto.customerId = this.customerId

    let monthInput = document.getElementsByClassName('monthInput');
    let withdrawalInput = document.getElementsByClassName('withdrawalInput');
    let bankChargesInput = document.getElementsByClassName('bankChargesInput');

    let monthlyDataArr = [];

    for (var i = 0; i < monthInput.length; i++) {
      let obj = {
        month: i,
        bankDeposit: (monthInput[i] as HTMLInputElement).value,
        withdrawal: (withdrawalInput[i] as HTMLInputElement).value,
        bankCharges: (bankChargesInput[i] as HTMLInputElement).value
      };

      monthlyDataArr.push(obj);
    }

    corporateTaxDto.monthlyData = JSON.stringify(monthlyDataArr);

    corporateTaxDto.otherIncome = JSON.stringify(this.otherIncomeArr);
    corporateTaxDto.otherExpense = JSON.stringify(this.otherExpenseArr);
    corporateTaxDto.costOfSale = JSON.stringify(this.costOfSaleArr);

    this.corporateTaxService.saveCorporateTax(corporateTaxDto).subscribe(res => {
      this.notify.success("Sales Tax Saved");
      this.getCorporateTax();
    },
      ({ error }) => {
        this.notify.error("Cannot Save");
      });

  }


}
