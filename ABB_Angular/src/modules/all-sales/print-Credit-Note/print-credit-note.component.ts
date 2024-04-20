import { Component, ElementRef, Injector, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AppComponentBase } from '@shared/app-component-base';
import { InvoiceServiceProxy } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs';

//import { NgxPrintElementService } from 'ngx-print-element';

@Component({
    selector: 'app-credit-note-payment',
    templateUrl: 'print-credit-note.component.html',
    styleUrls: ['print-credit-note.component.scss']
})

export class PrintCreditNoteComponent extends AppComponentBase implements OnInit {
    constructor(
        private _activatedRoute: ActivatedRoute,
        private injector: Injector,
        private _invoiceServiceProxy: InvoiceServiceProxy,
    ) {
        super(injector);
    }

    @ViewChild('printArea', { static: true }) printArea: ElementRef;
    note: string = "";
    dataList:any;

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
               // this.getInvoiceDetails(parms.id);
                this.printData = parms;
                setTimeout(() => {
                    const printContent = this.printArea.nativeElement.innerHTML;
                    const originalContent = document.body.innerHTML;

                    document.body.innerHTML = printContent;
                    window.print();
                    document.body.innerHTML = originalContent;
                    // alert("Please close this tab after printing.");
                     window.close();
                }, 100);
            }
        });
    }

    getInvoiceDetails(invoiceId: number) {
        this._invoiceServiceProxy.getReceivedPaymentDetails(invoiceId).pipe(
            finalize(() => { }))
            .subscribe((result) => {

                this.dataList = result;
                console.log("dataList:",this.dataList);
            });
    }

}

