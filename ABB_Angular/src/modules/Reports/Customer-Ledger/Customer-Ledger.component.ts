import { Component, Injector, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort, Sort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { AppComponentBase } from '@shared/app-component-base';
import { ChartOfAccountsServiceProxy, CustomerLedgerServiceProxy, CustomerServiceProxy, GeneralLedgerChartOfAccountData, GeneralLedgerOutputDto, GeneralLedgerServiceProxy, GetGeneralLedgerInputDto } from '@shared/service-proxies/service-proxies';
import * as moment from 'moment';
import { NgxSpinnerService } from 'ngx-spinner';
import { saveAs } from 'file-saver';
import * as Papa from 'papaparse';

@Component({
    selector: 'app-customer-ledger',
    templateUrl: './Customer-Ledger.component.html',
    styleUrls: ['./Customer-Ledger.component.scss']
})
export class CustomerLedgerComponent extends AppComponentBase {

    @ViewChild('ledgerPaginator') ledgerPaginator: MatPaginator;
    @ViewChild(MatSort) sort: MatSort;

    ledgerArr: GeneralLedgerOutputDto[] = [];
    ledgerDS: any;
    customerArr: any[] = [];
    companyArr: any[] = [];
    companyArrFiltered: any[] = [];
    selectedCustomerId: number = 0;
    selectedCompany: string = "all";
    selectedHeaders: string[] = [];
    showTotal: boolean = true;
    startDate: Date = new Date(new Date().setMonth(new Date().getMonth() - 1));
    endDate: Date = new Date();
    startDateAlt: string = moment(this.startDate).format('MM/DD/YYYY');
    endDateAlt: string = moment(this.endDate).format('MM/DD/YYYY');
    LedgerName: string = "Ledger";


    ledgerColumn: string[] = ['dateAlt', 'customerName', 'companyName', 'voucherId', 'description', 'debitAmount', 'creditAmount', 'balance', 'csr'];
    HeadersArr: any[] = [
        { name: 'Date', prop: 'dateAlt' },
        { name: 'Customer Name', prop: 'customerName' },
        { name: 'Company Name', prop: 'companyName' },
        { name: 'Voucher ID', prop: 'voucherId' },
        { name: 'Description', prop: 'description' },
        { name: 'Debit Amount', prop: 'debitAmount' },
        { name: 'Credit Amount', prop: 'creditAmount' },
        { name: 'Balance', prop: 'balance' },
        { name: 'CSR', prop: 'csr' },
    ];

    filteredLedgerColumn: string[] = [];
    constructor(private injector: Injector,
        private customerLedgerService: CustomerLedgerServiceProxy,
        private generalLedgerService: GeneralLedgerServiceProxy,
        private spinner: NgxSpinnerService,
        private customerService: CustomerServiceProxy) {
        super(injector);
    }

    async ngOnInit(): Promise<void> {
        this.spinner.show();
        this.selectedHeaders = this.HeadersArr.map(x => x.prop);
        await this.getHeaders();
        await this.getCustomers();
        await this.getLedgerData();
    }

    async getCustomers(): Promise<void> {
        this.customerService.getCustomersByTenantId().subscribe((res) => {
            this.customerArr = res;
            this.companyArr = [...new Set(res.map(x => x.bussinessName))];
            this.companyArrFiltered = this.companyArr;
        },
            ({ error }) => {
                this.notify.error("Cannot Retrieve Customers");
            });
    }

    async getLedgerData(): Promise<void> {
        this.spinner.show();
        let key = new GetGeneralLedgerInputDto();
        key.startDate = moment(this.startDate);
        key.endDate = moment(this.endDate);
        await this.customerLedgerService.getLedgerForTable(key).subscribe((res) => {
            // let obj = new GeneralLedgerOutputDto();
            // obj.type = 'Sum';
            // obj.chartOfAccountData = [];
            // var coaData = new GeneralLedgerChartOfAccountData();
            // obj.chartOfAccountData.push(coaData);
            // res.push(obj);
            this.ledgerArr = res;
            this.ledgerDS = new MatTableDataSource(this.ledgerArr);
            this.ledgerDS.paginator = this.ledgerPaginator;
            if (this.selectedCustomerId > 0 || this.selectedCompany != "all") {
                this.onFilterChange();
            }
            else {
                this.spinner.hide();
            }
        },
            ({ error }) => {
                this.spinner.hide();
                this.notify.error("Cannot Retreive Data");
            });
    }

    print(): void {
        let tbl = document.createElement('table');
        let tr = document.createElement('tr');

        var headers = this.HeadersArr.filter(x => this.selectedHeaders.some(y => y == x.prop));

        for (var thData of headers) {
            let th = document.createElement('th');
            th.innerHTML = thData.name;
            tr.appendChild(th);
        }

        tbl.appendChild(tr);

        tr = document.createElement('tr');

        let dollarSignProps = ['creditAmount', 'debitAmount', 'balance']

        for (var data of this.ledgerDS.data) {
            var entries = [];


            //sorting with tbl header
            for (var sHeader of this.selectedHeaders) {
                for (var e of Object.entries(data)) {
                    if (sHeader == e[0]) {
                        entries.push(e);
                    }
                }
            }


            for (var i = 0; i < entries.length; i++) {

                if (this.selectedHeaders.some(x => x.toLowerCase() == entries[i][0].toLowerCase())) {
                    let td = document.createElement('td');

                    switch (data.type) {
                        case "Sum":
                            console.log(entries[i]);
                            if (entries[i + 1] &&
                                entries[i][0] != "creditAmount" &&
                                entries[i][0] != "debitAmount" &&
                                (entries[i + 1][0] == "creditAmount" || entries[i + 1][0] == "debitAmount")) {

                                td.innerHTML = 'Total';
                            }
                            else if (entries[i][0].toLowerCase() == "balance" || entries[i][0].toLowerCase() == "companyname") {
                                td.innerHTML = '';
                            }

                            else if (dollarSignProps.some(x => x == entries[i][0])) {
                                td.innerHTML = entries[i][1] != null ? "$" + entries[i][1].toString() : "";
                            }
                            else {
                                td.innerHTML = entries[i][1] != null ? entries[i][1].toString() : "";
                            }
                            break;

                        case "Header":
                            if (this.checkEntry(entries[i][0])) {
                                td.colSpan = this.selectedHeaders.length;
                                td.setAttribute('class', 'tblHeaderLedger');
                                td.innerHTML = `
                                <div>
                                <div>${data.customerName + (data.companyName ? " - " + data.companyName : "")}</div>
                                <div>${this.startDateAlt} - ${this.endDateAlt}</div>
                                </div>
                                `
                                break;
                            }
                            else {
                                continue;
                            }

                        default:
                            if (dollarSignProps.some(x => x == entries[i][0])) {
                                td.innerHTML = entries[i][1] != null ? "$" + entries[i][1].toString() : "";
                            }
                            else {
                                td.innerHTML = entries[i][1] != null ? entries[i][1].toString() : "";
                            }
                            break;
                    }

                    tr.appendChild(td);
                }
            }
            tbl.appendChild(tr);
            tr = document.createElement('tr');
        }
        // let sumTr = document.createElement('tr');


        // for (var i = 0; i < this.selectedHeaders.length; i++) {

        //     if (this.selectedHeaders[i] == 'debitAmount' || this.selectedHeaders[i] == 'creditAmount') {

        //         let td = document.createElement('td');
        //         td.innerHTML = '$' + this.calculateTotalSum(this.selectedHeaders[i].replace('Amount', ''));

        //         sumTr.appendChild(td);
        //     }

        //     else if (this.selectedHeaders[i + 1] && this.selectedHeaders[i + 1] == 'creditAmount' || this.selectedHeaders[i + 1] == 'debitAmount') {
        //         let td = document.createElement('td');
        //         td.innerHTML = 'Total';

        //         sumTr.appendChild(td);
        //     }


        //     else {
        //         let td = document.createElement('td');
        //         td.innerHTML = '';

        //         sumTr.appendChild(td);
        //     }

        // }

        // if (sumTr.children.length > 0) {
        //     tbl.appendChild(sumTr);
        // }


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

           .tblHeaderLedger{
            background-color: #f3f4f6;
           }
        
        .tblHeaderLedger div{
            display: flex;
            flex-direction: column;
            align-items: center;
            font-weight: 600;
            font-size: 1rem;
           }
           
           @media print{
             @page {
                padding: 0;
                margin: 0;
                size: landscape;
             }
           }
        </style>
        `
        let cont = document.createElement('div');
        cont.appendChild(tbl);
        let WindowPrt: any = window.open('', '_blank');
        WindowPrt.document.write(styles);
        WindowPrt.document.write(cont.innerHTML);
        WindowPrt.document.title = 'General Ledger';
        setTimeout(() => {
            WindowPrt.document.close();
            WindowPrt.focus();
            WindowPrt.print();
            WindowPrt.close();
        }, 900);

    }

    onDateChange(type: string): void {
        switch (type) {
            case "start":
                this.startDateAlt = moment(this.startDate).format('MM/DD/YYYY');
                break;
            case "end":
                this.endDateAlt = moment(this.startDate).format('MM/DD/YYYY');
                break;
        }
    }

    onFilterChange(): void {
        let byCompany = this.selectedCompany && this.selectedCompany != "all";
        let byCustId = this.selectedCustomerId && this.selectedCustomerId > 0;
        // this.LedgerName = this.selectedCompany && this.selectedCompany != "all" ? this.selectedCompany : "Ledger";

        this.ledgerDS = new MatTableDataSource(this.ledgerArr.filter(x => (byCustId ? x.customerId == this.selectedCustomerId : true) && (byCompany ? x.companyName == this.selectedCompany : true)));

        // if (byCompany || byCustId) {
        //     var obj = new GeneralLedgerOutputDto();
        //     obj.debitAmount = this.ledgerDS.data.map(x => x.debitAmount || 0).reduce((accumulator, currentValue) => accumulator + currentValue, 0),
        //         obj.creditAmount = this.ledgerDS.data.map(x => x.creditAmount || 0).reduce((accumulator, currentValue) => accumulator + currentValue, 0)
        //     obj.chartOfAccountData = [];
        //     obj.type = 'Sum';
        //     obj.description = 'Total';
        //     var coaData = new GeneralLedgerChartOfAccountData();
        //     obj.chartOfAccountData.push(coaData);
        //     this.ledgerDS.data.push(obj);
        // }

        this.ledgerDS.paginator = this.ledgerPaginator;
        this.spinner.hide();


        this.ledgerDS.paginator = this.ledgerPaginator;
        this.spinner.hide();
    }


    calculateTotalSum(type): string {
        this.showTotal = true;
        switch (type) {
            case "credit":
                return this.ledgerDS.data.filter(x => x.creditAmount &&
                    (this.selectedCustomerId ? x.customerId == this.selectedCustomerId : true) &&
                    (this.selectedCompany != "all" ? x.companyName == this.selectedCompany : true)
                ).map(x => x.creditAmount).reduce((accumulator, currentValue) => accumulator + currentValue, 0).toFixed(2);
            case "debit":
                return this.ledgerDS.data.filter(x => x.debitAmount &&
                    (this.selectedCustomerId ? x.customerId == this.selectedCustomerId : true) &&
                    (this.selectedCompany != "all" ? x.companyName == this.selectedCompany : true)
                ).map(x => x.debitAmount).reduce((accumulator, currentValue) => accumulator + currentValue, 0).toFixed(2);

        }
    }

    downloadCsv(): void {
        let dataArr = [];

        for (var data of this.ledgerDS.data) {
            let obj = null;

            switch (data.type) {
                case 'Sum':
                    obj = {
                        Date: '',
                        Description: '',
                        VoucherID: 'Total',
                        'Debit Amount': '$' + data.creditAmount,
                        'Credit Amount': '$' + data.debitAmount
                    }
                    break;
                case 'Header':
                    obj = {
                        Date: data.customerName + (data.companyName ? ' - ' + data.companyName : ''),
                        Description: '',
                        VoucherID: '',
                        'Debit Amount':'',
                        'Credit Amount': ''
                    }
                    break;

                default:
                    obj = {
                        Date: data.dateAlt,
                        Description: data.description,
                        VoucherID: data.voucherId,
                        'Debit Amount': '$' + data.debitAmount,
                        'Credit Amount': '$' + data.creditAmount,
                        Balance: '$' + data.balance
                    }
                    break;
            }

            dataArr.push(obj);
        }

        const csv = Papa.unparse(dataArr, {
            header: true,
        });

        const blob = new Blob([csv], { type: 'text/csv;charset=utf-8' });
        saveAs(blob, 'General Ledger.csv');
    }



    sortLedgerData(sort: Sort): void {
        const data = this.ledgerArr.slice();
        if (!sort.active || sort.direction === 'asc') {
            this.ledgerDS = data.sort((a, b) => a.id - b.id);
            return;
        }
        else if (!sort.active || sort.direction === "desc") {

            this.ledgerDS = data.sort((a, b) => b.id - a.id);
            return
        }
    }

    onCustomerChange(): void{
        this.companyArrFiltered = this.selectedCustomerId ? this.companyArr.filter(x => x == (this.customerArr.find(y => y.id == this.selectedCustomerId)).bussinessName) : this.companyArr;
        this.selectedCompany = this.companyArrFiltered.length > 0 ? this.companyArrFiltered[0] : 'all';
    }

    saveHeaders(): void {
        this.ledgerColumn = this.selectedHeaders;

        this.showTotal = this.selectedHeaders.indexOf('credit') == -1 && this.ledgerColumn.indexOf('debit') == -1;

        this.generalLedgerService.saveHeaders(this.ledgerColumn.join(','), JSON.parse(localStorage.getItem('user')).id, "CustomerLedgers").subscribe((res) => {
        });
    }

    checkEntry(entryName: string): boolean {
        if (this.selectedHeaders.length == 0) {
            return false;
        }
        if (entryName == this.selectedHeaders[0]) {
            return true;
        }
    }

    async getHeaders(): Promise<void> {
        await this.generalLedgerService.getHeaders(JSON.parse(localStorage.getItem('user')).id, 'CustomerLedgers').subscribe((res) => {
            if (res && res.headers && res.headers.length > 0) {
                this.selectedHeaders = res.headers.split(',');
            }
        });
    }


}