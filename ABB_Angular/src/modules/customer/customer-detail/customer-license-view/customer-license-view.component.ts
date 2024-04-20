import { Component, EventEmitter, Injector, Input, Output } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AppComponentBase } from '@shared/app-component-base';
import { CustomerServiceProxy } from '@shared/service-proxies/service-proxies';
import { NotifyService } from 'abp-ng2-module';


@Component({
    selector: 'app-customer-license-view',
    templateUrl: './customer-license-view.component.html',
    styleUrls: ['./customer-license-view.component.scss']
})
export class CustomerLicenseViewComponent extends AppComponentBase {
    @Output() onSave = new EventEmitter<any>();
    @Output() activeTab = new EventEmitter<any>();
    @Input() comment: string;
    @Input() totalBalance: number = 0;

    licenseComment: string = null;
    customerId: number = 0;
    notify: NotifyService;

    constructor(protected _router: Router, private _activatedRoute: ActivatedRoute, private _customerService: CustomerServiceProxy, injector: Injector) {
        super(injector)
    }

    backToListView(): void {
        this._router.navigate(['/app/customers']);
    }

    public initialization() {
        this._activatedRoute.params.subscribe(parms => {
            if (parms.id) {
                this.customerId = parms.id;
                this._customerService.getCustomerLicenseData(parms.id).subscribe((res) => {
                    this.licenseComment = res.licenseComment;
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

    addCustomerLicenseComment(): void {
            this.activeTab.emit('Contact info');
            this.onSave.emit(this.comment);
    }

}
