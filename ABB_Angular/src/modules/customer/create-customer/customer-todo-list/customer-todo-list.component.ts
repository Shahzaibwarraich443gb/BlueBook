import { Component, EventEmitter, Injector, Input, OnInit, Output } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AppComponentBase } from '@shared/app-component-base';
import { CustomerServiceProxy } from '@shared/service-proxies/service-proxies';
import { NotifyService } from 'abp-ng2-module';

@Component({
  selector: 'app-customer-todo-list',
  templateUrl: './customer-todo-list.component.html',
  styleUrls: ['./customer-todo-list.component.css']
})
export class CustomerTodoListComponent extends AppComponentBase{
  @Output() onSave = new EventEmitter<any>();
  @Output() activeTab = new EventEmitter<any>();
  @Input() customerId: number;
  @Input() comment: string;
  todoListComment: string = null;
  notify: NotifyService;

  constructor( injector: Injector, private _router: Router, private _activatedRoute: ActivatedRoute, private _customerService: CustomerServiceProxy){
      super(injector);
  }


  backToListView(): void {
      this._router.navigate(['/app/customers']);
  }

  addCustomerTodoListComment(): void{
          this.activeTab.emit('CRM');
          this.onSave.emit(this.comment ? this.comment : null);
  }

}
