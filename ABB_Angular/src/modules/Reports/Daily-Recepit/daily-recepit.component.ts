import { Component, ElementRef, Injector, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { NgxSpinnerService } from 'ngx-spinner';
import { ChartOfAccountsServiceProxy, DailyReceiptServiceProxy, PaymentMethodServiceProxy } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs';
import { MatTableDataSource } from '@angular/material/table';
import * as moment from 'moment';
import * as Papa from 'papaparse';
import { saveAs } from 'file-saver';
import { jsPDF } from "jspdf";

@Component({
  selector: 'app-daily-recepit',
  templateUrl: 'daily-recepit.component.html',
  styleUrls: ['daily-recepit.component.scss'],
})

export class DailyReceiptComponent extends AppComponentBase implements OnInit {
  pageLength: any;
  paymentMethods: string[] = [];
  grandTotal: any;
  notFound: boolean = false;
  constructor(
    private injector: Injector,
    private _router: Router,
    private _reportDailyReceiptService: DailyReceiptServiceProxy,
    public _paymentService: PaymentMethodServiceProxy,
    public _chartOfAccoountService: ChartOfAccountsServiceProxy,
    private spinner: NgxSpinnerService,
  ) {
    super(injector);

  }
  @ViewChild('printArea', { static: true }) printArea: ElementRef;
  active: boolean = false;
  dailyReceiptDS: any;
  dailyReceiptList: any;
  accountsList: any;
  paymentMethodList: any;
  startDate: any;
  endDate: any;
  accountId: any;
  paymentMethodId: any;
  @ViewChild(MatSort) sort: MatSort;
  @ViewChild("RecepitPaginator") RecepitPaginator: MatPaginator;
  @ViewChild('reportContent') reportContent: ElementRef;
  selectedHeaders: string[] = [];

  columns: string[] = ['RecepitDate', 'CustomerName', 'Company', 'Invoice', 'PaymentRecevied', 'Bank', 'CSR'];

  ngOnInit() {
    // this.startDate = "2022-05-08";
    // this.startDate = new Date();
    // this.endDate = new Date();
    this.getRecepitList();
    this.getPaymentMethodList();
    this.getDepositToList();
  }

  getPaymentMethodList() {
    this._paymentService.getAll().subscribe((res) => {
      this.paymentMethodList = res;
      // this.paymentMethodId = res[0].id;
    });
  }

  getDepositToList() {
    this._chartOfAccoountService.getChartOfAccountsForRP().pipe(finalize(() => { }))
      .subscribe((result) => {
        this.accountsList = result;
        // this.accountId = result[0].id;
      });
  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dailyReceiptDS.filter = filterValue.trim().toLowerCase();
  }

  getRecepitList() {
    this.spinner.show();
    const startDateMoment = this.startDate ? moment(this.startDate) : undefined;
    const endDateMoment = this.endDate ? moment(this.endDate) : undefined;
    this._reportDailyReceiptService.getList(startDateMoment, endDateMoment, this.paymentMethodId, this.accountId).pipe(
      finalize(() => { }))
      .subscribe((result) => {
        this.dailyReceiptList = result;
        this.getUniqueReceiptPaymentMethods();
        this.getUniqueReceiptTotal();
        this.dailyReceiptDS = new MatTableDataSource<any>(this.dailyReceiptList); //.reverse()
        this.dailyReceiptDS.paginator = this.RecepitPaginator;
        this.pageLength = this.dailyReceiptList.length;
        if (this.dailyReceiptList.length === 0) {
          this.notFound = true;
        } else {
          this.notFound = false;
        }
        this.dailyReceiptDS.sort = this.sort;
        this.spinner.hide();
      }, ({ error }) => {
        this.spinner.hide();
        this.notify.error("Cannot Retreive Data");
      });
  }

  getUniqueReceiptPaymentMethods() {
    this.paymentMethods = Array.from(new Set(
      this.dailyReceiptList
        .filter(item => item.paymentMethod.startsWith('Total'))
        .map(item => {
          const totalIndex = item.paymentMethod.indexOf('Total');
          if (totalIndex !== -1) {
            const wordsAfterTotal = item.paymentMethod
              .substring(totalIndex + 6) // Skip 'Total' and space
              .split(' ', 3) // Split into an array of up to 3 words
              .filter(word => word !== ''); // Remove empty strings
            return wordsAfterTotal.join(' '); // Join the words back into a string
          }
          return '';
        })
    ));
    if (this.paymentMethods.length === 0) {
      this.selectMethod();
      //this.paymentMethods.push("All");
    }
  }

  selectMethod() {
    this.paymentMethods = [];
    const selectedMethod = this.paymentMethodList.find(obj => obj.id === this.paymentMethodId);
    if (selectedMethod) {
      this.paymentMethods.push(selectedMethod.name);
    }
  }

  getUniqueReceiptTotal() {
    const totalList = Array.from(new Set(
      this.dailyReceiptList
        .filter(item => item.paymentMethod.startsWith('Total'))
        .map(item => item.total) // Split and get the first word
    ));
    this.grandTotal = 0; // Initialize grandTotal before adding values
    for (let item of totalList) {
      this.grandTotal += item;
    }
  }

  search() {
    if (this.endDate) { if (this.startDate > this.endDate) { return this.notify.error(this.l("fromDate must be less then toDate")); } }
    //if (!this.startDate && !this.endDate) { return }
    this.getRecepitList();
  }

  clear() {
    this.accountId = undefined;
    this.paymentMethodId = undefined;
    this.startDate = undefined//new Date();
    this.endDate = undefined//new Date();
    this.grandTotal = 0;
    this.getRecepitList();
  }

  downloadExcel(): void {
    this.spinner.show();
    let dataArr = [];
    // const headerText = 'E Workforce Payroll Inc\nProfit & Loss Account\nFrom Date Period - To Date Period';  
    // dataArr.push({headerText});  
    for (var data of this.dailyReceiptList) {
      let obj = {
        Date: moment(data.paymentDate).isValid() ? moment(data.paymentDate).format('MM/DD/YYYY') : 'Invalid date',
        CustomerName: data.customerName || 'N/A',
        Company: data.company || 'N/A',
        Invoice: data.invoiceNo || 'N/A',
        AmountReceived: data.total || 0,
        Account: data.accountDescription || 'N/A',
        CSR: data.csr || 'N/A'
      };
      dataArr.push(obj);
    }

    const csv = Papa.unparse(dataArr, {
      header: true,
    });

    this.spinner.hide();
    const blob = new Blob([csv], { type: 'text/csv;charset=utf-8' });
    saveAs(blob, 'DailyReceipt.csv');
  }

  downloadPDF() {
    this.spinner.show();
    const DATA = this.printArea.nativeElement;
    const doc: jsPDF = new jsPDF("p", "mm", "a4");
    doc.html(DATA, {
      callback: (doc) => {
        // doc.output("dataurlnewwindow");
        doc.save('dailyReceipt.pdf');
        this.spinner.hide();
      }
    });
  }

  print() {
    let styles = `
    <style>
       table{
        border-collapse: collapse;
        width: 100%;
        font-family: Arial, Helvetica, sans-serif;
       }
       
       th,td{
        border: 1px solid black;
        padding: 0.3rem;
       }

       td{
        font-size: 0.8rem;
       }

       th{
        background-color: #e2e8f0
       }

       .title{
        text-align: center;
        padding: 5px 0;
        background: #e5e7eb;
        line-height: 0px;
       }
       .grand-total-footer{
        text-align: right;
        background-color: #e5e7eb;
        font-weight: 600;
        font-size: 1rem;
        color: black;
        padding: 10px 0px 10px 49rem;
        border-top: 1px solid #ccc;
        position: sticky;
        bottom: 0;
        z-index: 1;
       }
       
       @media print{
         @page {
            padding: 0;
            margin: 0;
            size: landscape;
         }
       }
    </style>
    `;
    let headingDiv = document.getElementsByClassName('title')[0];
    headingDiv.setAttribute('class', 'title');

    let footerDiv = document.getElementsByClassName('grand-total-footer')[0];
    footerDiv.setAttribute('class', 'grand-total-footer');

    const printContent = this.printArea.nativeElement.innerHTML;
    // Open a new window with the print content
    const printWindow = window.open('', '_blank');
    printWindow.document.write('<html><head><title>Print</title></head><body>');
    printWindow.document.write(printContent);
    printWindow.document.write('</body></html>')
    printWindow.document.write(styles);
    printWindow.document.close();

    // Print the new window and close it after printing
    printWindow.print();
    // printWindow.close();
  }

}



















