import { Component, ElementRef, EventEmitter, Injector, Input, OnInit, Output, ViewChild } from '@angular/core';
import { MatChipInputEvent } from '@angular/material/chips';
import { ContactInfoDto, CreateInvoiceDto, CustomerDto, CustomerServiceProxy, InvoiceDetailDto, InvoiceDto, InvoiceServiceProxy, ProductServiceDto, ProductServiceServiceProxy } from '@shared/service-proxies/service-proxies';
import { ENTER, COMMA, I } from '@angular/cdk/keycodes';
import { finalize } from 'rxjs';
import * as moment from 'moment';
import { NotifyService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/app-component-base';
import { setTimeout } from 'timers/promises';
import { ActivatedRoute, Router } from '@angular/router';
import { DecimalPipe } from '@angular/common';
import { AnyCnameRecord } from 'dns';

@Component({
  selector: 'app-invoice-detail',
  templateUrl: 'invoice-detail.component.html',
  styleUrls: ['invoice-detail.component.scss'],
})

export class InvoicesDetailComponent extends AppComponentBase implements OnInit {
  invoiceItem: any;
  constructor(
    private injector: Injector,
    public _customerServiceProxy: CustomerServiceProxy,
    public _productServiceServiceProxy: ProductServiceServiceProxy,
    private _invoiceServiceProxy: InvoiceServiceProxy,
    private _activatedRoute: ActivatedRoute,
    private router: Router
  ) {
    super(injector);
  }
  email: string;
  viewData: any;
  keyword: string
  filterType: string = 'Name';
  customers: CustomerDto[] = [];
  selectedProduct: any;
  productServiceDto: ProductServiceDto[] = [];
  createInvoiceKey: CreateInvoiceDto;
  signleCalculatedAmount: number = 0;
  invoiceDateAlt: any;
  invoiceDueDateAlt: any;
  isDisabled = false;
  customerId = 0;
  Invoice = new InvoiceDto();
  InvoiceDetail: InvoiceDetailDto[] = [];
  @Input() invoice = new CreateInvoiceDto();

  Customer = new ContactInfoDto();
  ProductService = new ProductServiceDto();
  name: any;
  invoiceDetailId: number;
  addOnBlur = true;
  invoiceId: number;
  readonly separatorKeysCodes = [ENTER, COMMA] as const;
  emailList: string[] = [];

  totalSalePrice: number = 0;
  items: any[] = [{
    name: '',
    description: '',
    quantity: 1,
    salePrice: 0,
    saleTax: 0,
    discount: 0,
    amount: 0
  }];

  ngOnInit() {
    this.customerList();
    this.productServiceList();
    this._activatedRoute.params.subscribe(parms => {
      if (parms.id) {
        this.invoiceId = +parms.id;
        this.getInvoiceDetails(this.invoiceId);
      }
    });

    this.calculateDate();
  }
  customerList() {
    this._customerServiceProxy.getAll(this.keyword, this.filterType, 0, 10).pipe(
      finalize(() => { }))
      .subscribe((result) => {
        this.customers = result.items;
        if (this.customerId) {
          const Datalist = this.customers.find((obj) => obj.id === this.customerId);
          if (Datalist) {
            this.Invoice.refCustomerID = Datalist.id;
            //this.addEmail({ value: Datalist.email } as MatChipInputEvent, 1);
          }
        }
      });
  }

  getInvoiceDetails(invoiceId: number) {
    this._invoiceServiceProxy.getInvoiceDetails(invoiceId).pipe(
      finalize(() => { }))
      .subscribe((result) => {
        this.viewData = result[0];
        this.Invoice.refCustomerID = result[0].customerId;
        this.email = result[0].email;
        this.Invoice.invoiceNo = result[0].invoiceNo;
        this.isDisabled = true;
        this.Invoice.refTermID = result[0].refTermId;
        this.calculateDate();
        this.Invoice.isSendLater = result[0].isSendLater;
        this.Invoice.note = result[0].note;
        this.items.splice(0, 1);
        this.items = [];
        result.forEach((item, index) => {
          this.addRowOnProductSelect(item, index, "edit");
        });
      });
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
      obj.invoiceDetailId = item.invoiceDetailId;
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
      this.calculateTotalAmount(i);
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

    this.totalSalePrice = this.InvoiceDetail.reduce((sum, item) => {
      return sum + item.amount;
    }, 0);

    //this.totalSalePrice = +this.decimalPipe.transform(totalamount, '1.2-2');
  }

  calculateDate() {
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
  }
  productServiceList() {
    this._productServiceServiceProxy.getAll().subscribe((result) => (
      this.productServiceDto = result
    ));
  }
  printInvoice() {
    if (this.invoiceId) {
      const list = { id: this.invoiceId }
      const queryParams = list;
      var urlTree = this.router.createUrlTree(['/app/print-invoice/'], { queryParams });
      var url = this.router.serializeUrl(urlTree);
      window.open(url, '_blank');
      // this.router.navigate(['/app/print-invoice/' + this.invoiceId]);
    }
  }

}
