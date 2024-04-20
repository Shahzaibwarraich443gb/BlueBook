import { Component, ElementRef, EventEmitter, Injector, Input, OnInit, Output, ViewChild } from '@angular/core';
import { MatChipInputEvent } from '@angular/material/chips';
import { ContactInfoDto, CreateInvoiceDto, CustomerDto, CustomerServiceProxy, InvoiceDetailDto, InvoiceDto, InvoiceServiceProxy, ProductServiceDto, ProductServiceServiceProxy, EstimateServiceProxy, InvoiceType, ChartOfAccountsServiceProxy, ChartOfAccountDto, ReceivedPaymentServiceServiceProxy, SalesReceiptServiceProxy, CreateSalesReceiptDto, SalesReceiptDto, GeneralPaymentMethodDto, PaymentMethodServiceProxy, ChargeCardDto, CompanyServiceProxy, SaveCreditNoteDto, CreditNoteServiceServiceProxy, InvoiceStatus, RecurringInvoiceDetailDto, ReceviedPayment } from '@shared/service-proxies/service-proxies';
import { ENTER, COMMA, I, SPACE } from '@angular/cdk/keycodes';
import { finalize } from 'rxjs';
import * as moment from 'moment';
import { NotifyService } from 'abp-ng2-module';

import { AppComponentBase } from '@shared/app-component-base';

import { ActivatedRoute, Router } from '@angular/router';

import { CreateChartOfAccountComponent } from 'modules/chart-of-account/create-chart-of-account/create-chart-of-account.component';
import { MatDialog } from '@angular/material/dialog';

import { CreatePaymentMethodComponent } from 'modules/Payement-Method/create-Payment-Method/create-Payement-Method.component';
import { CreditCardComponent } from '../credit-card-modal/credit-card.component';

@Component({
  selector: 'app-credit-note',
  templateUrl: 'Credit-Note.component.html',
  styleUrls: ['Credit-Note.component.scss'],
})

export class CreditNoteComponent extends AppComponentBase implements OnInit {
  constructor(
    private injector: Injector,
    public _customerServiceProxy: CustomerServiceProxy,
    public _productServiceServiceProxy: ProductServiceServiceProxy,
    private _invoiceServiceProxy: InvoiceServiceProxy,
    private _estimateServiceProxy: EstimateServiceProxy,
    public _chartOfAccoountService: ChartOfAccountsServiceProxy,
    private _activatedRoute: ActivatedRoute,
    public _paymentService: PaymentMethodServiceProxy,
    private _Salereceipt: SalesReceiptServiceProxy,
    public _dialog: MatDialog,
    private _router: Router,
    public _companyService: CompanyServiceProxy,
    public _creditNoteServie: CreditNoteServiceServiceProxy
  ) {
    super(injector);
    this.intialization();
  }


  keyword: string
  invoiceId: number
  filterType: string = 'Name';
  selectedAll: boolean = false;
  customers: CustomerDto[] = [];
  selectedProduct: any;
  totalOpenBalance: any;
  productServiceDto: ProductServiceDto[] = [];
  createSalesReceiptKey: CreateSalesReceiptDto;
  createCNKey: SaveCreditNoteDto = new SaveCreditNoteDto();
  //createSalesReceiptKey: CreateSalesReceiptDto;
  DepositToList: ChartOfAccountDto[] = [];
  generalPayementMethodDto: GeneralPaymentMethodDto[] = [];
  signleCalculatedAmount: number = 0;
 
 //Table Data
  selectedRows: any[] = [];
 dataList: Array<any> = [];
  selectedItemRows: any[] = [];
  totalAmount: number = 0;
  creditAmount:number=0;
  singleAmount: any[] = [];
  CreditNoteDate: any;
  
  isDisabled = false;
  discountInAmount;

  

  customerId = 0;
  companyName: string;
  // saleReceipt = new SalesReceiptDto();
  InvoiceDetail: InvoiceDetailDto[] = [];

  //  SalesReceipt = new CreateSalesReceiptDto();

  Customer = new ContactInfoDto();
  ProductService = new ProductServiceDto();
  name: any;

  addOnBlur = true;
  readonly separatorKeysCodes = [ENTER, COMMA, SPACE] as const;
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
    discountAmount: 0

  }];

  ngOnInit() {
    this.customerList();
   
     this.caculateDate();
    //this.openChartOfAccountDialog();
    this._activatedRoute.params.subscribe(parms => {
      if (parms.id) {
        this.invoiceId = +parms.id;
        this.getInvoiceDetails(this.invoiceId)
      }
      else {
        this.createCNKey.refInvoiceType = 3;

      }
    });


    this.productServiceList();
    // this.Invoice.refTermID = 0;
    // this.calculateDate();
  }
  getInvoiceDetails(invoiceId: number) {
    this._invoiceServiceProxy.getInvoiceDetails(invoiceId).pipe(
      finalize(() => { }))
      .subscribe((result) => {
        console.log("result:", result);
        this.createCNKey.refCustomerID = result[0].customerId;
        this.customerId = result[0].customerId;
        this.createCNKey.invoiceNo = result[0].invoiceNo;
    
        this.isDisabled = true;
        this.createCNKey.note = result[0].note;
        this.items.splice(0, 1);
        this.items = [];
        result.forEach((item, index) => {
          this.addRowOnProductSelect(item, index, "edit");
        });
      });
  }
  selectAll() {
    this.selectedRows = [];
    this.totalAmount = 0;
    if (this.selectedAll) {
      this.selectedItemRows = [];
      this.dataList.forEach((item, index) => {
        item.selected = true;
        this.selectedRows.push(item);
        setTimeout(() => {
          this.totalAmount += item.openBalance;
          this.singleAmount[index] = item.openBalance;
        }, 100);
        this.setPaymentList(item, index);
      });
    } else {
      this.dataList.forEach((item, index) => {
        item.selected = false;
        this.singleAmount[index] = 0;
      });
      this.selectedItemRows = [];
    }
  }
  public setPaymentList(item: any, i: any) {
    if (this.isDisabled) {
      let payment = new ReceviedPayment();
      payment.isCheck = true;
      payment.invoiceDetailId = item.invoiceDetailId;
      payment.openBalance = item.openBalance;
      payment.paidAmount = this.singleAmount[i];
      payment.invoiceNo = item.invoiceNo;
      payment.invoiceID = item.invoiceID;
      payment.refProducID = item.refProducID;
      this.selectedItemRows.push(payment);
    } else {
      this.dataList.filter(obj => obj.invoiceID === item.invoiceID).forEach(element => {
        let payment = new ReceviedPayment();
        payment.isCheck = true;
        payment.invoiceDetailId = element.invoiceDetailId;
        payment.refPaidInvoiceID = element.ref_PaidInvoiceID;
        payment.invoiceDueDate = element.invoiceDueDate;
        payment.openBalance = element.openBalance;
        payment.paidAmount = this.singleAmount[i];
        payment.invoiceNo = element.invoiceNo;
        payment.invoiceID = element.invoiceID;
        payment.refProducID = element.refProducID;
        // add if you want to get more values ...
        this.selectedItemRows.push(payment);
      });
    }
  }
  
  update() {
    abp.message.confirm(
      this.l("Are you sure want to update invoice"),
      undefined,
      (result: boolean) => {
        if (result) {
          this.save('save');
        }
      }
    );
  }
  getDepositToList() {
    this._chartOfAccoountService
      .getChartOfAccountsForRP()
      .subscribe((arg) => {
        this.DepositToList = arg
      });
  }

  caculateDate(){
    this.CreditNoteDate=new Date();
  }
  getPaymentMethodList() {
    this._paymentService
      .getAll()
      .subscribe((arg) => {
        this.generalPayementMethodDto = arg
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
          amount: 0
        };
      });
      this.items.splice(index, 1, ...mappedItems);

      //const isNow = this.InvoiceDetail.filter((obj) => obj.refProducID === item.name);
      let data = [];
      for (let i = 0; i < mappedItems.length; i++) {
        let obj = new InvoiceDetailDto();
        obj.refProducID = mappedItems[i].name;
        obj.quantity = mappedItems[i].quantity;
        obj.discount = mappedItems[i].discount;
        obj.saleTax = mappedItems[i].saleTax;
        obj.amount = mappedItems[i].amount;
        obj.rate = mappedItems[i].salePrice;
        obj.description = mappedItems[i].description;
        data.push(obj);
      }
      this.InvoiceDetail.splice(index, 1, ...data);
    }

    if (index === this.items.length - 1 && item.name !== '' && item.name !== undefined) {
      this.items.push({
        name: '',
        description: '',
        quantity: 1,
        salePrice: 0,
        saleTax: 0,
        discount: 0,
        amount: 0
      });
    }
    else if (event === "edit") {
      this.items.push({
        name: item.productId,
        description: item.description,
        quantity: item.quantity,
        salePrice: item.rate,
        saleTax: item.saleTax,
        discount: item.discount,
        amount: +item.amount
      });

      let data = [];
      let obj = new InvoiceDetailDto();
      obj.refProducID = item.productId;
      obj.quantity = item.quantity;
      obj.discount = item.discount;
      obj.saleTax = item.saleTax;
      obj.amount = item.amount;
      obj.rate = item.rate;
      obj.description = item.description;
      obj.id = item.invoiceDetailId;
      data.push(obj);
      this.InvoiceDetail.splice(index, 1, ...data);
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
      else if (name === "discountAmount") {
        this.InvoiceDetail[i].discountAmount = value
      }
      //this.calculateDiscountAmount(value,i);
      this.calculateTotalAmount(i);
    }
  }
  calculateDiscountAmount(value: any, i: number) {

    this.InvoiceDetail[i].discountAmount = (value / 100) * this.InvoiceDetail[i].rate;
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
    this.items[i].discountAmount = discountAmount

    this.totalSalePrice = this.InvoiceDetail.reduce((sum, item) => {
      return sum + item.amount;
    }, 0);
  }
  getReceivedList(customerId: any) {
    this.selectedRows = [];
    this.createCNKey.refCustomerID = customerId;
    this.selectedItemRows = [];
    this._invoiceServiceProxy.getReceivedPaymentList(customerId).pipe(
      finalize(() => { }))
      .subscribe((result) => {
        this.dataList = result;
        this.totalOpenBalance = this.dataList.reduce((total, item) => total + item.openBalance, 0);
      });
  }
  selectCustomerEmail(customerId: any) {
    this.customerId = customerId;
    const customer = this.customers.find((obj) => obj.id === customerId);
    this.emailList=[];
    if (customer.email) {
      this.addEmail({ value: customer.email } as MatChipInputEvent, 1);
    }
    this.getReceivedList(customerId);
    if (customer.refCopmayId) {
      this.getCompany(customer.refCopmayId);
    } else {
      this.companyName = "";
    }

  }
  getCompany(companyId: number) {
    this._companyService.get(companyId).pipe(
      finalize(() => { }))
      .subscribe((result) => {
        this.companyName = result.name;
      });
  }

  changeTotal(value: number): void {
    if (value > this.totalOpenBalance) {
      this.totalAmount = 0;
      this.selectedAll = true;
      this.selectAll();
    } else if (value < this.totalOpenBalance) {
      this.selectedRows = [];
      const totalOpenBalance = this.dataList.reduce((total, item) => total + item.openBalance, 0);
      let distributedAmount = 0;
      this.singleAmount = this.dataList.map((item, i) => {
        if (distributedAmount + item.openBalance <= value) {
          distributedAmount += item.openBalance;
          item.selected = true; // Enable checkbox
          this.selectedRows.push(item);
          this.setPaymentList(item, i);
          return item.openBalance;
        } else if (distributedAmount < value) {
          const remainingAmount = value - distributedAmount;
          distributedAmount = value;
          item.selected = true; // Enable checkbox
          this.selectedRows.push(item);
          this.setPaymentList(item, i);
          return remainingAmount;
        } else {
          item.selected = false; // Disable checkbox
          return 0;
        }
      });
    } else {
      // Handle the case when value is equal to totalOpenBalance
    }
  }

  // Get data lists
  customerList() {
    this._customerServiceProxy.getAll(this.keyword, this.filterType, 0, 10).pipe(
      finalize(() => { }))
      .subscribe((result) => {
        this.customers = result.items;

        if (this.customerId) {
          const Datalist = this.customers.find((obj) => obj.id === this.customerId);
          if (Datalist) {
            this.createCNKey.refCustomerID = Datalist.id;
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
  selectRow(item: any, i: any) {
    if (item.selected) {
      this.selectedRows.push(item);
      this.singleAmount[i] = item.openBalance;
      this.setPaymentList(item, i);
    } else {
      const index = this.selectedRows.findIndex((row) => row.invoiceID === item.invoiceID);
      if (index > -1) {
        this.selectedRows.splice(index, 1);
        this.selectedItemRows.splice(index, 1);
      }
      this.singleAmount[i] = 0;
    }
    this.selectedAll = this.dataList.every((row) => row.selected);
    this.totalAmount = this.selectedRows.reduce((total, row) => total + (this.singleAmount[this.dataList.indexOf(row)] || 0), 0);
  }
  updateSingleAmount(i: number, item: any = null) {
    const foundItem = this.selectedItemRows.find((row) => row.invoiceNo === item.invoiceNo);
    if (foundItem) {
      foundItem.paidAmount = this.singleAmount[i];
    }

    if (this.singleAmount[i] > this.dataList[i].openBalance) {
      setTimeout(() => {
        this.singleAmount[i] = this.dataList[i].openBalance;
      }, 100);
    }

    setTimeout(() => {
      this.totalAmount = this.selectedRows.reduce((total, row) => {
        var selectedRow = this.dataList.find((item) => item.invoiceID === row.invoiceID);
        if (selectedRow && selectedRow.selected) {
          var index = this.dataList.indexOf(selectedRow);
          return total + (this.singleAmount[index] || 0);
        }
        return total;
      }, 0);
    }, 100);
  }
  // Save
  save(type?: any) {
    if (!this.createCNKey.refCustomerID) {
      // this.notify.info(this.l("Please Select Customer"));
      return abp.message.error('Please Select Any Customer', 'Something Wrong!!');
    }
    else if (!this.selectedProduct) {
      return abp.message.error('Please Select At Least One Product!', 'Something Wrong!!');
    }

    this.createCNKey.refInvoiceType = InvoiceType._3;
    this.createCNKey.refInvoiceStatus = InvoiceStatus._3;
    // this.createSalesReceiptKey.salesReceipt = this.saleReceipt;
    let d = this.CreditNoteDate;
    this.createCNKey.creditNoteDate = moment(new Date(d.getFullYear(), d.getMonth(), d.getDate(), d.getHours(), d.getMinutes() - d.getTimezoneOffset()).toISOString());
    this.createCNKey.emails = this.emailList;
    this.createCNKey.creditNoteDto = this.InvoiceDetail;
    this.createCNKey.total = this.totalSalePrice;

    if (this.invoiceId) {
      this.createCNKey.invoiceID = this.invoiceId;
    }
    this._creditNoteServie.saveCreditNote(this.createCNKey).subscribe((result) => {
      //abp.message.success('Invoice save successfully', 'Success');

      if (!this.invoiceId) {
        this.notify.info(this.l("Saved Successfully"));
        this._router.navigate(['/app/customer-detail-view/' + this.customerId]);
        this.resetForm();
      } else {
        this.getInvoiceDetails(this.invoiceId);
        this.notify.info(this.l("Update Successfully"));
       
      }

    });


    if (type === "print") {
      window.print();
    }
  }
  resetForm() {
    this.companyName = null;
    

    this.emailList = [];

  }

}
