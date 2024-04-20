import { Component, Injector, Input, OnInit, TemplateRef, ViewChild } from '@angular/core'
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { Sort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { ActivatedRoute, Router } from '@angular/router';
import { MatSort } from '@angular/material/sort';
import { CustomerServiceProxy, CustomerTaxSelection, EntityDto, InvoiceServiceProxy } from '@shared/service-proxies/service-proxies';
import * as moment from 'moment';
import { AppComponentBase } from '@shared/app-component-base';
import { finalize } from 'rxjs';
import { ViewHistoryComponent } from 'modules/all-sales/view-history/view-history.component';


@Component({
    selector: 'app-customer-detail-view',
    templateUrl: './customer-detail-view.component.html',
    styleUrls: ['./customer-detail-view.component.scss']
})
export class CustomerDetailViewComponent extends AppComponentBase implements OnInit {
    @ViewChild(MatPaginator) paginator: MatPaginator;
    @ViewChild(MatSort) sort: MatSort;
    @ViewChild('serviceModal') serviceModal: TemplateRef<any>;
    @ViewChild('CustomerDtlViewComponent') customerDtlComp;
    @ViewChild('CustomerInfoViewComponent') CustomerInfoViewComponent;
    @ViewChild('customerLicenseViewComponent') customerLicenseViewComponent;
    @ViewChild('customerContactInfoViewComponent') customerContactInfoViewComponent;
    @ViewChild('customerPassViewComponent') customerPassViewComponent;
    @ViewChild('customerAddressViewComponent') customerAddressViewComponent;
    @ViewChild('customerTodoListViewComponent') customerTodoListViewComponent;
    @ViewChild('customerCRMViewComponent') customerCRMViewComponent;
    @ViewChild('customerAttachmentViewComponent') customerAttachmentViewComponent;
    @ViewChild('customerDiaryViewComponent') customerDiaryViewComponent;

    @Input() activeTab: string = 'Info';

    comment: string = "";
    customerId: number;
    customerTransactions = [];
    showServiceType: boolean = false;
    serviceTypeKey: number = 0;
    serviceTypeVal: number = 0;
    includedServiceArr: number[] = [];
    totalBalance: number = 0.00;
    displayedColumns: string[] = ['Date', 'Type', 'Invoice', 'Product', 'Description', 'CSR', 'Balance', 'Total', 'Status', 'Action'];
    serviceTypes: string[] = ['Corporate Tax', 'Personal Tax', 'Sales Tax'];
    dataSource: any = null;

    constructor(private injector: Injector,
        private _customerService: CustomerServiceProxy,
        public _dialog: MatDialog,
        private _activatedRoute: ActivatedRoute,
        private _invoiceService: InvoiceServiceProxy,
        private _router: Router,

        private dialog: MatDialog) { super(injector); }

    ngOnInit(): void {
        this.getCustomerTransactions();
    }

    ngAfterViewInit(): void{
        var el = document.getElementsByClassName('mat-tab-labels');
        var elActive = document.getElementsByClassName('mat-tab-label-active');
    

        for(var i=0; i<el.length; i++){
            el[i].classList.add('customerMatTab');
        }

        for(var i=0; i<elActive.length; i++){
            elActive[i].classList.add('mat-tab-label-active');
        }
    }

    applyFilter(event: Event) {
        const filterValue = (event.target as HTMLInputElement).value;
        this.dataSource.filter = filterValue.trim().toLowerCase();
    }

    moveNextTab(comment) {
        this.comment = comment;
        for (let i = 0; i < document.querySelectorAll('.mat-tab-label-content').length; i++) {
            if ((<HTMLElement>document.querySelectorAll('.mat-tab-label-content')[i]).innerText == this.activeTab) {
                (<HTMLElement>document.querySelectorAll('.mat-tab-label')[i]).click();
            }
        }
    }

    tabChange(event): void {
        if (event.index == 0) {
            this.CustomerInfoViewComponent.ngOnInit();
        }
        if (event.index == 1) {
            setTimeout(() => {
                this.customerDtlComp.getCustomerData();
            }, 1000);
        }
        else if (event.index == 2) {
            setTimeout(() => {
                this.customerPassViewComponent.initialization();
            }, 1000);
        }
        else if (event.index == 3) {
            setTimeout(() => {
                this.customerLicenseViewComponent.initialization();
            }, 1000);
        }
        else if (event.index == 4) {
            setTimeout(() => {
                this.customerContactInfoViewComponent.getCustomerContactData();
            }, 1000);
        }
        else if (event.index == 5) {
            setTimeout(() => {
                this.customerAddressViewComponent.initialization();
            }, 1000);
        }
        else if (event.index == 6) {
            setTimeout(() => {
                this.customerTodoListViewComponent.initialization();
            }, 1000);
        }
        else if (event.index == 7) {
            setTimeout(() => {
                this.customerCRMViewComponent.initialization();
            }, 1000);
        }
        else if (event.index == 8) {
            setTimeout(() => {
                this.customerAttachmentViewComponent.getCustomerAttachments();
            }, 1000);
        }
        else if (event.index == 9) {
            setTimeout(() => {
                this.customerDiaryViewComponent.getCustomerId();
            }, 1000);
        }
    }

    invokeService(): void {
        this.showServiceType = true;
        this.dialog.closeAll();
        if (this.includedServiceArr.indexOf(this.serviceTypeVal) == -1) {
            let customerTaxSelection = new CustomerTaxSelection();
            customerTaxSelection.customerId = this.customerId;
            customerTaxSelection.taxService = this.serviceTypeVal;
            this._customerService.saveCustomerTaxSelection(customerTaxSelection).subscribe(res => {

            });
            this.includedServiceArr.push(this.serviceTypeVal);
        }
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

    getCustomerTransactions(): void {
        this._activatedRoute.params.subscribe(parms => {
            if (parms.id) {

                this.customerId = parms.id;
                let entityDto = new EntityDto();
                entityDto.id = this.customerId;
                this._customerService.customerTaxSelectionGet(entityDto).subscribe((res) => {
                    this.includedServiceArr = res.map(x => x.taxService);
                });
                this._invoiceService.getCustomerTransaction(parms.id).subscribe((res) => {
                    this.customerTransactions = res;
                    this.totalBalance = 0.00;
                    for (var data of this.customerTransactions) {
                        data.invoiceDateAlt = moment(data.invoiceDate).format('MM/DD/YYYY');
                        if(data.status.toLowerCase() == "open"){
                            this.totalBalance += parseFloat(data.balance)
                        }
                        else if(data.status.toLowerCase() == "partial"){
                            this.totalBalance += Math.abs(parseFloat(data.total) - parseFloat(data.balance));
                        }

                    }

                    
                    this.totalBalance = parseFloat(this.totalBalance.toFixed(2));
                    this.dataSource = new MatTableDataSource<any>(this.customerTransactions);
                    this.dataSource.paginator = this.paginator;
                });
            }
        });

    }

    setService(event: any, key: number) {
        let element = document.getElementsByClassName('salesTaxContainer')[0];

        for (var i = 0; i < element.children.length; i++) {
            element.children[i].classList.remove('active');
        }

        event.target.classList.add('active');
        this.serviceTypeKey = key;
    }

    openServiceModal(): void {
        this.serviceTypeVal = 0;
        this.dialog.open(this.serviceModal, {
            width: '50%',
            height: 'auto',
            disableClose: false,
        });
    }

    UpdateInvoice(item: any) {
        if (item.type === "Invoice") {
            this._router.navigate(['/app/invoices/' + item.id]);
        }
        else if (item.type === "Credit Note") {
            this._router.navigate(['/app/credit-note/' + item.id]);
        }
        else if (item.type === "Estimate") {
            this._router.navigate(['/app/Estimate/' + item.id]);
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
        } else if (element.orignalInvoiceNo.startsWith('CN-') && element.status === 'Closed') {
            return true;
        } else if (element.orignalInvoiceNo.startsWith('ET-') && element.status === 'Closed') {
            return true;
        } else {
            return false;
        }
    }
    isDeleteEditable(element: any): boolean {
        if (element.orignalInvoiceNo.startsWith('IN-') && element.status === 'Open') {
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
        if (element.orignalInvoiceNo.startsWith('RP-') && element.status === 'Closed') {
            return true;
        } else if (element.orignalInvoiceNo.startsWith('IN-') && element.status === 'Open') {
            return true;
        } else if (element.orignalInvoiceNo.startsWith('CN-') && element.status === 'Closed') {
            return true;
        } else if (element.orignalInvoiceNo.startsWith('ET-') && element.status === 'Closed') {
            return true;
        } else {
            return false;
        }
    }

    invoicePrint(item) {
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
        // const queryParams = item; 
        // const urlTree = this._router.createUrlTree(['/app/print-received-payment/'], { queryParams });
        // const url = this._router.serializeUrl(urlTree);
        // window.open(url, '_blank');
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