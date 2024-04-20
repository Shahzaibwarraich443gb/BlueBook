import { Component, EventEmitter, Injector, Input, OnInit, Output } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { AppComponentBase } from '@shared/app-component-base';
import { CreateOrEditCustomerDto, CreateOrEditPhoneDto, CustomerServiceProxy, PhoneDto, UserNamePasswordDto } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-user-info',
  templateUrl: './user-info.component.html',
  styleUrls: ['./user-info.component.css']
})
export class UserInfoComponent extends AppComponentBase implements OnInit {
  @Output() onSave = new EventEmitter<any>();
  @Output() activeTab = new EventEmitter<any>();

  userNamePasswords: UserNamePasswordDto[] = [];
  @Input() customerId: number;

  @Input() customer = new CreateOrEditCustomerDto();
  userNamePassword = new UserNamePasswordDto();

  constructor(
    injector: Injector,
    public _dialog: MatDialog,
    public _router:Router,
    public _customerServiceProxy: CustomerServiceProxy,
    private _activatedRoute: ActivatedRoute,
  ) {
    super(injector)
  }
  addMore() {
    if (this.userNamePassword.type != null && this.userNamePassword.userName != null && this.userNamePassword.password != null && this.userNamePassword.url != null) {
      var index = this.userNamePasswords.findIndex(x => x == this.userNamePassword);
      if (index !== -1) this.userNamePasswords[index] = this.userNamePassword;
      else this.userNamePasswords.push(this.userNamePassword);
      this.userNamePassword = new UserNamePasswordDto();
      this.userNamePassword.type = 'Personal';
    }
  }

  update(userNamePassword: UserNamePasswordDto) {
    var index = this.userNamePasswords.findIndex(x => x == userNamePassword);
    if (index !== -1) this.userNamePassword = this.userNamePasswords[index];
  }

  delete(userNamePassword: UserNamePasswordDto) {
    var index = this.userNamePasswords.findIndex(x => x == userNamePassword);
    if (index !== -1) this.userNamePasswords.splice(index, 1);
  }

  save() {
    this.addMore();
    if (this.customerId > 0) {
      this.customer.id = this.customerId;
    }
    this.customer.userPassword = this.userNamePasswords;
    this._customerServiceProxy.updateCustomerUser(this.customer)
      .pipe(
        finalize(() => {
        })
      )
      .subscribe((result) => {
        this.customer = result;
        // Emit Next Tab name
        this.activeTab.emit('Address');
        this.userNamePassword.type = 'Personal';
        this.notify.info('Customer contact info saved succesfully');
        this.onSave.emit();
        this._router.navigate(['/app/create-customer']);
      });
  }

  ngOnInit() {
    this.customer = new CreateOrEditCustomerDto();
    this.userNamePassword = new UserNamePasswordDto();
    this.customer.userPassword = [];
    this._activatedRoute.params.subscribe(parms => {
      if (parms.id) {
        this.customer.id = this.customerId = parms.id;
        this._customerServiceProxy.getCustomerUsers(parms.id).pipe(
          finalize(() => {
          })
        )
          .subscribe((result) => {
            if (result != null) {
              this.customer.userPassword = result;
              this.userNamePasswords = result;
              this.userNamePassword.type = 'Personal';
            }
          });
      } else {
        this.userNamePassword.type = 'Personal';
      }
    });
  }
}
