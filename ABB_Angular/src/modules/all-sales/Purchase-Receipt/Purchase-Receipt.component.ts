import { Component, Injector, OnInit } from '@angular/core';
import { ContactInfoDto, CustomerDto, CustomerServiceProxy, InvoiceDetailDto, InvoiceServiceProxy, ProductServiceDto, ProductServiceServiceProxy, EstimateServiceProxy, ChartOfAccountsServiceProxy, ChartOfAccountDto, SalesReceiptServiceProxy, GeneralPaymentMethodDto, PaymentMethodServiceProxy, ChargeCardDto, VendorDto, PurchaseReceiptServiceServiceProxy, VenderServiceProxy, SavePurchaseReceiptDto, PurchaseInvoiceServiceServiceProxy, SaveReceivedPayment, ReceviedPayment, CompanyServiceProxy } from '@shared/service-proxies/service-proxies';
import { ENTER, COMMA } from '@angular/cdk/keycodes';
import * as moment from 'moment';
import { AppComponentBase } from '@shared/app-component-base';
import { ActivatedRoute } from '@angular/router';

import { CreateChartOfAccountComponent } from 'modules/chart-of-account/create-chart-of-account/create-chart-of-account.component';
import { MatDialog } from '@angular/material/dialog';

import { CreatePaymentMethodComponent } from 'modules/Payement-Method/create-Payment-Method/create-Payement-Method.component';
import { CreditCardComponent } from '../credit-card-modal/credit-card.component';
import { NgxSpinnerService } from 'ngx-spinner';
import { MatChipInputEvent } from '@angular/material/chips';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-Purchase-Receipt',
  templateUrl: 'Purchase-Receipt.component.html',
  styleUrls: ['Purchase-Receipt.component.scss'],
})

export class PurchaseReceiptComponent extends AppComponentBase implements OnInit {
  isActive: boolean = false;
  constructor(
    private injector: Injector,
    public _customerServiceProxy: CustomerServiceProxy,
    public _productServiceServiceProxy: ProductServiceServiceProxy,
    private _invoiceServiceProxy: InvoiceServiceProxy,
    public _chartOfAccoountService: ChartOfAccountsServiceProxy,
    private _activatedRoute: ActivatedRoute,
    public _paymentService: PaymentMethodServiceProxy,
    private _Salereceipt: SalesReceiptServiceProxy,
    private _purchaseReceipt: PurchaseReceiptServiceServiceProxy,
    private _vendor: VenderServiceProxy,
    public _companyService: CompanyServiceProxy,
    private _purchaseInvoiceService: PurchaseInvoiceServiceServiceProxy,
    public _dialog: MatDialog,
    private _spinner: NgxSpinnerService
  ) {
    super(injector);
    this.intialization();
  }

  keyword: string

  filterType: string = 'Name';
  customers: CustomerDto[] = [];

  vendors: VendorDto[] = [];
  selectedProduct: any;
  productServiceDto: ProductServiceDto[] = [];
  // createSalesReceiptKey: CreateSalesReceiptDto;
  //  createSalesReceiptKey: CreateSalesReceiptDto;
  createPRKey: SavePurchaseReceiptDto = new SavePurchaseReceiptDto();
  DepositToList: ChartOfAccountDto[] = [];
  generalPayementMethodDto: GeneralPaymentMethodDto[] = [];
  signleCalculatedAmount: number = 0;
  EstimationDate: any;
  PurchaseReceiptDate: any = new Date();
  SaleReceiptDate: any;

  discountInAmount;
  referenceNo: any;
  refDepositToAccountId: any;
  refPaymentMethodID: any;
  refCashEquivalentsAccountId: any
  customerId = 0;
  companyName: string;
  // saleReceipt = new SalesReceiptDto();

  InvoiceDetail: InvoiceDetailDto[] = [];

  //  SalesReceipt = new CreateSalesReceiptDto();

  Customer = new ContactInfoDto();
  ProductService = new ProductServiceDto();
  name: any;
  showCheckNo: boolean = false;
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
    discountAmount: 0
  }];

  ngOnInit() {
    this._spinner.show();
    // this.customerList();
    this.vendorlist();
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
        if (arg.length > 0) {
          this.refCashEquivalentsAccountId = arg[0].id;
        }
      });
  }
  getPaymentMethodList() {
    this._paymentService
      .getAll()
      .subscribe((arg) => {
        this.generalPayementMethodDto = arg
        if (arg.length > 0) {
          this.refPaymentMethodID = arg[0].id;
        }
      });
  }
  selectPaymentType(item: number) {
    var paymentType = this.generalPayementMethodDto.find((obj) => obj.id === item);
    if (item == 9) {
      this.showCheckNo = true;
    }
    else {
      this.showCheckNo = false;
    }
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
      if (result) { this.createPRKey.chargeCard = result; }
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
    //   this.createSalesReceiptKey = new CreateSalesReceiptDto();
    //  // this.SalesReceipt = new CreateSalesReceiptDto();
    //   this.createSalesReceiptKey.salesReceipt = new SalesReceiptDto();
    //   this.createPRKey..invoiceDetails = [];
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
          discountAmount: 0
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
        obj.discountAmount = mappedItems[i].discountAmount;

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
        discountAmount: 0
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
    if (customer) {
      this.addEmail({ value: customer.email } as MatChipInputEvent, 1);
    }
    //get getCompanyName
    this.getCompany(customerId);

  }

  //   getCompanyName(customerId: number)
  //   {
  // this._estimateServiceProxy.getCompanyName(customerId).subscribe(x=>{
  //   this.companyName=x;

  // })
  //   }

  // Get data lists
  // customerList() {
  //   this._customerServiceProxy.getAll(this.keyword, this.filterType, 0, 10).pipe(
  //     finalize(() => { }))
  //     .subscribe((result) => {
  //       this.customers = result.items;

  //       if (this.customerId) {
  //         const Datalist = this.customers.find((obj) => obj.id === this.customerId);
  //         if (Datalist) {
  //           this.saleReceipt.refCustomerID = Datalist.id;
  //           this.addEmail({ value: Datalist.email } as MatChipInputEvent, 1);
  //         }
  //       }
  //     });
  // }



  //Get data lists

  vendorlist() {
    this._vendor.getAll().subscribe((result) => {
      this.vendors = result;
      this._spinner.hide();

    })




  }

  getCompany(companyId: number) {
    this._companyService.get(companyId).pipe(
      finalize(() => { }))
      .subscribe((result) => {
        this.companyName = result.name;
      });
  }


  productServiceList() {
    this._productServiceServiceProxy.getAllExpense().subscribe((result) => (
      this.productServiceDto = result
    ));
  }

  // Save
  save(type?: any) {

    this.isActive = true;
    if (!this.createPRKey.refSupplierId) {
      this.isActive = false;
      return this.notify.error(this.l("please select any vendor"));
    }
    if (!this.referenceNo) {
      this.isActive = false;
      return this.notify.error(this.l("please add reference number"));
    }
    if (!this.selectedProduct) {
      this.isActive = false;
      return this.notify.error(this.l("please select at least one product"));
    }
    if (!this.refPaymentMethodID) {
      this.isActive = false;
      return this.notify.error(this.l("please select payment method"));
    }
    if (!this.refCashEquivalentsAccountId) {
      this.isActive = false;
      return this.notify.error(this.l("please select cash account"));
    }


    this._spinner.show();
    this.createPRKey.refInvoiceType = 7;
    let d = this.PurchaseReceiptDate;

    this.createPRKey.purchaseReceiptDate = moment(new Date(d.getFullYear(), d.getMonth(), d.getDate(), d.getHours(), d.getMinutes() - d.getTimezoneOffset()).toISOString());
    this.createPRKey.referenceNo = this.referenceNo;

    this.createPRKey.puchaseReceiptDto = this.InvoiceDetail;
    this.createPRKey.refPaymentMethodID = this.refPaymentMethodID;
    this.createPRKey.refCashEquivalentsAccountId = this.refCashEquivalentsAccountId;
    this.createPRKey.refDepositToAccountId = this.refDepositToAccountId;
    this.createPRKey.total = this.totalSalePrice;
    this._purchaseReceipt.savePurchaseReceipt(this.createPRKey).subscribe((result) => {

      let invoiceDetails = [];
      for (var data of this.createPRKey.puchaseReceiptDto) {
        let obj = {
          amount: data.amount,
          paidAmount: data.amount,
          refProducID: data.refProducID,
        }
        invoiceDetails.push(obj);
      }

      this._chartOfAccoountService.changeCoaBalance("PurchaseInvoice", 0,0, invoiceDetails).subscribe((res) => {

        let key = new SaveReceivedPayment();

        let recPayments = [];

        for (var data of this.createPRKey.puchaseReceiptDto) {
          let obj = new ReceviedPayment();
          obj.isCheck = true;
          obj.openBalance = data.amount;
          obj.paidAmount = data.amount;
          obj.refProducID = data.refProducID;
          recPayments.push(obj);
        }

        key.emails = [];
        key.paymentDate = moment(new Date());
        key.receivedPayments = recPayments;
        key.refDepositToAccountId = this.refCashEquivalentsAccountId;
        key.refInvoiceType = 5;
        key.refPaymentMethodID = this.createPRKey.refPaymentMethodID;
        key.refSupplierID = this.createPRKey.refSupplierId;
        key.total = this.createPRKey.total;
        key.invoiceID = null;


        this._purchaseInvoiceService.savePurchasePayment(0, key).subscribe(
          (res2) => {
            let invoiceDetails = [];
            for (var i = 0; i < key.receivedPayments.length; i++) {
              let index = i;
              let obj = {
                refProducID: key.receivedPayments[i].refProducID,
                amount: 0,
                paidAmount: key.receivedPayments[index].paidAmount
              }
              invoiceDetails.push(obj);

              if (index == key.receivedPayments.length - 1) {
                this._chartOfAccoountService.changeCoaBalance("PurchasePayment", key.refDepositToAccountId, 0, invoiceDetails).subscribe((res) => { });
              }
            }
          });
        this.resetForm();
        this.notify.info(this.l("SavedSuccessfully"));
        this.isActive = false;
        this._spinner.hide();
      });
    });


    if (type === "print") {
      window.print();
    }
  }

  resetForm() {
    this.createPRKey.refSupplierId = null
    this.items = [];
    this.items = [{
      name: '',
      description: '',
      quantity: 1,
      salePrice: 0,
      saleTax: 0,
      discount: 0,
      amount: 0,
      discountAmount: 0
    }];
    this.PurchaseReceiptDate = new Date();
    this.referenceNo = null;
    this.selectedProduct = null;
    this.createPRKey.note = "";
  }

}
