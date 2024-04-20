import { Component, EventEmitter, Injector, Input, Output } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AppComponentBase } from '@shared/app-component-base';
import { CustomerServiceProxy } from '@shared/service-proxies/service-proxies';
import { NotifyService } from 'abp-ng2-module';


@Component({
    selector: 'app-customer-license',
    templateUrl: './customer-license.component.html',
    styleUrls: ['./customer-license.component.scss']
})
export class CustomerLicenseComponent extends AppComponentBase {
    @Input() customerId:number;
    @Input() comment:string;
    @Output() onSave = new EventEmitter<any>();
    @Output() activeTab = new EventEmitter<any>();
    notify: NotifyService;

    constructor( injector: Injector, private _router: Router, private _activatedRoute: ActivatedRoute, private _customerService: CustomerServiceProxy){
        super(injector);
    }

    backToListView(): void {
        this._router.navigate(['/app/customers']);
    }


    addCustomerLicenseComment(): void{
            this.activeTab.emit('Contact info');
            this.onSave.emit(this.comment);

    }

}
