import { Component, EventEmitter, Injector, Input, OnInit, Output } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AppComponentBase } from '@shared/app-component-base';
import { CustomerServiceProxy } from '@shared/service-proxies/service-proxies';
import { NotifyService } from 'abp-ng2-module';

@Component({
  selector: 'app-customer-crm-view',
  templateUrl: './customer-crm-view.component.html',
  styleUrls: ['./customer-crm-view.component.css']
})
export class CustomerCRMViewComponent extends AppComponentBase{
  @Output() onSave = new EventEmitter<any>();
  @Output() activeTab = new EventEmitter<any>();
  @Input() customerId: number;
  @Input() comment: string;
  @Input() totalBalance: number = 0;
  CRMComment: string = null;
  notify: NotifyService;

  constructor( injector: Injector, protected _router: Router, private _activatedRoute: ActivatedRoute, private _customerService: CustomerServiceProxy){
      super(injector);
  }


  backToListView(): void {
      this._router.navigate(['/app/customers']);
  }

  addCustomerCRMComment(): void{
          this.activeTab.emit('Attachments');
          this.onSave.emit(this.comment ? this.comment : "");

  }

  public initialization(): void{
    this._activatedRoute.params.subscribe(parms => {
      if (parms.id) {
        this.customerId = parms.id;
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

}

