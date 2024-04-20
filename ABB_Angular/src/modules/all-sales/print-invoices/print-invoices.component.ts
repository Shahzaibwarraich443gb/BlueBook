import { Component, ElementRef, Injector, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AppComponentBase } from '@shared/app-component-base';
import { InvoiceServiceProxy } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs';

@Component({
    selector: 'app-print-invoices',
    templateUrl: 'print-invoices.component.html',
    styleUrls: ['print-invoices.component.scss']
})

export class PrintInvoicesComponent extends AppComponentBase implements OnInit {
    totalSaleTax: any;
    constructor(
        private _activatedRoute: ActivatedRoute,
        private injector: Injector,
        private _invoiceServiceProxy: InvoiceServiceProxy,
    ) {
        super(injector);
    }

    @ViewChild('printArea', { static: true }) printArea: ElementRef;
    note: string = "";
    companyName: string = "";
    comAddress: string = "";
    comCity: string = "";
    comState: string = "";
    comPostCode: string = "";
    comCountry: string = "";
    comPhone: string = "";
    comEmail: string = "";

    customerAddress: string = "";
    customerCity: string = "";
    customerState: string = "";
    customerPostCode: string = "";
    customerCountry: string = "";
    customerPhone: string = "";
    customerEmail: string = "";
    customerName: string = "";

    orignalInvoiceNo: string = "";
    paymentDate: string = "";
    refrenceNo: string = "";
    total: string = "";
    csr: string = "";
    openBalance: string = "";

    dataList: any;
    printData: any;

    ngOnInit(): void {
        this._activatedRoute.queryParams.subscribe(parms => {
            if (parms) {
               // this.getInvoiceDetails(parms.id);
                this.getNewPrintList(parms.id);
            } 
            // else if (parms.id) {
            //     this.getInvoiceDetails(parms.id);
            // }
        });
    }

    getInvoiceDetails(invoiceId: number) {
        this._invoiceServiceProxy.getInvoiceDetails(invoiceId).pipe(
            finalize(() => { }))
            .subscribe((result) => {
                this.dataList = result;
                this.printData = result[0];
                if (this.printData) {
                    this.orignalInvoiceNo = this.printData.invoiceNo;
                    this.refrenceNo = this.printData.refrenceNo;
                    this.paymentDate = this.printData.invoiceDueDate;
                    this.note = this.printData.note;
                    this.csr = this.printData.csr;
                    this.openBalance = this.printData.balance;
                    this.companyName = this.printData.companyName;
                    this.comAddress = this.printData.comAddress;
                    this.comCity = this.printData.comCity;
                    this.comState = this.printData.comState;
                    this.comPostCode = this.printData.comPostCode;
                    this.comCountry = this.printData.comCountry;
                    this.comEmail = this.printData.comEmail;
                    this.comPhone = this.printData.comPhone;

                    this.customerEmail = this.printData.customerEmail;
                    this.customerName = this.printData.customerName;
                    this.customerPhone = this.printData.customerPhone;
                    this.customerCity = this.printData.customerCity;
                    this.customerState = this.printData.customerState;
                    this.customerPostCode = this.printData.customerPostCode;
                    this.customerCountry = this.printData.customerCountry;

                    this.total = this.dataList.reduce((sum, item) => {
                        return sum + item.amount;
                    }, 0);
                    this.totalSaleTax = this.dataList.reduce((sum, item) => {
                        return sum + item.saleTax;
                    }, 0);

                    setTimeout(() => {
                        const printContent = this.printArea.nativeElement.innerHTML;
                        const originalContent = document.body.innerHTML;

                        document.body.innerHTML = printContent;
                        window.print();
                        document.body.innerHTML = originalContent;
                        // alert("Please close this tab after printing.");
                        window.close();
                    }, 500);
                } else {
                    window.close();
                }
            });
    }

    getNewPrintList(invoiceId: number){
        this._invoiceServiceProxy.getPrintDetails(invoiceId).pipe(finalize(() => { }))
            .subscribe((result) => {
                console.log("print:",result);
                this.printData = result;
                this.dataList = result.printDetails;
                if (this.printData) {
                    this.orignalInvoiceNo = this.printData.orignalInvoiceNo;
                    this.refrenceNo = this.printData.refrenceNo;
                    this.paymentDate = this.printData.invoiceDueDate;
                    this.note = this.printData.note;
                    this.csr = this.printData.csr;
                    this.openBalance = this.printData.balance;
                    this.companyName = this.printData.companyName;
                    this.comAddress = this.printData.comAddress;
                    this.comCity = this.printData.comCity;
                    this.comState = this.printData.comState;
                    this.comPostCode = this.printData.comPostCode;
                    this.comCountry = this.printData.comCountry;
                    this.comEmail = this.printData.comEmail;
                    this.comPhone = this.printData.comPhone;

                    this.customerEmail = this.printData.customerEmail;
                    this.customerName = this.printData.customerName;
                    this.customerPhone = this.printData.customerPhone;
                    this.customerCity = this.printData.customerCity;
                    this.customerState = this.printData.customerState;
                    this.customerPostCode = this.printData.customerPostCode;
                    this.customerCountry = this.printData.customerCountry;

                    this.total = this.dataList.reduce((sum, item) => {
                        return sum + item.amount;
                    }, 0);
                    this.totalSaleTax = this.dataList.reduce((sum, item) => {
                        return sum + item.saleTax;
                    }, 0);

                    setTimeout(() => {
                        const printContent = this.printArea.nativeElement.innerHTML;
                        const originalContent = document.body.innerHTML;

                        document.body.innerHTML = printContent;
                        window.print();
                        document.body.innerHTML = originalContent;
                        // alert("Please close this tab after printing.");
                        window.close();
                    }, 500);
                }
        });
    }
}

