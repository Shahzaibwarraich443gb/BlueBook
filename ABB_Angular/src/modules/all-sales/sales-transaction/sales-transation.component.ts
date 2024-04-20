import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { CalenderServiceServiceProxy, CustomerServiceProxy, InvoiceServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/app-component-base';
import { ActivatedRoute, Router } from '@angular/router';
import { MatDatepicker } from '@angular/material/datepicker';
import * as moment from 'moment';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort, Sort } from '@angular/material/sort';
import { MatPaginator } from '@angular/material/paginator';
import { finalize } from 'rxjs';
import { MatDialog } from '@angular/material/dialog';
import { ViewHistoryComponent } from '../view-history/view-history.component';
import { EmailComponent } from '../email-modal/email.component';
import { NgxSpinnerService } from 'ngx-spinner';
//import { NgxPrintElementService } from 'ngx-print-element';

@Component({
  selector: 'app-sales-transation',
  templateUrl: 'sales-transaction.component.html',
  styleUrls: ['sales-transation.component.scss']
})

export class SalesTransactionComponent extends AppComponentBase implements OnInit {
  constructor(
    private injector: Injector,
    public _customerServiceProxy: CustomerServiceProxy,
    private _activatedRoute: ActivatedRoute,
    public _dialog: MatDialog,
    //  public print: NgxPrintElementService,
    private _calendarService: CalenderServiceServiceProxy,
    private spinner: NgxSpinnerService,
    private _invoiceService: InvoiceServiceProxy, private _router: Router,
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

  dataSource: any;
  fromDate: string = null;
  active: boolean = false;
  toDate: string = null;
  customerTransactions = [];
  displayedColumns: string[] = ['Date', 'Type', 'Invoice', 'Product', 'Company', 'Customer', 'Balance', 'Total', 'Status', 'Action'];

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  @ViewChild('releasedAtPicker') releasedAtPicker: MatDatepicker<Date>;
  @ViewChild('releasedAtPicker2') releasedAtPicker2: MatDatepicker<Date>;

  ngOnInit(): void {
    this.getCustomerTransactions();
  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();
  }

  getCustomerTransactions() {
    this.spinner.show();
    this._invoiceService.getAllTransactions().subscribe((res) => {
      this.customerTransactions = res;
      this.spinner.hide();
      for (var data of this.customerTransactions) {
        data.invoiceDateAlt = moment(data.creationTime).format('MM/DD/YYYY');
      }

      this.dataSource = new MatTableDataSource<any>(this.customerTransactions);
      this.dataSource.paginator = this.paginator;
      if (this.active) {
        this.search();
      }
    });
  }

  clear() {
    this.fromDate = null;
    this.toDate = null;
    this.active = false;
    this.getCustomerTransactions();
  }

  isActive(isactive: boolean) {
    this.active = isactive;
    if (this.toDate) { if (this.fromDate > this.toDate) { return this.notify.warn(this.l("FromDate must be less then ToDate")); } }
    if (!this.fromDate && !this.toDate) { return }
    this.getCustomerTransactions();
  }

  search() {
    if (this.fromDate != null && this.toDate === null) {
      this.customerTransactions = this.customerTransactions.filter(item => item.creationTime >= this.fromDate);
    }
    else if (this.toDate != null && this.fromDate === null) {
      this.customerTransactions = this.customerTransactions.filter(item => item.creationTime <= this.toDate);
    }
    else {
      this.customerTransactions = this.customerTransactions.filter(item => item.creationTime >= this.fromDate && item.creationTime <= this.toDate);
    }
    this.dataSource = new MatTableDataSource<any>(this.customerTransactions);
    this.dataSource.paginator = this.paginator;
  }

  customerTransaction(item: any): void {
    this._router.navigate(['/app/customer-detail-view/' + item.refCustomerId]);
  }

  UpdateInvoice(item: any) {
    if (item.type === "Invoice") {
      this._router.navigate(['/app/invoices/' + item.id]);
    }
    else if (item.type === "Credit Note") {
      this._router.navigate(['app/credit-note' + item.id]);
    }
      else if(item.type==="Estimate")
      {
        this._router.navigate(['app/Estimate' + item.id]);
      }
    

    else {
      this._router.navigate(['/app/all-sales/received-payment/' + item.id]);
    }
  }
  public viewHistoryModal(item?: any): void {
    const dialogRef = this._dialog.open(ViewHistoryComponent, {
      data: { item: item },
    });
    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        console.log(result);
      }
      dialogRef.close();
    });
  }

  compare(a: number | string, b: number | string, isAsc: boolean): any {
    return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
  }

  sortTransactions(sort: Sort): void {
    const data = this.customerTransactions.slice();
    if (!sort.active || sort.direction === 'asc') {
      this.dataSource.data = data;
      return;
    }

    this.dataSource.data = data.sort((a, b) => {
      const isAsc = sort.direction === 'asc';
      return this.compare(a.Roles, b.Roles, isAsc);

    });

  }

  InvoiceDetail(element: any) {
    this._router.navigate(['/app/invoice/detail/' + element.id]);
  }

  isDetailEditable(element: any): boolean {
    if (element.orignalInvoiceNo.startsWith('IN-') && element.status === 'Open') {
      return true;
    } else if (element.orignalInvoiceNo.startsWith('IN-') && element.status === 'Partial') {
      return true;
    } else if (element.orignalInvoiceNo.startsWith('IN-') && element.status === 'Paid') {
      return true;
    } else {
      return false;
    }
  }
  isPrintEditable(element: any): boolean {
    if (element.orignalInvoiceNo.startsWith('IN-') && element.status === 'Open') {
      return true;
    } else if (element.orignalInvoiceNo.startsWith('RP-') && element.status === 'Closed') {
      return true;
    } else if (element.orignalInvoiceNo.startsWith('IN-') && element.status === 'Partial') {
      return true;
    } else if (element.orignalInvoiceNo.startsWith('IN-') && element.status === 'Paid') {
      return true;
    } else if (element.orignalInvoiceNo.startsWith('CN-') && element.status === 'Closed') {
      return true;
    } else if (element.orignalInvoiceNo.startsWith('ET-') && element.status === 'Closed') {
      return true;
    } else {
      return false;
    }
  }

  isReminderEditable(element: any): boolean {
    if (element.orignalInvoiceNo.startsWith('IN-')) {
      return true;
    } else {
      return false;
    }
  }
  isEmailEditable(element: any): boolean {
    if (element.orignalInvoiceNo.startsWith('IN-')) {
      return true;
    } else if (element.orignalInvoiceNo.startsWith('RP-') && element.status === 'Closed') {
      return true;
    } else if (element.orignalInvoiceNo.startsWith('CN-') && element.status === 'Closed') {
      return true;
    } else if (element.orignalInvoiceNo.startsWith('ET-') && element.status === 'Closed') {
      return true;
    } else {
      return false;
    }
  }

  isInvoiceEditable(element: any): boolean {
    // Modify the condition based on your requirements
    if (element.orignalInvoiceNo.startsWith('IN-') && element.status === '--') {
      return true;
    } else if (element.orignalInvoiceNo.startsWith('CN-') && element.status === '--') {
      return true;
    } else if (element.orignalInvoiceNo.startsWith('ET-') && element.status === '--') {
      return true;
    } else {
      return false;
    }
  }

  sendReminder(element) {
    this._calendarService.addInvoiceReminder(element.id).pipe(
      finalize(() => { })).subscribe((res) => {
        if (res === "success") {
          abp.notify.success(this.l('Your Reminder has been sent'));
        } else {
          abp.notify.error(this.l('Invoice already exists in the system reminders'));
        }
      });
  }

  // invoicePrint(item){
  //   this._router.navigate(['/app/print-received-payment/'], { queryParams: item });
  // }
  invoicePrint(item) {
    // Assuming `item` is an object containing query parameters
    const queryParams = item;
    var urlTree = null;
    var url = null;
    if (item.type == 'Credit Note') {
      urlTree = this._router.createUrlTree(['/app/print-Credit-Note/'], { queryParams });
      url = this._router.serializeUrl(urlTree);
    }
    else if (item.type == 'Invoice') {
      urlTree = this._router.createUrlTree(['/app/print-invoice/'], { queryParams });
      url = this._router.serializeUrl(urlTree);
    } else {
      urlTree = this._router.createUrlTree(['/app/print-received-payment/'], { queryParams });
      url = this._router.serializeUrl(urlTree);
    }
    window.open(url, '_blank');
  }


  public openEmailModal(item: any): void {
    const dialogRef = this._dialog.open(EmailComponent, {
      data: { items: item },
    });
    dialogRef.afterClosed().subscribe((result) => {
      dialogRef.close();
    });
  }

  DeleteInvoice(item: any) {
    abp.message.confirm(
      this.l(`Are you sure want to delete ` + item.invoiceCode),
      undefined,
      (result: boolean) => {
        if (result) {
          this._invoiceService.deleteTransactionDetail(item.id).pipe(
            finalize(() => { }))
            .subscribe((result) => {
              if (result === 1) {
                this.getCustomerTransactions();
                abp.notify.success(this.l('Successfully Deleted'));
              } else {
                abp.notify.error(this.l('Invoice Already Deleted'));
              }
            });
        }
      }
    );
  }
}

