import { Component, EventEmitter, Injector, Input, OnInit, Output } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { AppComponentBase } from '@shared/app-component-base';
import { CreateOrEditCustomerDto, LanguageDto, EthnicityDto, SourceReferralTypeDto, SalesPersonTypeDto, LanguageServiceProxy, CustomerServiceProxy, EthnicitiesServiceProxy, SourceReferralTypeServiceProxy, SalesPersonTypeServiceProxy, DetailDto, JobTitleServiceProxy, JobTitle, JobTitleDto } from '@shared/service-proxies/service-proxies';
import { CreateEthnicityComponent } from 'modules/ethnicity/create-ethnicity/create-ethnicity.component';
import { CreateJoTitleComponent } from 'modules/job-title/create-job-title/create-job-title.component';
import { CreatelanguageComponent } from 'modules/Language/create-language/create-language.component';
import { CreateSalePersonComponent } from 'modules/sale-person/create-sale-person/create-sale-person.component';
import { CreateSourecReferalComponent } from 'modules/source-referal/create-source-referal/create-source-referal.component';
import { finalize } from 'rxjs';

@Component({
    selector: 'app-customer-dtl',
    templateUrl: './customer-dtl-view.component.html',
    styleUrls: ['./customer-dtl-view.component.scss']
})
export class CustomerDtlViewComponent extends AppComponentBase implements OnInit {
    @Output() onSave = new EventEmitter<any>();
    @Output() activeTab = new EventEmitter<any>();

    @Input() customerId: number;
    @Input() comment: string;
    @Input() totalBalance: number = 0;

    languages: LanguageDto[] = [];
    ethnicities: EthnicityDto[] = [];
    salesPersonTypes: SalesPersonTypeDto[] = [];
    jobTitleArr: JobTitleDto[] = [];
    showPass: boolean = false;

    customer = new CreateOrEditCustomerDto();


    constructor(
        injector: Injector,
        public _dialog: MatDialog,
        private _activatedRoute: ActivatedRoute,
        public _languageServiceProxy: LanguageServiceProxy,
        public _customerServiceProxy: CustomerServiceProxy,
        public _ethnicitiesServiceProxy: EthnicitiesServiceProxy,
        public _sourceReferralTypeServiceProxy: SourceReferralTypeServiceProxy,
        public _salesPersonTypeServiceProxy: SalesPersonTypeServiceProxy,
        private _jobTitleService: JobTitleServiceProxy,
        private _customerService: CustomerServiceProxy,
        protected _router: Router
    ) {
        super(injector)
    }

    ngOnInit() {
        this.intialization();
        this.getRelevantData();
    }

   public getCustomerData(): void {
        this._activatedRoute.params.subscribe(parms => {
            if (parms.id) {
                this.customerId = parms.id;
                this._customerServiceProxy.getCustomerDetails(this.customerId).pipe(
                    finalize(() => {
                    })
                )
                    .subscribe((result) => {
                        if (result != undefined) {
                            this.customer = new CreateOrEditCustomerDto();
                            this.customer = result;
                            console.log(this.customer);
                        }
                        if (this.customer.detail.salesPersonType == null) {
                            this.customer.detail.salesPersonType = new SalesPersonTypeDto();
                        }
                        if (this.customer.detail.language == null) {
                            this.customer.detail.language = new LanguageDto();
                        }
                        if (this.customer.detail.ethnicity == null) {
                            this.customer.detail.ethnicity = new EthnicityDto();

                        }
                    });

                if(!this.comment){
                    this._customerService.getCustomerComment(this.customerId).subscribe({
                        next:(res)=>{
                            this.comment = res;
                        }
                    })
                }
            }
        });
    }

    private intialization() {
        this.customer = new CreateOrEditCustomerDto();
        this.customer.detail = new DetailDto();
        this.customer.detail.language = new LanguageDto();
        this.customer.detail.ethnicity = new EthnicityDto();
        this.customer.detail.salesPersonType = new SalesPersonTypeDto();
    }

    getRelevantData() {
        this.getLanguages();
        this.getEthnicities();
        this.getSalesPersonTypes();
        this.getJobTitle();
    }

    save() {
        ;
        if (this.customerId > 0) {
            this.customer.id = this.customerId;
        }
        this._customerServiceProxy.updateCustomerDetail(this.customer).pipe(
            finalize(() => {
            })
        )
            .subscribe((result) => {
                this.customer = result;
                // Emit Next Tab name
                this.activeTab.emit('Password');
                this.notify.info('Customer detail saved succesfully');
                this.onSave.emit(this.comment);
            });
    }

    public openCreateSalesPersonTypeDialog(id?: number): void {
        const dialogRef = this._dialog.open(CreateSalePersonComponent, {
            data: { id: id },
        });

        dialogRef.afterClosed().subscribe((result) => {
            dialogRef.close();
            this.getSalesPersonTypes();
        });
    }

    public openJobTitleDialog(id?: number): void {
        const dialogRef = this._dialog.open(CreateJoTitleComponent, {
            data: { id: id },
        });

        dialogRef.afterClosed().subscribe((result) => {
            dialogRef.close();
            this.getJobTitle();
        });
    }

    protected getSalesPersonTypes() {
        this._salesPersonTypeServiceProxy
            .getAll()
            .subscribe((arg) => (this.salesPersonTypes = arg));
    }

    getJobTitle(): void {
        this._jobTitleService.getAll().subscribe((res) => {
            this.jobTitleArr = res;
        });
    }

    protected getEthnicities() {
        this._ethnicitiesServiceProxy
            .getAll()
            .subscribe((arg) => (this.ethnicities = arg));
    }

    public openCreateEthnicityDialog(id?: number): void {
        const dialogRef = this._dialog.open(CreateEthnicityComponent, {
            data: { id: id },
        });

        dialogRef.afterClosed().subscribe((result) => {
            dialogRef.close();
            this.getEthnicities();
        });
    }

    protected getLanguages() {
        this._languageServiceProxy
            .getAll()
            .subscribe((arg) => (this.languages = arg));
    }

    public openCreateLanguageDialog(id?: number): void {
        const dialogRef = this._dialog.open(CreatelanguageComponent, {
            data: { id: id },
        });

        dialogRef.afterClosed().subscribe((result) => {
            dialogRef.close();
            this.getLanguages();
        });
    }

    hideDialog() {
        this.onSave.emit();
    }
}

