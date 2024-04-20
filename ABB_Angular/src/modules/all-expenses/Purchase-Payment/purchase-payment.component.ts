import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { MatChipInputEvent } from '@angular/material/chips';
import { AddGeneralLedgarInputDto, CompanyServiceProxy, ContactInfoDto, CustomerDto, CustomerServiceProxy, GeneralLedgerServiceProxy, GeneralPaymentMethodDto, InvoiceDto, InvoiceServiceProxy, InvoiceType, PaymentMethodServiceProxy, ProductServiceServiceProxy, PurchaseInvoiceServiceServiceProxy, ReceivedPaymentServiceServiceProxy, ReceviedPayment, SavePurchaseInvoice, SaveReceivedPayment, VenderServiceProxy } from '@shared/service-proxies/service-proxies';
import { ENTER, COMMA } from '@angular/cdk/keycodes';
import { finalize } from 'rxjs';
import { AppComponentBase } from '@shared/app-component-base';
import { MatDatepicker } from '@angular/material/datepicker';
import { MatDialog } from '@angular/material/dialog';
import { CreatePaymentMethodComponent } from 'modules/Payement-Method/create-Payment-Method/create-Payement-Method.component';
import { ChartOfAccountsServiceProxy } from '@shared/service-proxies/service-proxies';
import { ChartOfAccountDto } from '@shared/service-proxies/service-proxies';
import { CreateChartOfAccountComponent } from 'modules/chart-of-account/create-chart-of-account/create-chart-of-account.component';
import * as moment from 'moment';
import { ActivatedRoute, Router } from '@angular/router';
// import { NgxPrintElementService } from 'ngx-print-element';
import { CreditCardComponent } from 'modules/all-sales/credit-card-modal/credit-card.component';
import { NgxSpinnerService } from 'ngx-spinner';

@Component({
  selector: 'app-purchase-payment',
  templateUrl: 'purchase-payment.component.html',
  styleUrls: ['purchase-payment.component.scss']
})

export class PurchasePaymentComponent extends AppComponentBase implements OnInit {
  vendorId: number;
  vendorName: string;
  isActive: boolean = false;
  constructor(
    private injector: Injector,
    public _dialog: MatDialog,
    public _productServiceServiceProxy: ProductServiceServiceProxy,
    public _purchaseInvoiceService: PurchaseInvoiceServiceServiceProxy,
    private _invoiceServiceProxy: InvoiceServiceProxy,
    public _paymentService: PaymentMethodServiceProxy,
    public _chartOfAccoountService: ChartOfAccountsServiceProxy,
    public _companyService: CompanyServiceProxy,
    private _activatedRoute: ActivatedRoute,
    private _router: Router,
    private spinner: NgxSpinnerService,
    // public print: NgxPrintElementService,
    private _generalLedgerService: GeneralLedgerServiceProxy,
    private _vendor: VenderServiceProxy,
  ) {
    super(injector);
  }
  public config = {
    printMode: 'template-popup', // template
    popupProperties: 'toolbar=yes,scrollbars=yes,resizable=yes,top=0,left=0,fullscreen=yes',
    pageTitle: 'Hello World',
    templateString: '<header>I\'m part of the template header</header>{{printBody}}<footer>I\'m part of the template footer</footer>',
    stylesheets: [{ rel: 'stylesheet', href: 'https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0/css/bootstrap.min.css' }],
    styles: ['td { border: 1px solid black; color: green; }', 'table { border: 1px solid black; color: red }', 'header, table, footer { margin: auto; text-align: center; }']
  }
  selectedAll: boolean = false;
  selectedRows: any[] = [];
  selectedItemRows: any[] = [];
  paymentDateAlt: any;

  singleAmount: any[] = [];
  totalAmount: number = 0;
  creditAmount: number = 0;
  dataList: Array<any> = [];

  totalOpenBalance: any;
  keyword: string;
  filterType: string = 'Name';
  customers: CustomerDto[] = [];
  companyName: string;
  createRPKey: SaveReceivedPayment;
  chargeIsActive = false;
  isDisabled = false;

  purchaseInvoice = new SavePurchaseInvoice;
  payementMethodList: GeneralPaymentMethodDto[] = [];
  DepositToList: ChartOfAccountDto[] = [];

  Invoice = new InvoiceDto();
  receivedPayment = new SaveReceivedPayment();
  selectedItems: ReceviedPayment[] = [];

  @ViewChild('releasedAtPicker') releasedAtPicker: MatDatepicker<Date>;
  @ViewChild('releasedAtPicker2') releasedAtPicker2: MatDatepicker<Date>;

  vendors: any;
  name: string;
  invoiceId: number;
  addOnBlur = true;
  emailList: string[] = [];

  ngOnInit() {
    this.spinner.show();
    this.vendorlist();
    this.paymentDateAlt = new Date;
    this._activatedRoute.params.subscribe(parms => {
      if (parms.id) {
        this.invoiceId = +parms.id;
        this.getInvoiceDetails(this.invoiceId);
      }
    });
    this.getPaymentMethodList();
    this.getDepositToList();
  }

  getInvoiceDetails(invoiceId: number) {
    this._invoiceServiceProxy.getReceivedPaymentDetails(invoiceId).pipe(
      finalize(() => { }))
      .subscribe((result) => {
        this.dataList = result;
        const data = result[0];
        this.receivedPayment.refCustomerID = data?.refCustomerID;
        this.isDisabled = true;
        this.receivedPayment.refCompanyID = data?.refCompanyID;
        if (data?.paymentDate) {
          this.paymentDateAlt = new Date(data?.paymentDate.format('YYYY-MM-DD'));
          this.receivedPayment.refPaymentMethodID = data?.refPaymentMethodID;
        }
        this.receivedPayment.refDepositToAccountId = data?.refDepositToAccountID;
        this.receivedPayment.referenceNo = data?.refrenceNo;
        this.receivedPayment.invoiceNo = data?.rP_Invoice;
        this.receivedPayment.note = data?.note;
        this.totalOpenBalance = this.dataList.reduce((sum, item, index) => {
          item.openBalance = item.origionalAmount;
          item.selected = true;
          this.selectRow(item, index)
          return sum + item.openBalance;
        }, 0);
      });
  }

  vendorlist() {
    this._vendor.getAll().subscribe((result) => {
      this.vendors = result;
      this.spinner.hide();
    })
  }

  getReceivedList(vendorId: any) {
    this.spinner.show();
    let vendor = this.vendors.find((obj) => obj.id === vendorId);
    if (vendor) { this.vendorName = vendor.vendorName; }
    this.selectedRows = [];
    this.receivedPayment.refSupplierID = vendorId;
    this.selectedItemRows = [];
    this._invoiceServiceProxy.getPurchasePaymentList(vendorId).pipe(
      finalize(() => { }))
      .subscribe((result) => {
        this.spinner.hide();
        this.dataList = result;
        this.totalOpenBalance = this.dataList.reduce((total, item) => total + item.openBalance, 0);
      });
  }

  getPaymentMethodList() {
    this._paymentService.getAll().subscribe((res) => {
      this.payementMethodList = res;
      if (!this.invoiceId) { this.receivedPayment.refPaymentMethodID = res[0].id; }
    });
  }

  selectPaymentType(item: number) {
    // var paymentType = this.payementMethodList.find((obj) => obj.id === item);
    // if (paymentType.name.toLowerCase() === "Credit Card".toLowerCase()) {
    //   this.chargeIsActive = true;
    // } else {
    //   this.chargeIsActive = false;
    // }
  }

  getDepositToList() {
    this._chartOfAccoountService
      .getChartOfAccountsForRP()
      .pipe(finalize(() => { }))
      .subscribe((result) => {
        this.DepositToList = result;
        if (!this.invoiceId) { this.receivedPayment.refDepositToAccountId = result[0].id; }
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

  public openCreditCardDialog(id?: number): void {
    const dialogRef = this._dialog.open(CreditCardComponent, {
      data: { totalAmount: this.totalAmount, customerName: this.vendorName },
    });
    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.receivedPayment.chargeCard = result;
        this.creditAmount = this.totalAmount;
        this.chargeIsActive = false;
      }
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

  update() {
    abp.message.confirm(
      this.l("Are you sure want to update invoice"),
      undefined,
      (result: boolean) => {
        if (result) {
          this.save();
        }
      }
    );
  }

  save() {
    this.isActive = true;
    if (!this.purchaseInvoice.vendorId) {
      this.isActive = false;
      return this.notify.error(this.l("please select any vendor"));
    }
    if (!this.receivedPayment.refDepositToAccountId) {
      this.isActive = false;
      return this.notify.error(this.l("please select bank account"));
    }
    if (!this.selectedItemRows.length && !this.invoiceId) {
      this.isActive = false;
      return this.notify.error(this.l("please select at least one invoice"));
    }

    let paymentDateAlt = this.paymentDateAlt;
    this.createRPKey = this.receivedPayment;
    this.createRPKey.refInvoiceType = InvoiceType._5;
    this.createRPKey.total = this.totalAmount; // this.totalOpenBalance;
    this.createRPKey.emails = this.emailList;
    this.createRPKey.receivedPayments = this.selectedItemRows;
    if (this.paymentDateAlt) {
      this.createRPKey.paymentDate = moment(new Date(paymentDateAlt.getFullYear(), paymentDateAlt.getMonth(), paymentDateAlt.getDate(), paymentDateAlt.getHours(), paymentDateAlt.getMinutes() - paymentDateAlt.getTimezoneOffset()).toISOString());
    }
    if (this.invoiceId) {
      this.createRPKey.id = this.invoiceId;
    }

    this.spinner.show();
    this._purchaseInvoiceService.savePurchasePayment(0, this.createRPKey).subscribe(
      (res) => {
        if (!this.invoiceId) {
          this.clearForm();
          this.notify.info(this.l("Saved Successfully"));

          let invoiceDetails = [];
          for (var i = 0; i < this.createRPKey.receivedPayments.length; i++) {
            let index = i;
            this._invoiceServiceProxy.getInvoiceDetails(this.createRPKey.receivedPayments[i].invoiceID).subscribe((invoiceRes) => {
              for (var data of invoiceRes) {
                let obj = {
                  refProducID: data.productId,
                  amount: 0,
                  paidAmount: this.createRPKey.receivedPayments[index].paidAmount
                }
                invoiceDetails.push(obj);
              }

              if (index == this.createRPKey.receivedPayments.length - 1) {

                
              let generalLederInput = new AddGeneralLedgarInputDto();
              generalLederInput.processType = "ReceivedPayment";
              generalLederInput.invoiceId = res;

              this._generalLedgerService.addLedger(generalLederInput).subscribe((res) => {

              },
                ({ error }) => {
                  this.notify.error("Cannot Add Record In Ledger");
                });

                this._chartOfAccoountService.changeCoaBalance("PurchasePayment", this.createRPKey.refDepositToAccountId,0, invoiceDetails).subscribe((res) => { });
              }
            });
          }

        } else {
          this.getInvoiceDetails(this.invoiceId);
          this.notify.info(this.l("Update Successfully"));
        }
        this.isActive = false;
        this.spinner.hide();
      });
  }

  clearForm() {
    this.totalAmount = 0;
    this.dataList.length = 0;
    this.purchaseInvoice.vendorId = 0;
    this.selectedItemRows = [];
    this.selectedRows = [];
  }

}

