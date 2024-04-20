import { Component, ElementRef, EventEmitter, Injector, Input, OnInit, Output, ViewChild } from '@angular/core';
import { MatChipInputEvent } from '@angular/material/chips';
import { ContactInfoDto, CreateInvoiceDto, CustomerDto, CustomerServiceProxy, InvoiceDetailDto, InvoiceDto, InvoiceServiceProxy, ProductServiceDto, ProductServiceServiceProxy, EstimateServiceProxy, InvoiceType, ChartOfAccountsServiceProxy, ChartOfAccountDto, ReceivedPaymentServiceServiceProxy, SalesReceiptServiceProxy, CreateSalesReceiptDto, SalesReceiptDto, GeneralPaymentMethodDto, PaymentMethodServiceProxy, ChargeCardDto, CompanyServiceProxy, SaveCreditNoteDto, CreditNoteServiceServiceProxy, InvoiceStatus, RecurringInvoiceDetailDto, ReceviedPayment, EmployeActivitiesServiceProxy, AuditLogsDto } from '@shared/service-proxies/service-proxies';
import { ENTER, COMMA, I, SPACE } from '@angular/cdk/keycodes';
import { finalize } from 'rxjs';
import * as moment from 'moment';
import { NotifyService } from 'abp-ng2-module';

import { AppComponentBase } from '@shared/app-component-base';

import { ActivatedRoute, Router } from '@angular/router';
import { MatPaginator } from '@angular/material/paginator';
import * as Papa from 'papaparse';
import { NgxSpinnerService } from 'ngx-spinner';
import { DailyReceiptServiceProxy } from '@shared/service-proxies/service-proxies';

import { MatTableDataSource } from '@angular/material/table';
import { MatDialog } from '@angular/material/dialog';
import { MatSort, Sort } from '@angular/material/sort';

import { saveAs } from 'file-saver';
import jsPDF from 'jspdf';


@Component({
  selector: 'app-Employee-Activities',
  templateUrl: 'Employee-Activities.component.html',
  styleUrls: ['Employee-Activities.component.scss'],
})

export class EmployeeActivitiesComponent extends AppComponentBase implements OnInit {
  active: boolean = false;
  constructor(
    private injector: Injector,
    private spinner: NgxSpinnerService,
    public _employeeActivity: EmployeActivitiesServiceProxy,
    public _dialog: MatDialog,
    private _router: Router,

  ) {
    super(injector);

  }
  @ViewChild('printArea', { static: true }) printArea: ElementRef;
  createALKey: AuditLogsDto = new AuditLogsDto();
  columns: string[] = ['logDate', 'EmployeerName', 'Action', 'Operation', 'OperationBy', 'Customer#', 'CustomerName', 'CompanyName'];
  notFound: boolean = false;
  AuditLogDS: any;
  AuditLogList: any;
  selectedToDate: any
  MethodName: any
  selectedFromDate: any
  @ViewChild(MatSort) sort: MatSort;
  @ViewChild("RecepitPaginator") RecepitPaginator: MatPaginator;



  ngOnInit() {
    // this.spinner.show();
    this.selectedFromDate = new Date();
    this.selectedFromDate.setDate(this.selectedFromDate.getDate() - 1);
    this.selectedToDate = new Date();
    this.getList();
  }

  getList() {
    this.spinner.show();
    const startDateMoment = this.selectedFromDate ? moment(this.selectedFromDate) : undefined;
    const endDateMoment = this.selectedToDate ? moment(this.selectedToDate) : undefined;
    this._employeeActivity.getList(startDateMoment, endDateMoment, this.MethodName).pipe(
      finalize(() => { }))
      .subscribe((result) => {
        this.AuditLogList = result;
        this.AuditLogDS = new MatTableDataSource<any>(this.AuditLogList.reverse());
        this.AuditLogDS.paginator = this.RecepitPaginator;
        this.AuditLogDS.sort = this.sort;
        this.spinner.hide();
      });
  }

  // isActive(isactive: boolean) {
  //   this.active = isactive;
  //   if (this.selectedToDate) { if (this.selectedFromDate > this.selectedToDate) { return this.notify.warn(this.l("selectedFromDate must be less then selectedToDate")); } }
  //   if (!this.selectedFromDate && !this.selectedToDate) { return }
  //   this.getRecepitList();
  // }
  search() {
    if (this.selectedToDate) { if (this.selectedFromDate > this.selectedToDate) { return this.notify.error(this.l("fromDate must be less then toDate")); } }
    //if (!this.startDate && !this.endDate) { return }
    this.getList();
  }
  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.AuditLogDS.filter = filterValue.trim().toLowerCase();
  }
  downloadPdf() { //downloadpdf
    this.spinner.show();
    const DATA = this.printArea.nativeElement;
    const doc: jsPDF = new jsPDF("p", "mm", "a4");
    doc.html(DATA, {
      callback: (doc) => {
        // doc.output("dataurlnewwindow");
        doc.save('EmployeActivities.pdf');
        this.spinner.hide();
      }
    });
  }
  downloadExcel(): void {
    this.spinner.show();
    let dataArr = [];
    // const headerText = 'E Workforce Payroll Inc\nProfit & Loss Account\nFrom Date Period - To Date Period';  
    // dataArr.push({headerText});  
    for (var data of this.AuditLogList) {
      let obj = {

        EmployeeName: data.employeeName || 'N/A',
        Action: data.appLog || 'N/A',
        Operation: data.operation || 'N/A',
        OperationBy: data.userName || 'N/A',
        CustomerNo: data.customerNo,
        CustomerName: data.customerName,
        CompanyName: data.companyName
      };
      dataArr.push(obj);
    }

    const csv = Papa.unparse(dataArr, {
      header: true,
    });

    this.spinner.hide();
    const blob = new Blob([csv], { type: 'text/csv;charset=utf-8' });
    saveAs(blob, 'AuditLog.csv');
  }
  print() {
    const printContent = this.printArea.nativeElement.innerHTML;
    // Open a new window with the print content
    const printWindow = window.open('', '_blank');
    printWindow.document.write('<html><head><title>Print</title></head><body>');
    printWindow.document.write(printContent);
    printWindow.document.write('</body></html>');
    printWindow.document.close();

    // Print the new window and close it after printing
    printWindow.print();
    // printWindow.close();
  }
  clear() {
    //this.accountId = undefined;
    //this.paymentMethodId = undefined;
    this.selectedFromDate = undefined//new Date();
    this.selectedToDate = undefined//new Date();

    this.getList();
  }

}



















