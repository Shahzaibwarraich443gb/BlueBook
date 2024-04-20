import { Component, Injector, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort, Sort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { AppComponentBase } from '@shared/app-component-base';
import { ChartOfAccountsServiceProxy, GeneralLedgerChartOfAccountData, GeneralLedgerOutputDto, GeneralLedgerServiceProxy, GetGeneralLedgerInputDto } from '@shared/service-proxies/service-proxies';
import * as moment from 'moment';
import { NgxSpinnerService } from 'ngx-spinner';
import { saveAs } from 'file-saver';
import * as Papa from 'papaparse';

@Component({
    selector: 'app-general-ledger',
    templateUrl: './General-Ledger.component.html',
    styleUrls: ['./General-Ledger.component.scss']
})
export class GeneralLedgerComponent extends AppComponentBase {

    @ViewChild('ledgerPaginator') ledgerPaginator: MatPaginator;
    @ViewChild(MatSort) sort: MatSort;

    ledgerArr: GeneralLedgerOutputDto[] = [];
    ledgerDS: any;
    mainHeadArr: any[] = [];
    coaArr: any[] = [];
    coaArrFiltered: any[] = [];
    showTotal: boolean = true;
    selectedMainHeadId: number = 0;
    selectedSubHeadId: number = 0;
    selectedHeaders: string[] = [];
    startDate: Date = new Date(new Date().setMonth(new Date().getMonth() - 1));
    endDate: Date = new Date();
    startDateAlt: string = moment(this.startDate).format('MM/DD/YYYY');
    endDateAlt: string = moment(this.endDate).format('MM/DD/YYYY');
    LedgerName: string = "Ledger";


    ledgerColumn: string[] = ['dateAlt', 'customerName', 'companyName', 'voucherId', 'description', 'debit', 'credit', 'balance', 'csr'];
    HeadersArr: any[] = [
        { name: 'Date', prop: 'dateAlt' },
        { name: 'Customer Name', prop: 'customerName' },
        { name: 'Company Name', prop: 'companyName' },
        { name: 'Voucher ID', prop: 'voucherId' },
        { name: 'Description', prop: 'description' },
        { name: 'Debit', prop: 'debitAmount' },
        { name: 'Credit', prop: 'creditAmount' },
        { name: 'Balance', prop: 'balance' },
        { name: 'CSR', prop: 'csr' },
    ];

    filteredLedgerColumn: string[] = [];
    constructor(private injector: Injector,
        private generalLedgerService: GeneralLedgerServiceProxy,
        private spinner: NgxSpinnerService,
        private chartOfAccountService: ChartOfAccountsServiceProxy) {
        super(injector);
    }

    async ngOnInit(): Promise<void> {
        this.spinner.show();
        this.selectedHeaders = this.HeadersArr.map(x => x.prop);
        await this.getHeaders();
        await this.getCoaList();
        await this.getAllMainHead();
        await this.getLedgerData();
    }

    async getAllMainHead(): Promise<void> {
        await this.chartOfAccountService.getAllMainHead().subscribe((res) => {
            this.mainHeadArr = res;
        });
    }

    async getCoaList(): Promise<void> {
        await this.chartOfAccountService.getChartOfAccountList().subscribe((res) => {
            this.coaArr = res;
            this.coaArrFiltered = res;
        })
    }

    async getLedgerData(): Promise<void> {
        this.spinner.show();
        let key = new GetGeneralLedgerInputDto();
        key.startDate = moment(this.startDate);
        key.endDate = moment(this.endDate);
        await this.generalLedgerService.getLedgerForTable(key).subscribe((res) => {
            // let obj = new GeneralLedgerOutputDto();
            // obj.type = 'Sum';
            // obj.chartOfAccountData = [];
            // var coaData = new GeneralLedgerChartOfAccountData();
            // obj.chartOfAccountData.push(coaData);
            // res.push(obj);
            this.ledgerArr = res;
            this.ledgerDS = new MatTableDataSource(this.ledgerArr);
            this.ledgerDS.paginator = this.ledgerPaginator;
            if (this.selectedMainHeadId > 0 || this.selectedSubHeadId > 0) {
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

    checkEntry(entryName: string): boolean {
        if (this.selectedHeaders.length == 0) {
            return false;
        }
        if (entryName == this.selectedHeaders[0]) {
            return true;
        }
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


            //sorting ledger data props with tbl header
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
                            if (entries[i + 1] &&
                                entries[i][0] != "creditAmount" &&
                                entries[i][0] != "debitAmount" &&
                                (entries[i + 1][0] == "creditAmount" || entries[i + 1][0] == "debitAmount")) {

                                td.innerHTML = 'Total';
                            }
                            else if (entries[i][0].toLowerCase() == "balance") {
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
                                <div>${data.chartOfAccountData.find(x => x.subHeadId == data.linkedSubHeadId) ? data.chartOfAccountData.find(x => x.subHeadId == data.linkedSubHeadId).subHeadName : ''}</div>
                                <div>${this.startDateAlt} - ${this.endDateAlt}</div>
                                </div>
                                `
                                break;
                            }
                            else{
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
        //         td.innerHTML = '$' + 'see' //this.calculateTotalSum(this.selectedHeaders[i].replace('Amount', ''));

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


    calculateTotalSum(type): string {
        this.showTotal = true;
        switch (type) {
            case "credit":
                return this.ledgerDS.data.filter(x => x.creditAmount &&
                    (this.selectedMainHeadId ? x.chartOfAccountData.some(y => y.mainHeadId == this.selectedMainHeadId) : true) &&
                    (this.selectedSubHeadId ? x.chartOfAccountData.some(y => y.subHeadId == this.selectedSubHeadId) : true)
                ).map(x => x.creditAmount).reduce((accumulator, currentValue) => accumulator + currentValue, 0).toFixed(2);
            case "debit":
                return this.ledgerDS.data.filter(x => x.debitAmount &&
                    (this.selectedMainHeadId ? x.chartOfAccountData.some(y => y.mainHeadId == this.selectedMainHeadId) : true) &&
                    (this.selectedSubHeadId ? x.chartOfAccountData.some(y => y.subHeadId == this.selectedSubHeadId) : true)
                ).map(x => x.debitAmount).reduce((accumulator, currentValue) => accumulator + currentValue, 0).toFixed(2);

        }
    }

    downloadCsv(): void {
        let dataArr = [];

        for (var data of this.ledgerDS.data) {
            let obj = null;
           console.log(data);
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
                        Date: data.chartOfAccountData.find(x => x.subHeadId == data.linkedSubHeadId) ? data.chartOfAccountData.find(x => x.subHeadId == data.linkedSubHeadId).subHeadName : '',
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


    onSubHeadChange(): void {
        let data = null;
        if (this.selectedMainHeadId != null && this.selectedMainHeadId > 0 && this.selectedSubHeadId == 0) {
            data = null;
        }
        else if (this.selectedSubHeadId == 0) {
            data = this.ledgerArr;
        }
        else {
            this.LedgerName = this.coaArr.find(x => x.id == this.selectedSubHeadId).accountDescription;
            if (this.selectedMainHeadId != null && this.selectedMainHeadId > 0) {
                data = this.ledgerArr.filter(x => x.chartOfAccountData.some(y => y.subHeadId == this.selectedSubHeadId && y.mainHeadId == this.selectedMainHeadId));
            }
            else {
                data = this.ledgerArr.filter(x => x.chartOfAccountData.some(y => y.subHeadId == this.selectedSubHeadId));
            }
        }
        this.ledgerDS = new MatTableDataSource(data ? data : []);
        this.ledgerDS.paginator = this.ledgerPaginator;
    }


    onMainHeadChange(): void{
        this.coaArrFiltered = this.selectedMainHeadId ? this.coaArr.filter(x => x.mainHeadId == this.selectedMainHeadId) : this.coaArr;
    }


    onFilterChange(): void {
        let byMainAccount = this.selectedMainHeadId && this.selectedMainHeadId > 0;
        let bySubAccount = this.selectedSubHeadId && this.selectedSubHeadId > 0;


        this.ledgerDS = new MatTableDataSource(this.ledgerArr.filter(x =>
            (byMainAccount ? x.chartOfAccountData.some(y => y.mainHeadId == this.selectedMainHeadId) : true)
            &&
            (bySubAccount ? x.chartOfAccountData.some(y => y.subHeadId == this.selectedSubHeadId) : true)
        ));


        // if (byMainAccount) {
        //     this.LedgerName = this.mainHeadArr.some(x => x.id == this.selectedMainHeadId) ? this.mainHeadArr.find(x => x.id == this.selectedMainHeadId).name : "Ledger";
        // }
        // else if (bySubAccount) {
        //     this.LedgerName = this.coaArr.some(x => x.id == this.selectedSubHeadId) ? this.coaArr.find(x => x.id == this.selectedSubHeadId).accountDescription : "Ledger";
        // }
        // else {
        //     this.LedgerName = "Ledger";
        // }

        // if (byMainAccount || bySubAccount) {
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

    saveHeaders(): void {
        this.ledgerColumn = this.selectedHeaders;

        this.showTotal = this.selectedHeaders.indexOf('credit') == -1 && this.selectedHeaders.indexOf('debit') == -1;

        this.generalLedgerService.saveHeaders(this.ledgerColumn.join(','), JSON.parse(localStorage.getItem('user')).id, "GeneralLedgers").subscribe((res) => {
        });
    }

    async getHeaders(): Promise<void> {
        await this.generalLedgerService.getHeaders(JSON.parse(localStorage.getItem('user')).id, "GeneralLedgers").subscribe((res) => {
            if (res && res.headers && res.headers.length > 0) {
                this.selectedHeaders = res.headers.split(',');
            }
        });
    }


}