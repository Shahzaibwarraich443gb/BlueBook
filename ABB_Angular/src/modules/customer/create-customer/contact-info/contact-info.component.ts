import { Component, EventEmitter, Injector, Input, OnInit, Output } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute } from '@angular/router';
import { AppComponentBase } from '@shared/app-component-base';
import { CreateOrEditEmailInputDto, CreateOrEditPhoneDto, CreateOrEditAddressDto, PhoneDto, ContactPersonTypeDto, ContactInfoDto, CreateOrEditCustomerDto, CustomerServiceProxy, EthnicitiesServiceProxy, LanguageServiceProxy, SalesPersonTypeServiceProxy, SourceReferralTypeServiceProxy } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-contact-info',
  templateUrl: './contact-info.component.html',
  styleUrls: ['./contact-info.component.css']
})
export class ContactInfoComponent extends AppComponentBase implements OnInit {
  @Output() onSave = new EventEmitter<any>();
  @Output() activeTab = new EventEmitter<any>();

  customerCotacts: ContactInfoDto[] = [];
  @Input() customerId: number;
  @Input() comment: string;

  @Input() customer = new CreateOrEditCustomerDto();
  customerCotact = new ContactInfoDto();
  phones: CreateOrEditPhoneDto[] = [];
  phone = new CreateOrEditPhoneDto();

  constructor(
    injector: Injector,
    public _dialog: MatDialog,
    private _activatedRoute: ActivatedRoute,
    public _customerServiceProxy: CustomerServiceProxy,
  ) {
    super(injector)
  }
  addMore() {
    if (this.customerCotact.contactType != null && this.customerCotact.name != null) {
      var index = this.customerCotacts.findIndex(x => x == this.customerCotact);
      if (index !== -1) this.customerCotacts[index] = this.customerCotact;
      else this.customerCotacts.push(this.customerCotact);
      
      this.customerCotact = new ContactInfoDto();
      this.customerCotact.contactType = 'Manager';
      this.customerCotact.numberType = 'Personal';
      this.customerCotact.emailType = 'Office';
    }
  }

  updateContactInfo(customerCotact: ContactInfoDto) {
    var index = this.customerCotacts.findIndex(x => x == customerCotact);
    if (index !== -1) this.customerCotact = this.customerCotacts[index];
  }

  deleteContactInfo(customerCotact: ContactInfoDto) {
    var index = this.customerCotacts.findIndex(x => x == customerCotact);
    if (index !== -1) this.customerCotacts.splice(index, 1);
  }

  save() {
    this.addMore();
    if (this.customerId > 0) {
      this.customer.id = this.customerId;
    }
    this.customer.contactPersons = this.customerCotacts;
    this._customerServiceProxy.updateCustomerContactInfo(this.customer)
      .pipe(
        finalize(() => {
        })
      )
      .subscribe((result) => {
        this.customer = result;
        // Emit Next Tab name
        this.activeTab.emit('Address');
        this.notify.info('Customer Contact info saved succesfully');
        this.onSave.emit(this.comment);
      });
  }

  ngOnInit() {
    this.intialization();

    this._activatedRoute.params.subscribe(parms => {
      if (parms.id) {
        this._customerServiceProxy.getCustomerContact(parms.id).pipe(
          finalize(() => {
          })
        )
          .subscribe((result) => {
            if (result != null) {
              this.customerCotacts = result;
              this.customer.contactPersons = result;
            }
          });
      } else {
        this.intialization();
      }
    });

  }

  private intialization() {
    this.customer = new CreateOrEditCustomerDto();
    this.customerCotact = new ContactInfoDto();
    this.customer.contactPersons = [];
    this.customerCotacts = [];
    this.customerCotact.contactType = 'Manager';
    this.customerCotact.numberType = 'Personal';
    this.customerCotact.emailType = 'Office';
  }

  updatePhoneNumber(phone: PhoneDto) {
    var index = this.phones.findIndex(x => x == phone);
    if (index !== -1) this.phone = this.phones[index];
  }

  deletePhoneNumber(phone: PhoneDto) {
    var index = this.phones.findIndex(x => x == phone);
    if (index !== -1) this.phones.splice(index, 1);
  }

  addPhoneNumber() {
    if (this.phone.number != null) {
      var index = this.phones.findIndex(x => x == this.phone);
      if (index !== -1) this.phones[index] = this.phone;
      else this.phones.push(this.phone);
      this.phone = new PhoneDto();
      this.phone.type = 0;
    }
  }
}
