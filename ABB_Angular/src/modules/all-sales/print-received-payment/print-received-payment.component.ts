import { Component, ElementRef, Injector, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AppComponentBase } from '@shared/app-component-base';
import { InvoiceServiceProxy } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs';

//import { NgxPrintElementService } from 'ngx-print-element';

@Component({
    selector: 'app-print-received-payment',
    templateUrl: 'print-received-payment.component.html',
    styleUrls: ['print-received-payment.component.scss']
})

export class PrintReceivedPaymentComponent extends AppComponentBase implements OnInit {
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
    customerPhone: string = "+011 000000000";
    customerEmail: string = "";
    customerName: string = "";

    orignalInvoiceNo: string = "";
    paymentDate: string = "";
    refrenceNo: string = "";
    total: string = "";
    openBalance: string = "";

    dataList: any;

    public config = {
        printMode: 'template-popup', // template
        popupProperties: 'toolbar=yes,scrollbars=yes,resizable=yes,top=0,left=0,fullscreen=yes',
        pageTitle: 'Hello World',
        templateString: '<header>I\'m part of the template header</header>{{printBody}}<footer>I\'m part of the template footer</footer>',
        stylesheets: [{ rel: 'stylesheet', href: 'https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0/css/bootstrap.min.css' }],
        styles: ['td { border: 1px solid black; color: green; }', 'table { border: 1px solid black; color: red }', 'header, table, footer { margin: auto; text-align: center; }']
    }
    printData: any;

    ngOnInit(): void {
        this._activatedRoute.queryParams.subscribe(parms => {
            if (parms) {
                this.getInvoiceDetails(parms.id);
            }
        });
    }

    getInvoiceDetails(invoiceId: number) {
        this._invoiceServiceProxy.getReceivedPaymentDetails(invoiceId).pipe(
            finalize(() => { }))
            .subscribe((result) => {
                this.dataList = result;
                this.printData = result[0];
                if (this.printData) {
                    this.orignalInvoiceNo = this.printData.rP_Invoice;
                    this.refrenceNo = this.printData.refrenceNo;
                    this.paymentDate = this.printData.paymentDate;
                    this.note = this.printData.note;
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
                        return sum + item.origionalAmount;
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

    // getInvoiceDetails(invoiceId: number) {
    //     this._invoiceServiceProxy.getInvoiceDetails(invoiceId).pipe(
    //       finalize(() => { }))
    //       .subscribe((result) => {
    //         this.printData = result[0];
    //             this.orignalInvoiceNo = this.printData.rP_Invoice;
    //             this.refrenceNo = this.printData.refrenceNo;
    //             console.log("printData:", result);
    //             console.log("printData:", this.printData);
    //       });
    //   }

}

