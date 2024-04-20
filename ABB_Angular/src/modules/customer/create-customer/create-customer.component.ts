import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute } from '@angular/router';
import { CreateOrEditCustomerDto, CustomerServiceProxy, SourceReferralTypeServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-create-customer',
  templateUrl: './create-customer.component.html',
  styleUrls: ['./create-customer.component.css']
})
export class CreateCustomerComponent implements OnInit {
  @Output() onSave = new EventEmitter<any>();
  @Input() customer = new CreateOrEditCustomerDto();
  customerId = 0;
  isDetailOrEditView = false;
  comment: string = "";
  @Input() activeTab: string = 'Info';


  @ViewChild('CustomerInfoViewComponent') CustomerInfoViewComponent;


  customer1: any;
  constructor(
    public _dialog: MatDialog,
    private _activatedRoute: ActivatedRoute,
    public _customerServiceProxy: CustomerServiceProxy,
    public _sourceReferralTypeServiceProxy: SourceReferralTypeServiceProxy,
  ) { }

  ngOnInit() {
    this.customer = new CreateOrEditCustomerDto();
    this.isDetailOrEditView = false;
    this._activatedRoute.params.subscribe(parms => {
      if (parms.id) {
        this.customerId = parms.id;
        this.isDetailOrEditView = true;
      }
    });
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


  tabChange(event): void {
    if (event.index == 0) {

    }
  }

  moveNextTab(comment = "") {
    this._customerServiceProxy.saveCustomerComment(this.customerId, comment).subscribe({
      next: (res) => {

      },
      error: (err) => {
      }
    });
    for (let i = 0; i < document.querySelectorAll('.mat-tab-label-content').length; i++) {
      if ((<HTMLElement>document.querySelectorAll('.mat-tab-label-content')[i]).innerText == this.activeTab) {
        (<HTMLElement>document.querySelectorAll('.mat-tab-label')[i]).click();
      }
    }
  }

  save() {

  }
}


