import { Component, ElementRef, EventEmitter, Injector, Input, OnInit, Output, ViewChild } from '@angular/core';
import { MatChipInputEvent } from '@angular/material/chips';
import { ContactInfoDto, CreateInvoiceDto, CustomerDto, CustomerServiceProxy, InvoiceDetailDto, InvoiceDto, InvoiceServiceProxy, ProductServiceDto, ProductServiceServiceProxy,EstimateServiceProxy, InvoiceType, ChartOfAccountsServiceProxy, ChartOfAccountDto, ReceivedPaymentServiceServiceProxy, SalesReceiptServiceProxy, CreateSalesReceiptDto, SalesReceiptDto, GeneralPaymentMethodDto, PaymentMethodServiceProxy, ChargeCardDto, Company, CompanyServiceProxy } from '@shared/service-proxies/service-proxies';
import { ENTER, COMMA, I } from '@angular/cdk/keycodes';
import { finalize } from 'rxjs';
import * as moment from 'moment';
import { NotifyService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/app-component-base';
import { setTimeout } from 'timers/promises';
import { ActivatedRoute } from '@angular/router';

import { CreateChartOfAccountComponent } from 'modules/chart-of-account/create-chart-of-account/create-chart-of-account.component';
import { MatDialog } from '@angular/material/dialog';

import { CreatePaymentMethodComponent } from 'modules/Payement-Method/create-Payment-Method/create-Payement-Method.component';
import { CreditCardComponent } from '../credit-card-modal/credit-card.component';

@Component({
  selector: 'app-Sales-Receipt',
  templateUrl: 'Sales-Receipt.component.html',
  styleUrls: ['Sales-Receipt.component.scss'],
})

export class SalesReceiptComponent extends AppComponentBase implements OnInit {
  constructor(
    private injector: Injector,
    public _customerServiceProxy: CustomerServiceProxy,
    public _productServiceServiceProxy: ProductServiceServiceProxy,
    private _invoiceServiceProxy: InvoiceServiceProxy,
    private _estimateServiceProxy: EstimateServiceProxy,
    public _chartOfAccoountService: ChartOfAccountsServiceProxy,
    private _activatedRoute: ActivatedRoute,
    public _paymentService: PaymentMethodServiceProxy,
    private _Salereceipt:SalesReceiptServiceProxy,
    public _dialog: MatDialog,
     public  _companyService:CompanyServiceProxy,

  ) {
    super(injector);
    this.intialization();
  }

  keyword: string

  filterType: string = 'Name';
  customers: CustomerDto[] = [];
  selectedProduct: any;
  productServiceDto: ProductServiceDto[] = [];
  createSalesReceiptKey: CreateSalesReceiptDto;
 //createSalesReceiptKey: CreateSalesReceiptDto;
  DepositToList: ChartOfAccountDto[] = [];
  generalPayementMethodDto: GeneralPaymentMethodDto[] = [];
  signleCalculatedAmount: number = 0;
  EstimationDate: any;
  SaleReceiptDate:any;
  ExpirationDate: any;
  discountInAmount;
  referenceNo: any;
  invoiceId: number;
  refDepositToAccountId: any;
  refPaymentMethodID:any;
  customerId = 0;
  companyName: string;
  saleReceipt = new SalesReceiptDto();
  InvoiceDetail: InvoiceDetailDto[] = [];

  //  SalesReceipt = new CreateSalesReceiptDto();

  Customer = new ContactInfoDto();
  ProductService = new ProductServiceDto();
  name: any;

  addOnBlur = true;
  readonly separatorKeysCodes = [ENTER, COMMA] as const;
  emailList: string[] = [];
  chargeIsActive = false;
  totalSalePrice = 0.00;
  AmountReceived;
  BalanceDue;

  items: any[] = [{
    name: '',
    description: '',
    quantity: 1,
    salePrice: 0,
    saleTax: 0,
    discount: 0,
    amount: 0,
    discountAmount:0

  }];

  ngOnInit() {
   
    this.SaleReceiptDate = new Date;
    this.customerList();
    this.getPaymentMethodList();
    this.getDepositToList();
   //this.openChartOfAccountDialog();
    this._activatedRoute.params.subscribe(parms => {
      if (parms.id) {
        this.customerId = +parms.id;
      }
    });
    this.productServiceList();
   // this.Invoice.refTermID = 0;
   // this.calculateDate();
  }


  getDepositToList() {
    this._chartOfAccoountService
      .getChartOfAccountsForRP()
      .subscribe((arg) => {
        this.DepositToList = arg
        if(!this.invoiceId){
          this.saleReceipt.refDepositToAccountID=arg[0].id
        }
      });
  }
  getPaymentMethodList() {
    this._paymentService
      .getAll()
      .subscribe((arg) => {
        this.generalPayementMethodDto = arg
        
        if (!this.invoiceId) { this.refPaymentMethodID = arg[0].id; }
      });
  }
  selectPaymentType(item: number) {
    var paymentType = this.generalPayementMethodDto.find((obj) => obj.id === item);
    if (paymentType.name.toLowerCase() === "credit card".toLowerCase()) {
      this.chargeIsActive = true;
    } else {
      this.chargeIsActive = false;
    }
  }


  public openChartOfAccountDialog(id?: number): void {
    const dialogRef = this._dialog.open(CreateChartOfAccountComponent, {
      data: {
        id: id,
        setAssets: "fixed"
      },
    });
    dialogRef.afterClosed().subscribe((result) => {
      this.getDepositToList();
      dialogRef.close();
    });
  }
  public openCreditCardDialog(id?: number): void {
    const dialogRef = this._dialog.open(CreditCardComponent, {
      data: { totalAmount: this.totalSalePrice },
    });
    dialogRef.afterClosed().subscribe((result: ChargeCardDto) => {
      if (result) { this.saleReceipt.chargeCard = result; }
      dialogRef.close();
    });
  }
  public openPaymentMethodDialog(id?: number): void {
    const dialogRef = this._dialog.open(CreatePaymentMethodComponent, {
      data: { id: id },
    });
    dialogRef.afterClosed().subscribe((result) => {
      this.getPaymentMethodList();
      dialogRef.close();
    });
  }

  intialization() {
    this.createSalesReceiptKey = new CreateSalesReceiptDto();
   // this.SalesReceipt = new CreateSalesReceiptDto();
    this.createSalesReceiptKey.salesReceipt = new SalesReceiptDto();
    this.createSalesReceiptKey.salesReceipt.invoiceDetails = [];
  }
  //  Email
  addEmail(event: MatChipInputEvent, bit: number): void {
    if (event && !undefined) {
      const value = event.value;
      if (value && bit === 0) {
        this.emailList.push(value);
      }
      else if (value && bit === 1) {
        for (let i = 0; i < this.emailList.length; i++) {
          this.emailList.splice(i, 1);
        }
        this.emailList.splice(0, 1);
        this.emailList.push(value);
      }
      if (event.chipInput) {
        event.chipInput!.clear();
      }
    }
  }

  removeEmail(item: any): void {
    const index = this.emailList.indexOf(item);
    if (index >= 0) {
      this.emailList.splice(index, 1);
    }
  }
  // Table
  addRowOnProductSelect(item: any, index: number, event: any): void {
    const selectList = this.productServiceDto.filter((obj) => {
      return obj.id === item?.name;
    });
    this.selectedProduct = selectList;

    if (selectList.length > 0) {
      const mappedItems = selectList.map((selectItem) => {
        return {
          name: +selectItem.id,
          description: '',
          quantity: 1,
          salePrice: selectItem.salePrice,
          saleTax: +selectItem.saleTax,
          discount: 0,
          amount: 0,
          discountAmount:0
        };
      });
      this.items.splice(index, 1, ...mappedItems);

      let data = [];
      for (let i = 0; i < mappedItems.length; i++) {
        let obj = new InvoiceDetailDto();
        obj.refProducID = mappedItems[i].name;
        obj.quantity = mappedItems[i].quantity;
        obj.discount = mappedItems[i].discount;
        obj.saleTax = mappedItems[i].saleTax;
        obj.amount = mappedItems[i].amount;
        obj.rate = mappedItems[i].salePrice;
        obj.discountAmount=mappedItems[i].discountAmount;

        data.push(obj);
      }
      this.InvoiceDetail.splice(index, 1, ...data);
    }

    if (index === this.items.length - 1 && item.name !== '') {
      this.items.push({
        name: '',
        description: '',
        quantity: 1,
        salePrice: 0,
        saleTax: 0,
        discount: 0,
        amount: 0,
        discountAmount:0
      });
    }

    this.calculateTotalAmount(index);
  }

  changeValues(name: any, value: any, i: number) {
    if (this.InvoiceDetail.length > 0) {
      if (name === "saleprice") {
        this.InvoiceDetail[i].rate = value;
      } else if (name === "description") {
        this.InvoiceDetail[i].description = value;
      } else if (name === "quantity") {
        this.InvoiceDetail[i].quantity = value;
      } else if (name === "discount") {
        this.InvoiceDetail[i].discount = value;
      } else if (name === "amount") {
        this.InvoiceDetail[i].amount = value;
      }
       else if(name === "discountAmount"){
        this.InvoiceDetail[i].discountAmount=value
      }
      //this.calculateDiscountAmount(value,i);
      this.calculateTotalAmount(i);
    }
  }


  calculateDiscountAmount(value:any ,i: number)
  {

      this.InvoiceDetail[i].discountAmount= (value/100)* this.InvoiceDetail[i].rate;


  }
  removeRow(item: any, i: number) {
    if (+item.name) {
      this.totalSalePrice -= this.InvoiceDetail[i].amount;
      this.items.splice(i, 1);
      this.InvoiceDetail.splice(i, 1);
    } else {
      this.items.splice(i, 1);
    }
  }


  calculateTotalAmount(i: any) {
    //if (this.InvoiceDetail[i].saleTax && this.InvoiceDetail[i].discount > 0) {
    // Calculate the subtotal
    const subtotal = this.InvoiceDetail[i].quantity * this.InvoiceDetail[i].rate;
    // Calculate the discount amount
    const discountAmount = subtotal * (this.InvoiceDetail[i].discount / 100);
    // Calculate the subtotal after discount
    const subtotalAfterDiscount = subtotal - discountAmount;
    // Calculate the tax amount
    const taxAmount = subtotalAfterDiscount * (this.InvoiceDetail[i].saleTax / 100);
    // Calculate the total amount
    const totalAmount = subtotalAfterDiscount + taxAmount;
    this.signleCalculatedAmount = totalAmount;
    // }
    //  else {
    //   this.signleCalculatedAmount = this.InvoiceDetail[i].rate * this.InvoiceDetail[i].quantity - (this.InvoiceDetail[i].discount / 100) * (this.InvoiceDetail[i].rate * this.InvoiceDetail[i].quantity);
    // }
    this.InvoiceDetail[i].amount = this.signleCalculatedAmount;
    const maxDigits = 2; // Maximum number of decimal places
    const value = this.signleCalculatedAmount; // Your value here
    this.items[i].amount = value.toFixed(maxDigits);
   this.items[i].discountAmount=discountAmount

    this.totalSalePrice = this.InvoiceDetail.reduce((sum, item) => {
      return sum + item.amount;
    }, 0);
  }

  /* calculateDate() {
    const currentDate = moment();
    if (this.Invoice.refTermID) {
      const terms = this.Invoice.refTermID;
      const dueDate = moment(currentDate).add(terms, 'days');
      this.invoiceDueDateAlt = new Date(dueDate.format('YYYY-MM-DD'));
      this.invoiceDateAlt = new Date();
    } else {
      this.invoiceDueDateAlt = new Date();
      this.invoiceDateAlt = new Date();
    }
  } */

  selectCustomerEmail(customerId: any) {
    
    const customer = this.customers.find((obj) => obj.id === customerId);
    this.emailList=[];
    if (customer.email) {
      this.addEmail({ value: customer.email } as MatChipInputEvent, 1);
    }
    if (customer.refCopmayId) {
      this.getCompany(customer.refCopmayId);
    } else {
      this.companyName = "";
    }

  }

//   getCompanyName(customerId: number)
//   {
// this._estimateServiceProxy.getCompanyName(customerId).subscribe(x=>{
//   this.companyName=x;

// })
//   }

  // Get data lists

  getCompany(companyId: number) {
    this._companyService.get(companyId).pipe(
      finalize(() => { }))
      .subscribe((result) => {
        this.companyName = result.name;
      });
  }

  customerList() {
    this._customerServiceProxy.getAll(this.keyword, this.filterType, 0, 10).pipe(
      finalize(() => { }))
      .subscribe((result) => {
        this.customers = result.items;

        if (this.customerId) {
          const Datalist = this.customers.find((obj) => obj.id === this.customerId);
          if (Datalist) {
            this.saleReceipt.refCustomerID = Datalist.id;
            this.addEmail({ value: Datalist.email } as MatChipInputEvent, 1);
          }
        }
      });
  }


  productServiceList() {
    this._productServiceServiceProxy.getAll().subscribe((result) => (
      this.productServiceDto = result
    ));
  }

  // Save
  save(type?:any)
   {

    
    
   

    if (!this.saleReceipt.refCustomerID) {
      // this.notify.info(this.l("Please Select Customer"));
      return abp.message.error('Please Select Any Customer', 'Something Wrong!!');
    }
    else if (!this.selectedProduct) {
      return abp.message.error('Please Select At Least One Product!', 'Something Wrong!!');
    }
  else  if (!this.saleReceipt.refDepositToAccountID) {
      return abp.message.error('Please Select Deposit To Account', 'Something Wrong!!');
    }
  // this.createSalesReceiptKey.salesReceipt.refInvoiceType=InvoiceType._4;
  this.createSalesReceiptKey.salesReceipt.refInvoiceType=4;
  let SaleReceiptDate = this.SaleReceiptDate;
    this.createSalesReceiptKey.salesReceipt = this.saleReceipt;

    if (this.SaleReceiptDate) {
      this.createSalesReceiptKey.salesReceipt.saleReceiptDate = moment(new Date(SaleReceiptDate.getFullYear(), SaleReceiptDate.getMonth(), SaleReceiptDate.getDate(), SaleReceiptDate.getHours(), SaleReceiptDate.getMinutes() - SaleReceiptDate.getTimezoneOffset()).toISOString());
    }
    // let d= this.SaleReceiptDate;
    // this.createSalesReceiptKey.salesReceipt.saleReceiptDate =  moment(new Date(d.getFullYear(), d.getMonth(), d.getDate(), d.getHours(), d.getMinutes() - d.getTimezoneOffset()).toISOString());
    this.createSalesReceiptKey.salesReceipt.refrenceNo=this.referenceNo;
    this.createSalesReceiptKey.salesReceipt.email = this.emailList;
    
    this.createSalesReceiptKey.salesReceipt.invoiceDetails = this.InvoiceDetail;

    this.createSalesReceiptKey.salesReceipt.refPaymentMethodID=this.refPaymentMethodID;
    this.createSalesReceiptKey.salesReceipt.refDepositToAccountID=this.refDepositToAccountId;

    this.createSalesReceiptKey.salesReceipt.total=this.totalSalePrice


    this._Salereceipt.saveInvoice(this.createSalesReceiptKey).subscribe((result) => {
      //abp.message.success('Invoice save successfully', 'Success');
      this.notify.info(this.l("SavedSuccessfully"));
      this.resetForm();


    });


    if (type === "print") {
      window.print();
    }
  }
resetForm()
{
  this.companyName=null;
  this.EstimationDate=null;
  this.ExpirationDate=null;
  this.emailList=[];

}

}