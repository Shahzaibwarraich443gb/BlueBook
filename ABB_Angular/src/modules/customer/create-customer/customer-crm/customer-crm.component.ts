import { Component, EventEmitter, Injector, Input, OnInit, Output } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AppComponentBase } from '@shared/app-component-base';
import { CustomerServiceProxy } from '@shared/service-proxies/service-proxies';
import { NotifyService } from 'abp-ng2-module';

@Component({
  selector: 'app-customer-crm',
  templateUrl: './customer-crm.component.html',
  styleUrls: ['./customer-crm.component.css']
})
export class CustomerCRMComponent extends AppComponentBase{
  @Output() onSave = new EventEmitter<any>();
  @Output() activeTab = new EventEmitter<any>();
  @Input() customerId: number;
  @Input() comment: string;
  CRMComment: string = null;
  notify: NotifyService;

  constructor( injector: Injector, private _router: Router, private _activatedRoute: ActivatedRoute, private _customerService: CustomerServiceProxy){
      super(injector);
  }


  backToListView(): void {
      this._router.navigate(['/app/customers']);
  }

  addCustomerCRMComment(): void{
          this.activeTab.emit('Attachments');
          this.onSave.emit(this.comment ? this.comment : "");
  }

}
