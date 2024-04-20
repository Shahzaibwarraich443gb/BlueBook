import { Component, EventEmitter, Injector, Input, OnInit, Output } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AppComponentBase } from '@shared/app-component-base';
import { AddressDto, ContactInfoDto, CreateOrEditCustomerDto, CustomerAddressDto, CustomerServiceProxy } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-customer-address-view',
  templateUrl: './customer-address-view.component.html',
  styleUrls: ['./customer-address-view.component.css']
})
export class CustomerAddressViewComponent extends AppComponentBase{
  @Output() onSave = new EventEmitter<any>();
  @Output() activeTab = new EventEmitter<any>();

  @Input() customerId: number;
  @Input() comment: string;
  @Input() totalBalance: number = 0;

  customeraddress = new CustomerAddressDto();
  customerAddresses: CustomerAddressDto[] = [];
  customer = new CreateOrEditCustomerDto();

  constructor(
    injector: Injector,
    private _activatedRoute: ActivatedRoute,
    protected _router: Router,
    public _customerServiceProxy: CustomerServiceProxy,
  ) {
    super(injector)
  }

  public initialization(): void {
    this.customer = new CreateOrEditCustomerDto();
    this.customer.address = [];
    this._activatedRoute.params.subscribe(parms => {
      if (parms.id) {
        this.customerId = parms.id;
        this._customerServiceProxy.getCustomerAddress(parms.id).pipe(
          finalize(() => {
          })
        )
          .subscribe((result) => {
            if (result != null) {
              this.customer.address = result;
              this.customerAddresses = result;
            }
          });
          if(!this.comment){
            this._customerServiceProxy.getCustomerComment(this.customerId).subscribe({
                next:(res)=>{
                    this.comment = res;
                }
            })
        }
      }
    });
  }

  save() {
    this.addMore();
    if (this.customerId > 0) {
      this.customer.id = this.customerId;
    }
    this.customer.address = this.customerAddresses;
    this._customerServiceProxy.updateCustomerAddress(this.customer)
      .pipe(
        finalize(() => {
        })
      )
      .subscribe((result) => {
        this.customer = result;
        // Emit Next Tab name
        this.activeTab.emit('To-do List');
        this.notify.info('Customer address saved succesfully');
        this.onSave.emit(this.comment ? this.comment : "");
      });
  }

  addMore() {
    if (this.customeraddress.type != null && this.customeraddress.address != null) {
      var index = this.customerAddresses.findIndex(x => x == this.customeraddress);
      if (index !== -1) this.customerAddresses[index] = this.customeraddress;
      else this.customerAddresses.push(this.customeraddress);
      this.customeraddress = new CustomerAddressDto();
      this.customeraddress.type = 'Manager';
    }
  }

  update(customeraddress: CustomerAddressDto) {
    var index = this.customerAddresses.findIndex(x => x == customeraddress);
    if (index !== -1) this.customeraddress = this.customerAddresses[index];
  }

  delete(customeraddress: CustomerAddressDto) {
    var index = this.customerAddresses.findIndex(x => x == customeraddress);
    if (index !== -1) this.customerAddresses.splice(index, 1);
  }

}
