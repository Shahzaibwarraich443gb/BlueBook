import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { MatChipInputEvent } from '@angular/material/chips';
import { AddGeneralLedgarInputDto, CompanyServiceProxy, ContactInfoDto, CustomerDto, CustomerServiceProxy, GeneralLedgerServiceProxy, GeneralPaymentMethodDto, InvoiceDto, InvoiceServiceProxy, InvoiceType, PaymentMethodServiceProxy, ProductServiceServiceProxy, ReceivedPaymentServiceServiceProxy, ReceviedPayment, SaveReceivedPayment } from '@shared/service-proxies/service-proxies';
import { ENTER, COMMA, SPACE } from '@angular/cdk/keycodes';
import { finalize } from 'rxjs';
import { AppComponentBase } from '@shared/app-component-base';
import { MatDatepicker } from '@angular/material/datepicker';
import { MatDialog } from '@angular/material/dialog';
import { CreatePaymentMethodComponent } from 'modules/Payement-Method/create-Payment-Method/create-Payement-Method.component';
import { ChartOfAccountsServiceProxy } from '@shared/service-proxies/service-proxies';
import { ChartOfAccountDto } from '@shared/service-proxies/service-proxies';
import { CreateChartOfAccountComponent } from 'modules/chart-of-account/create-chart-of-account/create-chart-of-account.component';
import { CreditCardComponent } from '../credit-card-modal/credit-card.component';
import * as moment from 'moment';
import { ActivatedRoute, Router } from '@angular/router';
// import { NgxPrintElementService } from 'ngx-print-element';
import { NgxSpinnerService } from 'ngx-spinner';

@Component({
  selector: 'app-received-payments',
  templateUrl: 'received-payments.component.html',
  styleUrls: ['received-payments.component.scss']
})

export class ReceivedPaymentsComponent extends AppComponentBase implements OnInit {
  customerName: string;
  isActive: boolean = false;

  constructor(
    private injector: Injector,
    public _dialog: MatDialog,
    public _customerServiceProxy: CustomerServiceProxy,
    public _productServiceServiceProxy: ProductServiceServiceProxy,
    private _receviedPaymentServiceProxy: ReceivedPaymentServiceServiceProxy,
    private _invoiceServiceProxy: InvoiceServiceProxy,
    public _paymentService: PaymentMethodServiceProxy,
    public _chartOfAccoountService: ChartOfAccountsServiceProxy,
    public _companyService: CompanyServiceProxy,
    private _activatedRoute: ActivatedRoute,
    private _router: Router,
    // public print: NgxPrintElementService,
    private _generalLedgerService: GeneralLedgerServiceProxy,
    private spinner: NgxSpinnerService
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
  updateSingle: any[] = [];
  paidAmount: any[] = [];
  totalAmount: number = 0;
  creditAmount: number = 0;
  dataList: Array<any> = [];
  customerId: number;
  totalOpenBalance: any;
  isSendLater: boolean = false;
  isEdit: boolean = false;
  previousAmount: number = 0;
  keyword: string;
  filterType: string = 'Name';
  customers: CustomerDto[] = [];
  companyName: string;
  createRPKey: SaveReceivedPayment;
  chargeIsActive = false;
  isDisabled = false;

  payementMethodList: GeneralPaymentMethodDto[] = [];
  DepositToList: ChartOfAccountDto[] = [];

  Invoice = new InvoiceDto();
  receivedPayment = new SaveReceivedPayment();
  selectedItems: ReceviedPayment[] = [];

  @ViewChild('releasedAtPicker') releasedAtPicker: MatDatepicker<Date>;
  @ViewChild('releasedAtPicker2') releasedAtPicker2: MatDatepicker<Date>;

  Customer = new ContactInfoDto();
  name: string;
  invoiceId: number;
  addOnBlur = true;
  readonly separatorKeysCodes = [ENTER, COMMA, SPACE] as const;
  emailList: string[] = [];

  ngOnInit() {
    this.spinner.show();
    this.customerList();
    this.paymentDateAlt = new Date;
    this._activatedRoute.params.subscribe(parms => {
      if (parms.id) {
        this.isEdit = true;
        if (parms.id.includes("_")) {
          let customerId = parms.id.split("_")[1];
          this.receivedPayment.refCustomerID = parseInt(customerId);
          this.getReceivedList(this.receivedPayment.refCustomerID);
        }
        else {
          this.invoiceId = +parms.id;
          this.getInvoiceDetails(this.invoiceId);
        }

      }
    });
    this.getPaymentMethodList();
    this.getDepositToList();
  }

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

  selectCustomerEmail(customerId: number) {
    let customer = this.customers.find((obj) => obj.id === customerId);
    if (customer) {
      this.customerId = customerId;
      this.customerName = customer.name;
      this.emailList = [];
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
  }

  getInvoiceDetails(invoiceId: number) {
    this._invoiceServiceProxy.getReceivedPaymentDetails(invoiceId).pipe(
      finalize(() => { }))
      .subscribe((result) => {
        this.previousAmount = result[0].paidAmount;
        this.dataList = result;
        const data = result[0];
        this.receivedPayment.isSendLater = this.dataList[0].isSendLater;
        this.receivedPayment.refCustomerID = data?.refCustomerID;
        this.customerId = data?.refCustomerID;
        this.isDisabled = true;
        this.receivedPayment.refCompanyID = data?.refCompanyID;
        //this.receivedPayment.paymentDate = data.paymentDate;
        if (data?.paymentDate) {
          this.paymentDateAlt = new Date(data?.paymentDate.format('YYYY-MM-DD'));
          this.receivedPayment.refPaymentMethodID = data?.refPaymentMethodID;
        }
        this.receivedPayment.refDepositToAccountId = data?.refDepositToAccountID;
        this.receivedPayment.referenceNo = data?.refrenceNo;
        this.receivedPayment.invoiceNo = data?.rP_Invoice;
        this.receivedPayment.note = data?.note;
        this.totalOpenBalance = this.dataList.reduce((sum, item, index) => {
          if (item.openBalance === null) {
            item.openBalance = item.origionalAmount;
          }
          item.selected = true;
          this.updateSingle[index] = item.paidAmount;
          this.selectRow(item, index)
          this.singleAmount[index] = item.paidAmount;
          return sum + item.openBalance;
        }, 0);
        //setTimeout(() => {
        this.totalAmount = this.selectedRows.reduce((total, row, index) => {
          return total + (this.singleAmount[index] || 0);
        }, 0);
        //}, 500);
      });
  }


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
        this.spinner.hide();
      });
  }

  getPaymentMethodList() {
    this._paymentService.getAll().subscribe((res) => {
      this.payementMethodList = res;
      if (!this.invoiceId) { this.receivedPayment.refPaymentMethodID = res[0].id; }
    });
  }

  selectPaymentType(item: number) {
    var paymentType = this.payementMethodList.find((obj) => obj.id === item);
    if (paymentType.name.toLowerCase() === "Credit Card".toLowerCase()) {
      this.chargeIsActive = true;
    } else {
      this.chargeIsActive = false;
    }
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
      payment.paidAmount = this.updateSingle[i];
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
    if (this.isDisabled) {
      this.totalAmount = this.selectedRows.reduce((total, row) => {
        var selectedRow = this.dataList.find((item) => item.invoiceNo === row.invoiceNo);
        if (selectedRow && selectedRow.selected) {
          var index = this.dataList.indexOf(selectedRow);
          return total + (this.singleAmount[index] || 0);
        }
        return total;
      }, 0);
    }
    else {
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

  public openCreditCardDialog(id?: number): void {
    const dialogRef = this._dialog.open(CreditCardComponent, {
      data: { totalAmount: this.totalAmount, customerName: this.customerName },
    });
    dialogRef.afterClosed().subscribe((result) => {
      // if (Object.keys(result).length !== 0) {
      if (result) {
        this.receivedPayment.chargeCard = result;
        this.creditAmount = this.totalAmount;
        this.chargeIsActive = false;
      }
      dialogRef.close();
    });
  }

  getReceivedList(customerId: any) {
    this.spinner.show();
    this.selectedRows = [];
    this.receivedPayment.refCustomerID = customerId;
    this.selectedItemRows = [];
    this._invoiceServiceProxy.getReceivedPaymentList(customerId).pipe(
      finalize(() => { }))
      .subscribe((result) => {
        this.spinner.hide();
        this.dataList = result;
        this.totalOpenBalance = this.dataList.reduce((total, item) => total + item.openBalance, 0);
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
    if (!this.receivedPayment.refCustomerID) {
      this.isActive = false;
      return abp.message.error('Please select any customer', 'Something Wrong!!');
    }
    if (!this.receivedPayment.refDepositToAccountId) {
      this.isActive = false;
      return abp.message.error('Please select deposit yo account', 'Something Wrong!!');
    }
    this.isActive = false;
    if (!this.selectedItemRows.length && !this.invoiceId) {
      return abp.message.error('Please select at least one invoice !', 'Something Wrong!!');
    }
    if (this.totalAmount === 0) {
      return abp.message.error('Please add amount greater than zero !', 'Something Wrong!!');
    }

    let paymentDateAlt = this.paymentDateAlt;
    this.createRPKey = this.receivedPayment;
    this.createRPKey.refInvoiceType = InvoiceType._5;
    this.createRPKey.total = this.totalAmount; // this.totalOpenBalance;
    //this.createRPKey.paidAmount = this.totalAmount;
    this.createRPKey.emails = this.emailList;
    this.createRPKey.receivedPayments = this.selectedItemRows;

    if (this.paymentDateAlt) {
      this.createRPKey.paymentDate = moment(new Date(paymentDateAlt.getFullYear(), paymentDateAlt.getMonth(), paymentDateAlt.getDate(), paymentDateAlt.getHours(), paymentDateAlt.getMinutes() - paymentDateAlt.getTimezoneOffset()).toISOString());
    }
    if (this.invoiceId) {
      this.createRPKey.id = this.invoiceId;
    }

    this._receviedPaymentServiceProxy.saveReceivedPayment(0, this.createRPKey).subscribe(
      (res) => {
        let invoiceDetails = [];
        let pAmount = 0;
        for (var i = 0; i < this.createRPKey.receivedPayments.length; i++) {
          let index = i;
          pAmount += (this.isEdit ? this.createRPKey.receivedPayments[i].paidAmount : 0);
          this._invoiceServiceProxy.getInvoiceDetails(this.createRPKey.receivedPayments[i].invoiceID).subscribe((invoiceRes) => {
            for (var data of invoiceRes) {
              let obj = {
                invoiceNo: data.invoiceNo,
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


              this._chartOfAccoountService.changeCoaBalance("ReceivedPayment", this.createRPKey.refDepositToAccountId, this.previousAmount, invoiceDetails).subscribe((res) => {


                if (!this.invoiceId) {
                  this.isActive = false;
                  this.totalAmount = 0;
                  this.notify.info(this.l("Saved Successfully"));
                  this._router.navigate(['/app/customer-detail-view/' + this.receivedPayment.refCustomerID]);
                } else {
                  this.isActive = false;
                  this.selectedRows = [];
                  this.selectedItemRows = [];
                  this.getInvoiceDetails(this.invoiceId);
                  this.notify.info(this.l("Update Successfully"));
                }


              });
            }
          });





        }



      });
  }

  resetForm() {
    this.selectedItemRows = [];
    this.chargeIsActive = false;
    for (let i = 0; i < this.emailList.length; i++) {
      this.emailList.splice(i, 1);
    }
  }

  clearForm() {
    this.receivedPayment.refCustomerID = 0;
    this.selectedItemRows = [];
    this.selectedRows = [];
    this.totalAmount = 0;
    this.emailList = [];
  }

}

