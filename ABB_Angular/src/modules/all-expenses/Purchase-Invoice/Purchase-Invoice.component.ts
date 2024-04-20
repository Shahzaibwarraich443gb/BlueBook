import { Component, ElementRef, EventEmitter, Injector, Input, OnInit, Output, ViewChild } from '@angular/core';
import { MatChipInputEvent } from '@angular/material/chips';
import { AddGeneralLedgarInputDto, ChartOfAccountDto, ChartOfAccountsServiceProxy, CustomerServiceProxy, GeneralLedgerServiceProxy, InvoiceDetailDto, InvoiceDto, InvoiceServiceProxy, ProductServiceDto, ProductServiceServiceProxy, PurchaseInvoiceDto, PurchaseInvoiceServiceServiceProxy, SavePurchaseInvoice, VenderServiceProxy } from '@shared/service-proxies/service-proxies';
import { ENTER, COMMA, I } from '@angular/cdk/keycodes';
import { finalize } from 'rxjs';
import * as moment from 'moment';
import { NotifyService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/app-component-base';
import { setTimeout } from 'timers/promises';
import { ActivatedRoute } from '@angular/router';

import { result } from 'lodash-es';
import { PurchaseInvoiceAccountDto } from './../../../shared/service-proxies/service-proxies';
import { NgxSpinnerService } from 'ngx-spinner';

@Component({
  selector: 'app-Purchase-Invoice',
  templateUrl: 'Purchase-Invoice.component.html',
  styleUrls: ['Purchase-Invoice.component.scss'],
})

export class PurchaseInvoiceComponent extends AppComponentBase implements OnInit {
  isActive: boolean = false;

  constructor(
    private injector: Injector,
    private _vendor: VenderServiceProxy,
    public _customerServiceProxy: CustomerServiceProxy,
    public _productServiceServiceProxy: ProductServiceServiceProxy,
    public _chartOfAccoountService: ChartOfAccountsServiceProxy,
    public _purchaseInvoiceService: PurchaseInvoiceServiceServiceProxy,
    private _activatedRoute: ActivatedRoute,
    private _invoiceServiceProxy: InvoiceServiceProxy,
    private _generalLedgerService: GeneralLedgerServiceProxy,
    private _spinner: NgxSpinnerService
  ) {
    super(injector);
  }

  selectedProduct: any;
  totalSalePrice: number = 0;
  changeAmount: number = 0;
  invoiceId: any;
  signleCalculatedAmount: number;
  isDisabled = false;
  purchaseInvoice = new SavePurchaseInvoice;
  productServiceDto: ProductServiceDto[] = [];
  InvoiceDetail: PurchaseInvoiceDto[] = [];
  AccountInvoice: PurchaseInvoiceAccountDto[] = [];
  DepositToList: ChartOfAccountDto[] = [];
  invoiceDateAlt: any;
  invoiceDueDateAlt: any;
  vendors: any;
  keyword: string;
  filterType: string = 'Name';
  customers: any;
  accountItems: any[] = [{
    account: 0,
    description: '',
    amount: 0,
    customer: 0
  }];

  items: any[] = [{
    name: '',
    description: '',
    quantity: 1,
    salePrice: 0,
    saleTax: 0,
    discount: 0,
    amount: 0,
    customer: 0
  }];

  ngOnInit() {
    this._spinner.show();
    this.vendorlist();
    this.customerList();
    this.productServiceList();
    this.getDepositToList();
    this._activatedRoute.params.subscribe(parms => {
      if (parms.id) {
        this.invoiceId = +parms.id;
        this.getInvoiceDetails(this.invoiceId);
      } else {
        this.purchaseInvoice.refTermID = 0;
      }
    });
    this.calculateDate();
  }

  calculateDate() {
    const currentDate = moment();
    if (this.purchaseInvoice.refTermID) {
      const terms = this.purchaseInvoice.refTermID;
      const dueDate = moment(currentDate).add(terms, 'days');
      this.invoiceDueDateAlt = new Date(dueDate.format('YYYY-MM-DD'));
      this.invoiceDateAlt = new Date();
    } else {
      this.invoiceDueDateAlt = new Date();
      this.invoiceDateAlt = new Date();
    }
  }

  vendorlist() {
    this._vendor.getAll().subscribe((result) => {
      this.vendors = result;
    })
  }
  customerList() {
    this._customerServiceProxy.getAll(this.keyword, this.filterType, 0, 10).pipe(
      finalize(() => { }))
      .subscribe((result) => {
        this.customers = result.items;
        this._spinner.hide();
      });
  }
  productServiceList() {
    this._productServiceServiceProxy.getAllExpense().subscribe((result) => (
      this.productServiceDto = result
    ));
  }
  getDepositToList() {
    this._chartOfAccoountService.getChartOfAccountsForRP().pipe(finalize(() => { }))
      .subscribe((result) => {
        this.DepositToList = result;
        // if(!this.invoiceId){this.receivedPayment.refDepositToAccountId = result[0].id;}
      });
  }

  getInvoiceDetails(invoiceId: number) {
    this._invoiceServiceProxy.getInvoiceDetails(invoiceId).pipe(
      finalize(() => { }))
      .subscribe((result) => {
        const accountList = result.filter((obj) => {
          return obj.refChartOfAccountId > 0;
        });
        const invoiceList = result.filter((obj) => {
          return obj.refChartOfAccountId === 0;
        });
        this.purchaseInvoice.vendorId = result[0].vendorId;
        this.purchaseInvoice.invoiceNo = result[0].invoiceNo;
        // this.isDisabled = true;
        this.purchaseInvoice.refTermID = result[0].refTermId;
        this.calculateDate();
        // this.purchaseInvoice.isSendLater = result[0].isSendLater;
        this.items.splice(0, 1);
        this.accountItems.splice(0, 1);
        this.purchaseInvoice.note = result[0].note;
        this.accountItems = [];
        accountList.forEach((item, index) => {
          this.addNewRow(item, index, "edit");
        });

        this.items = [];
        invoiceList.forEach((item, index) => {
          this.addRowOnProductSelect(item, index, "edit");
        });
      });
  }

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
        amount: +item.amount,
        customer: +item.refCustomerId
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
      obj.invoiceDetailId = item.invoiceDetailId;
      obj.refCustomerID = item.refCustomerId;
      data.push(obj);
      this.InvoiceDetail.splice(index, 1, ...data);
    }
    this.calculateTotalAmount(index);
  }

  setCustomerId(item: any, customer: any, index: number): void {
    this.items[index].customer = customer;
    this.InvoiceDetail[index].refCustomerID = customer;
  }

  changeValues(name: any, value: any, i: number) {
    if (this.InvoiceDetail.length > 0 && this.items[i].name !== '') {
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
      this.calculateTotalAmount(i);
    }
  }

  removeRow(item: any, i: number) {
    if (+item.name) {
      this.totalSalePrice -= this.InvoiceDetail[i].amount;
      this.items.splice(i, 1);
      if (this.invoiceId) { this.InvoiceDetail[i].isPaid = true; }
      else { this.InvoiceDetail.splice(i, 1); }
    } else {
      this.items.splice(i, 1);
    }
  }

  calculateTotalAmount(i: any) {
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
    this.InvoiceDetail[i].amount = this.signleCalculatedAmount;
    const maxDigits = 2; // Maximum number of decimal places
    const value = this.signleCalculatedAmount; // Your value here
    this.items[i].amount = value.toFixed(maxDigits);

    this.totalSalePrice = this.InvoiceDetail.reduce((sum, item) => {
      return sum + item.amount;
    }, 0);
  }

  // For Account Table 
  addNewRow(item: any, index: number, event: any): void {
    if (index === this.accountItems.length - 1 && item.account !== '' && item.account !== undefined) {
      this.accountItems.push({
        account: 0,
        description: '',
        amount: 0,
        customer: 0
      });
    } else if (event === "edit") {
      this.accountItems.push({
        invoiceDetailId: item.invoiceDetailId,
        account: item.refChartOfAccountId,
        description: item.description,
        amount: +item.amount,
        customer: +item.refCustomerId
      });
      this.changeTotalAmonut("item", item.amount, index);
    }
  }

  changeTotalAmonut(type: string, event: number, index: number) {
    this.accountItems[index].amount = event;
    this.changeAmount = this.accountItems.reduce((sum, item) => {
      if (item.account !== 0) {
        return sum + item.amount;
      } else {
        return sum + 0;
      }
    }, 0);
  }

  accountRemoveRow(item: any, i: number) {
    if (+item.amount) {
      this.totalSalePrice -= this.accountItems[i].amount;
      this.accountItems.splice(i, 1);
      if (this.invoiceId) { this.accountItems[i].isPaid = true; }
      else { this.accountItems.splice(i, 1); }
    } else {
      this.accountItems.splice(i, 1);
    }
  }

  save() {
    this.isActive = true;
    if (!this.purchaseInvoice.vendorId) {
      this.isActive = false;
      return this.notify.error(this.l("please select any vendor"));
    }
    if (!this.purchaseInvoice.refNo) {
      this.isActive = false;
      return this.notify.error(this.l("please add reference number"));
    }
    if (!this.accountItems[0].account) {
      // this.isActive = false;
      // return this.notify.error(this.l("please select any account"));
    }
    if (!this.items[0].name) {
      this.isActive = false;
      return this.notify.error(this.l("please select any product"));
    }

    if (this.invoiceId) {
      this.purchaseInvoice.invoiceId = this.invoiceId;
    }

    this._spinner.show();
    let invoiceDateAlt = this.invoiceDateAlt;
    let invoiceDueDateAlt = this.invoiceDueDateAlt;
    let data = [];
    this.accountItems.forEach((item, index) => {
      let obj = new PurchaseInvoiceAccountDto();
      if (item.account !== 0) {
        obj.invoiceDetailID = item.invoiceDetailId;
        obj.refChartOfAccountID = item.account;
        obj.description = item.description;
        obj.amount = item.amount;
        obj.refCustomerID = item.customer;
        data.push(obj);
      } else {
        //this.accountItems.splice(index, 1);
      }
    });
    this.purchaseInvoice.total = this.changeAmount + this.totalSalePrice;
    this.purchaseInvoice.purchaseInvoiceAccount = data;
    this.purchaseInvoice.purchaseInvoice = this.InvoiceDetail;
    this.purchaseInvoice.purchaseInvoiceDate = moment(new Date(invoiceDateAlt.getFullYear(), invoiceDateAlt.getMonth(), invoiceDateAlt.getDate(), invoiceDateAlt.getHours(), invoiceDateAlt.getMinutes() - invoiceDateAlt.getTimezoneOffset()).toISOString());
    this.purchaseInvoice.invoiceDueDate = moment(new Date(invoiceDueDateAlt.getFullYear(), invoiceDueDateAlt.getMonth(), invoiceDueDateAlt.getDate(), invoiceDueDateAlt.getHours(), invoiceDueDateAlt.getMinutes() - invoiceDateAlt.getTimezoneOffset()).toISOString());
    this._purchaseInvoiceService.savePurchaseInvoice(this.purchaseInvoice).subscribe((res) => {
      if (res ) {
        let invoiceDetails = [];
        for (var data of this.purchaseInvoice.purchaseInvoice) {
          data.paidAmount = 0;
          invoiceDetails.push(data);
        }

        let generalLederInput = new AddGeneralLedgarInputDto();
        generalLederInput.processType = "PurchaseInvoice";
        generalLederInput.invoiceId = res;

        this._generalLedgerService.addLedger(generalLederInput).subscribe((res) => {

        },
          ({ error }) => {
            this.notify.error("Cannot Add Record In Ledger");
          });

          

        this._chartOfAccoountService.changeCoaBalance("PurchaseInvoice", 0, 0, invoiceDetails).subscribe((res) => { });

        if (!this.invoiceId) {
          this.clearForm();
          this.notify.info(this.l("Saved Successfully"));
        } else {
          this.notify.info(this.l("Update Successfully"));
        }
        this.isActive = false;
      }
      this._spinner.hide();
    })
  }

  clearForm() {
    this.items = [];
    this.items.push({ name: '', description: '', quantity: 1, salePrice: 0, saleTax: 0, discount: 0, amount: 0, customer: 0 });
    this.selectedProduct = null;

    this.accountItems = [];
    this.accountItems.push({ invoiceDetailId: 0, account: 0, description: '', amount: 0, customer: 0 });
    this.totalSalePrice = 0;
    this.changeAmount = 0;
    this.purchaseInvoice.refNo = null;
  }


}
