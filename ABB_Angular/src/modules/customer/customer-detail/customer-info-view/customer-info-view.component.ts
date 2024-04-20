import { Component, ElementRef, EventEmitter, Injector, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { AppComponentBase } from '@shared/app-component-base';
import { CreateOrEditCustomerDto, CustomerServiceProxy, CustomerInfoDto, SpouseDto, DependentDto, CustomerTypeDto, CustomerTypesServiceProxy, SourceReferralType, SourceReferralTypeDto, SourceReferralTypeServiceProxy, DLStatesServiceProxy, DLState, Dependent, RoleServiceProxy, User } from '@shared/service-proxies/service-proxies';
import { AppSessionService } from '@shared/session/app-session.service';
import { Console } from 'console';
import { CreateorEditCustomerTypeComponent } from 'modules/customer-type/create-or-edit-customer-type/create-or-edit-customer-type.component';
import { CreateSourecReferalComponent } from 'modules/source-referal/create-source-referal/create-source-referal.component';
import * as moment from 'moment';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-customer-info-view',
  templateUrl: './customer-info-view.component.html',
  styleUrls: ['./customer-info-view.component.scss']
})
export class CustomerInfoViewComponent extends AppComponentBase implements OnInit {
  @Output() onSave = new EventEmitter<any>();
  @Output() activeTab = new EventEmitter<any>();
  @Output() customerId = new EventEmitter<any>();
  @ViewChild('fiscalYearEndInput') fiscalYearEndInput: ElementRef<HTMLInputElement>;

  customerTypeDto: CustomerTypeDto[] = [];
  customerTypes = [];
  sourceReferralTypes: SourceReferralTypeDto[] = [];

  dependentForm: FormGroup;

  fb: FormBuilder;


  @Input() customer = new CreateOrEditCustomerDto();
  @Input() totalBalance: number = 0;

  isDetailView = false;
  showSpouseField = false;
  showDependentField = false;
  showDlCode = false;
  showSpouseDlCode = false;
  fiscalYearEndAlt: Date;
  dobAlt: Date;
  dlIssueAlt: Date;
  dlExpiryAlt: Date;
  spouseDobAlt: Date;
  spouseDlIssueAlt: Date;
  spouseDlExpiryAlt: Date;

  dlStatesArr: DLState[] = [];

  taxIdMask = {
    guide: true,
    showMask: true,
    mask: [/\d/, /\d/, '-', /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/]
  };

  custSSNMask = {
    guide: true,
    showMask: true,
    mask: [/\d/, /\d/, /\d/, '-', /\d/, /\d/, '-', /\d/, /\d/, /\d/, /\d/]
  };

  constructor(
    injector: Injector,
    public _dialog: MatDialog,
    protected _router: Router,
    private _activatedRoute: ActivatedRoute,
    public _customerServiceProxy: CustomerServiceProxy,
    public _sourceReferralTypeServiceProxy: SourceReferralTypeServiceProxy,
    public _customerTypesServiceProxy: CustomerTypesServiceProxy,
    private _dlstateService: DLStatesServiceProxy,
    private _appSessionService: AppSessionService,
    private _roleService: RoleServiceProxy
  ) {
    super(injector);

    this.fb = new FormBuilder();


  }

  public ngOnInit() {
    this.getDLStates();
    this.CustomerTypeList();
    this.getSourceReferralTypes();
    this.intialization();
    this.dependentForm = this.fb.group({
      dependendentDetail: this.fb.array([new FormGroup({
        id: new FormControl(0),
        dependentName: new FormControl(''),
        relation: new FormControl(''),
        ssn: new FormControl(''),
        dob: new FormControl('')
      })]),
    });

    this._activatedRoute.params.subscribe(parms => {
      if (parms.id) {
        this.isDetailView = true;
        this.customerId.emit(parms.id);

        this._customerServiceProxy.getCustomerInfo(parms.id).pipe(
          finalize(() => {
          })
        )
          .subscribe((result) => {
            this.customer = result;
            this.onFiscalYearChange();
            this.showSpouseDlCode = this.customer.customerInfo.spouse.dlState == 37;
            this.showDlCode = this.customer.customerInfo.dlState == 37;
            this.fiscalYearEndAlt = new Date(moment(result.fiscalYearEnd).format('YYYY-MM-DD'));
            this.dobAlt = new Date(moment(result.dateOfBirth).format('YYYY-MM-DD'));
            this.dlIssueAlt = new Date(moment(result.customerInfo.dlIssue).format('YYYY-MM-DD'));
            this.dlExpiryAlt = new Date(moment(result.customerInfo.dlExpiry).format('YYYY-MM-DD'));
            this.spouseDobAlt = new Date(moment(result.customerInfo.spouse.dateOfBirth).format('YYYY-MM-DD'));
            this.spouseDlIssueAlt = new Date(moment(result.customerInfo.spouse.dlIssue).format('YYYY-MM-DD'));
            this.spouseDlExpiryAlt = new Date(moment(result.customerInfo.spouse.dlExpiry).format('YYYY-MM-DD'));
            if (this.customer.customerInfo.customerType == null) {
              this.customer.customerInfo.customerType = new CustomerTypeDto();
            }


            if (this.customer.customerInfo.spouse == null) {
              this.customer.customerInfo.spouse = new SpouseDto();
            }


            if (this.customer.customerInfo.dependent == null || this.customer.customerInfo.dependent.length == 0) {
              this.customer.customerInfo.dependent = [];
            }
            else {
              for (var i = 0; i < result.customerInfo.dependent.length; i++) {

                if (i == 0) {
                  this.dependent.controls[i].get('id').setValue(result.customerInfo.dependent[i].id);
                  this.dependent.controls[i].get('dependentName').setValue(result.customerInfo.dependent[i].name);
                  this.dependent.controls[i].get('ssn').setValue(result.customerInfo.dependent[i].ssn);
                  this.dependent.controls[i].get('relation').setValue(result.customerInfo.dependent[i].relation);
                  this.dependent.controls[i].get('dob').setValue(new Date(result.customerInfo.dependent[i].dateOfBirth.format('YYYY-MM-DD')));
                }
                else {
                  this.dependent.push(new FormGroup({
                    id: new FormControl(result.customerInfo.dependent[i].id),
                    dependentName: new FormControl(result.customerInfo.dependent[i].name),
                    ssn: new FormControl(result.customerInfo.dependent[i].ssn),
                    relation: new FormControl(result.customerInfo.dependent[i].relation),
                    dob: new FormControl(new Date(result.customerInfo.dependent[i].dateOfBirth.format('YYYY-MM-DD')))
                  }));
                }
              }
            }


            if (this.customer.customerInfo.sourceReferralType == null) {
              this.customer.customerInfo.sourceReferralType = new SourceReferralTypeDto();
            }


          });
      } else {
        this.intialization();
      }


    });

  }




  onFiscalYearChange(): void {
    if (document.getElementsByClassName('mat-calendar-period-button') && document.getElementsByClassName('mat-calendar-period-button').length > 0) {
      (document.getElementsByClassName('mat-calendar-period-button')[0] as HTMLButtonElement).disabled = true;
    }
    setTimeout(() => {
      if (this.fiscalYearEndInput.nativeElement.value) {
        this.fiscalYearEndInput.nativeElement.value = moment(new Date(this.fiscalYearEndInput.nativeElement.value)).format('MM/DD');
      }
    }, 10);
  }

  onCustDateChange(key) {
    if (key == 'dob') {
      this.customer.customerInfo.dateOfBirth = moment(this.dobAlt);
    }
    else if (key == 'dlIssue') {
      this.customer.customerInfo.dlIssue = moment(this.dlIssueAlt);
    }
    else if (key == 'dlExpiry') {
      this.customer.customerInfo.dlExpiry = moment(this.dlExpiryAlt);
    }
    else if (key == 'fiscalYearEnd') {
      this.customer.customerInfo.fiscalYearEnd = moment(this.fiscalYearEndAlt);
      this.onFiscalYearChange();
    }
  }

  onSpouseDateChange(key) {
    if (key == 'dob') {
      this.customer.customerInfo.spouse.dateOfBirth = moment(this.dobAlt);
    }
    else if (key == 'dlIssue') {
      this.customer.customerInfo.spouse.dlIssue = moment(this.dlIssueAlt);
    }
    else if (key == 'dlExpiry') {
      this.customer.customerInfo.spouse.dlExpiry = moment(this.dlExpiryAlt);
    }
  }


  get dependent() {
    return this.dependentForm.controls["dependendentDetail"] as FormArray;
  }

  addDependent(): void {
    this.dependent.push(new FormGroup({
      id: new FormControl(0),
      dependentName: new FormControl(''),
      ssn: new FormControl(''),
      relation: new FormControl(''),
      dob: new FormControl('')
    }));
  }

  getDLStates(): void {
    this._dlstateService.getDLState().subscribe((res) => {
      this.dlStatesArr = res;
    });
  }

  toggleDLCode(): void {
    this.showDlCode = this.customer.customerInfo.dlState == 37;
    this.customer.customerInfo.code = this.dlStatesArr.find(x => x.id == this.customer.customerInfo.dlState).stateCode;
  }

  toggleSpouseDLCode(): void {
    this.showSpouseDlCode = this.customer.customerInfo.spouse.dlState == 37;
    this.customer.customerInfo.spouse.code = this.dlStatesArr.find(x => x.id == this.customer.customerInfo.spouse.dlState).stateCode;

  }

  intialization() {
    this.customer = new CreateOrEditCustomerDto();
    this.customer.customerInfo = new CustomerInfoDto();
    this.customer.customerInfo.spouse = new SpouseDto();
    this.customer.customerInfo.dependent = [];
    this.customer.customerInfo.customerType = new CustomerTypeDto();
    this.customer.customerInfo.sourceReferralType = new SourceReferralTypeDto();

  }
  backToListView(): void {
    this._router.navigate(['/app/customers']);
  }
  save() {

    this.customer.customerInfo.dependent = [];
    for (var form of this.dependent.controls) {

      let depObj = new Dependent();
      depObj.id = form.get('id').value;
      depObj.name = form.get('dependentName').value;
      depObj.ssn = form.get('ssn').value;
      depObj.relation = form.get('relation').value;
      depObj.dateOfBirth = form.get('dob').value;

      if (!depObj.name) {
        continue;
      }

      this.customer.customerInfo.dependent.push(depObj);
    }

    this.customer.customerInfo.dependent = [...new Set(this.customer.customerInfo.dependent)];
    this._customerServiceProxy.createOrEdit(this.customer).pipe(
      finalize(() => {
      })
    )
      .subscribe((result) => {
        this.customer = result;
        this.onFiscalYearChange();
        this.showSpouseDlCode = this.customer.customerInfo.spouse.dlState == 37;
        this.showDlCode = this.customer.customerInfo.dlState == 37;
        // emit customer Id for Other Relevant Tabs
        this.customerId.emit(this.customer.id);
        // Emit Next Tab name
        this.activeTab.emit('Detail');
        this.notify.info(this.l('Customer info saved succesfully'));
        this.onSave.emit(this.customer.comment ?? "");
      });
  }

  CustomerTypeList() {
    this._customerTypesServiceProxy
      .getAll()
      .subscribe((arg) => (this.customerTypeDto = arg));
  }

  public openCustomerTypeDialog(id?: number): void {
    const dialogRef = this._dialog.open(CreateorEditCustomerTypeComponent, {
      data: { id: id },
    });
    dialogRef.afterClosed().subscribe((result) => {
      this.CustomerTypeList();
      dialogRef.close();
    });
  }
  public openCreateSourceReferralTypeDialog(id?: number): void {
    const dialogRef = this._dialog.open(CreateSourecReferalComponent, {
      data: { id: id },
    });

    dialogRef.afterClosed().subscribe((result) => {
      dialogRef.close();
      this.getSourceReferralTypes();
    });
  }

  protected getSourceReferralTypes() {
    this._sourceReferralTypeServiceProxy
      .getAll()
      .subscribe((arg) => (this.sourceReferralTypes = arg));
  }
}
