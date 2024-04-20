import { Component, Injector, Input, ViewChild } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, NgForm } from '@angular/forms';
import { AppComponentBase } from '@shared/app-component-base';
import { CustomerDto, CustomerInfoDto, CustomerServiceProxy, DLStatesServiceProxy, Dependent, IncomeDetails, PersonalTaxDto, PersonalTaxServiceProxy, Spouse, SpouseDto, TaxServiceMasterServiceProxy } from '@shared/service-proxies/service-proxies';
import * as moment from 'moment';
import { NgxSpinnerService } from 'ngx-spinner';

@Component({
  selector: 'app-personal-tax',
  templateUrl: './personal-tax.component.html',
  styleUrls: ['./personal-tax.component.scss']
})
export class PersonalTaxComponent extends AppComponentBase {

  @Input() customerId;

  constructor(private injector: Injector,
    private taxMasterService: TaxServiceMasterServiceProxy,
    private customerService: CustomerServiceProxy,
    private spinner: NgxSpinnerService,
    private dlStateService: DLStatesServiceProxy,
    private fb: FormBuilder,
    private personalTaxService: PersonalTaxServiceProxy) {
    super(injector)

    this.personalTaxObj = new PersonalTaxDto();

    this.personalTaxObj.spouse = new Spouse();

    this.personalTaxObj.dependents = [] as Dependent[];


    this.dependentDetailForm = new FormGroup({
      id: new FormControl(0),
      dependentName: new FormControl(''),
      ssn: new FormControl(''),
      relation: new FormControl(''),
      dob: new FormControl('')
    });

    this.incomeDetailForm = new FormGroup({
      id: new FormControl(0),
      incomeDescription: new FormControl(''),
      amount: new FormControl(''),
      federalWH: new FormControl(''),
      stateWH: new FormControl('')
    })

    this.otherExpenseForm = new FormGroup({
      expenseName: new FormControl('Mortgage Interest'),
      expenseAmount: new FormControl('')
    });

    this.tblForms = this.fb.group({
      dependendentDetail: this.fb.array([this.dependentDetailForm]),
      incomeDetail: this.fb.array([this.incomeDetailForm]),
      otherExpense: this.fb.array([this.otherExpenseForm])
    });
  }

  financialYearArr: number[] = [];
  taxFillingStatus: any[] = [];
  tenureArr: any[] = [];
  formList: any[] = [];

  @ViewChild('personalTaxForm') personalTaxForm!: NgForm;

  showOtherExpense: boolean = false;
  dlStateArr: any[] = [];
  incomeDescArr: any[] = [];
  personalTaxObj: PersonalTaxDto = new PersonalTaxDto();

  startEndDate: string = null;

  dueDate: string = null;


  totalOtherExpense: number = 0.00;

  selectedFinancialYear: any = null;

  issueDateAlt: Date;

  spouseIssueDateAlt: Date;

  SpouseExpiryDateAlt: Date;

  DependentDobDateAlt: Date[] = [];

  expiryDateAlt: Date;

  dependentDetailForm: FormGroup;

  incomeDetailForm: FormGroup;

  otherExpenseForm: FormGroup;

  tblForms: FormGroup;

  async ngOnInit(): Promise<void> {
    this.generateFinancialYears();
    await this.reRender();
  }

  async reRender(): Promise<void> {
    this.spinner.show();

    this.tblForms = this.fb.group({
      dependendentDetail: this.fb.array([this.dependentDetailForm]),
      incomeDetail: this.fb.array([this.incomeDetailForm]),
      otherExpense: this.fb.array([this.otherExpenseForm])
    });
    this.selectedFinancialYear = this.personalTaxObj.financialYear ?? this.financialYearArr[0];
    await this.getTaxFillingStatus();
    await this.getTenure();
    await this.getFormList();
    await this.getStates();
    await this.getIncomeDescriptions();
    await this.getpersonalTaxData();
    await this.getCustomerData();
  }

  async getpersonalTaxData(): Promise<void> {
    this.personalTaxObj.customerId = this.customerId;
    this.personalTaxObj.financialYear = this.selectedFinancialYear ?? this.financialYearArr[0];
    await this.personalTaxService.personalTaxGet(this.personalTaxObj).subscribe((res) => {
      this.personalTaxObj.accountNumber = res.accountNumber;
      this.personalTaxObj.routingNumber = res.routingNumber;
      this.personalTaxObj.bankName = res.bankName;

      if (!res.incomeDetails || res.incomeDetails.length == 0) {
        this.incomeDetails.controls[0].get('id').setValue(0);
        this.incomeDetails.controls[0].get('incomeDescription').setValue('W-3');
        this.incomeDetails.controls[0].get('federalWH').setValue(null);
        this.incomeDetails.controls[0].get('stateWH').setValue(null);
        this.incomeDetails.controls[0].get('amount').setValue(null);
      }

      else {


        for (var i = 0; i < res.incomeDetails.length; i++) {
          if (i == 0) {
            this.incomeDetails.controls[i].get('id').setValue(res.incomeDetails[i].id);
            this.incomeDetails.controls[i].get('incomeDescription').setValue(res.incomeDetails[i].incomeDescription);
            this.incomeDetails.controls[i].get('federalWH').setValue(res.incomeDetails[i].federalWH);
            this.incomeDetails.controls[i].get('stateWH').setValue(res.incomeDetails[i].stateWH);
            this.incomeDetails.controls[i].get('amount').setValue(res.incomeDetails[i].amount);
          }
          else {
            this.incomeDetails.push(new FormGroup({
              id: new FormControl(res.incomeDetails[i].id),
              incomeDescription: new FormControl(res.incomeDetails[i].incomeDescription),
              amount: new FormControl(res.incomeDetails[i].amount),
              federalWH: new FormControl(res.incomeDetails[i].federalWH),
              stateWH: new FormControl(res.incomeDetails[i].stateWH)
            }));
          }
        }

      }
      var otherExpense = [];
      if (res.otherExpense) {
        otherExpense = JSON.parse(res.otherExpense);
        this.showOtherExpense = true;
      }

      else {
        this.showOtherExpense = false;
        this.otherExpenses.controls[0].get('expenseName').setValue('Mortgage Interest');
        this.otherExpenses.controls[0].get('expenseAmount').setValue(null);
      }

      this.totalOtherExpense = 0.00;

      for (var i = 0; i < otherExpense.length; i++) {
        if (i == 0) {
          this.otherExpenses.controls[i].get('expenseName').setValue(otherExpense[i].expenseName);
          this.otherExpenses.controls[i].get('expenseAmount').setValue(otherExpense[i].expenseAmount);
        }
        else {
          this.otherExpenses.push(new FormGroup({
            expenseName: new FormControl(otherExpense[i].expenseName),
            expenseAmount: new FormControl(otherExpense[i].expenseAmount)
          }));
        }
        this.totalOtherExpense += parseFloat(otherExpense[i].expenseAmount);
      }


    },
      ({ error }) => {
        this.notify.error("Cannot retrieve tax data")
      })
  }

  async getCustomerData(): Promise<void> {
    await this.customerService.getCustomer(this.customerId).subscribe((res: CustomerInfoDto) => {

      this.customerService.getSpouse(res.spouse.id).subscribe((spouseRes: SpouseDto) => {
        //c-here
        this.customerService.dependentGetListByCustomerId(res.id).subscribe((depRes: Dependent[]) => {
          this.personalTaxObj.filerOccupation = res.jobDescription;
          this.personalTaxObj.spouse.spouseJobDescription = spouseRes.spouseJobDescription;
          this.personalTaxObj.filersLicenseNumber = res.drivingLicense;
          this.personalTaxObj.issueDate = res.dlIssue;
          this.issueDateAlt = res.dlIssue ? new Date((res.dlIssue).format('YYYY-MM-DD')) : null;
          this.personalTaxObj.expiryDate = res.dlExpiry;
          this.expiryDateAlt = res.dlExpiry ? new Date(res.dlExpiry.format('YYYY-MM-DD')) : null;
          this.personalTaxObj.issueState = res.dlState;
          this.personalTaxObj.threeDigitCode = res.code;
          this.personalTaxObj.spouse.drivingLicense = spouseRes.drivingLicense;
          this.personalTaxObj.issueDate = spouseRes.dlIssue;
          this.spouseIssueDateAlt = spouseRes.dlIssue ? new Date(spouseRes.dlIssue.format('YYYY-MM-DD')) : null;
          this.SpouseExpiryDateAlt = spouseRes.dlExpiry ? new Date(spouseRes.dlExpiry.format('YYYY-MM-DD')) : null;
          this.personalTaxObj.spouse.dlExpiry = spouseRes.dlExpiry;
          this.personalTaxObj.spouse.dlState = spouseRes.dlState;
          this.personalTaxObj.spouse.code = spouseRes.code;
          this.personalTaxObj.spouse.id = spouseRes.id;
          for (var i = 0; i < depRes.length; i++) {
            if (i == 0) {
              this.dependent.controls[i].get('id').setValue(depRes[i].id);
              this.dependent.controls[i].get('dependentName').setValue(depRes[i].name);
              this.dependent.controls[i].get('ssn').setValue(depRes[i].ssn);
              this.dependent.controls[i].get('relation').setValue(depRes[i].relation);
              this.dependent.controls[i].get('dob').setValue(new Date(depRes[i].dateOfBirth.format('YYYY-MM-DD')));
            }
            else {

              this.dependent.push(new FormGroup({
                id: new FormControl(depRes[i].id),
                dependentName: new FormControl(depRes[i].name),
                ssn: new FormControl(depRes[i].ssn),
                relation: new FormControl(depRes[i].relation),
                dob: new FormControl(new Date(depRes[i].dateOfBirth.format('YYYY-MM-DD')))
              }));

            }
          }
          this.spinner.hide();

        });
      });
    });
  }

  async getStates(): Promise<void> {
    await this.dlStateService.getDLState().subscribe((res) => {
      this.dlStateArr = res;
    })
  }

  async getTaxFillingStatus(): Promise<void> {
    await this.taxMasterService.getTaxFillingStatus().subscribe((res) => {
      this.taxFillingStatus = res;
      this.personalTaxObj.taxFillingStatus = res[0].value;
    });
  }

  async getTenure(): Promise<void> {
    await this.taxMasterService.getTenure().subscribe((res) => {
      this.tenureArr = res;
      this.personalTaxObj.tenure = res[0].value;
    })
  }

  async getFormList(): Promise<void> {
    await this.taxMasterService.getFormList().subscribe((res) => {
      this.formList = res;
      this.personalTaxObj.form = res[0];
    })
  }


  onDateChange(key: string): void {
    if (key == "issueDate") {
      this.personalTaxObj.issueDate = moment(this.issueDateAlt);
    }
    else if (key == "expiryDate") {
      this.personalTaxObj.expiryDate = moment(this.expiryDateAlt);
    }
  }


  onSpouseDateChange(key: string): void {
    if (key == "issueDate") {
      this.personalTaxObj.spouse.dlIssue = moment(this.issueDateAlt);
    }
    else if (key == "expiryDate") {
      this.personalTaxObj.spouse.dlExpiry = moment(this.expiryDateAlt);
    }
  }

  onDependentDateChange(index: number): void {
    this.personalTaxObj.dependents.push(new Dependent());
    this.personalTaxObj.dependents[index].dateOfBirth = moment(this.DependentDobDateAlt[index]);
    if (this.DependentDobDateAlt[index]) {
      this.addDependent(index);
    }
  }

  async getIncomeDescriptions(): Promise<void> {
    await this.taxMasterService.getIncomeDescriptions().subscribe((res) => {
      this.incomeDescArr = res;
    });
  }

  generateFinancialYears(): void {
    let year = new Date().getFullYear() - 1;

    for (var i = 0; i < 5; i++) {
      this.financialYearArr.push(year);
      year -= 1;

    }

    this.personalTaxObj.financialYear = this.financialYearArr[0];

    this.generateStartEndDate();
  }

  generateStartEndDate(): void {
    let startDate = new Date(this.personalTaxObj.financialYear, 1, 1);
    let endDate = new Date(this.personalTaxObj.financialYear, 12, 31);
    this.startEndDate = moment(startDate).format('MM/DD/YY') + '-' + moment(endDate).format('MM/DD/YY');
    this.generateDueDate();
  }

  generateDueDate(): void {
    this.dueDate = moment(new Date(this.personalTaxObj.financialYear, 1, 15)).format('MM/DD/YY');
  }

  get dependent() {
    return this.tblForms.controls["dependendentDetail"] as FormArray;
  }

  get incomeDetails() {
    return this.tblForms.controls["incomeDetail"] as FormArray;
  }


  get otherExpenses() {
    return this.tblForms.controls["otherExpense"] as FormArray;
  }

  isFormControlEmpty(control: FormControl): boolean {
    return control.value === null || control.value === '';
  }

  areAllControlsEmpty(formGroup: FormGroup | FormArray): boolean {
    for (const controlName in formGroup.controls) {
      const control = formGroup.controls[controlName];

      if (control instanceof FormControl) {
        if (!this.isFormControlEmpty(control)) {
          return false;
        }
      } else if (control instanceof FormGroup || control instanceof FormArray) {
        if (!this.areAllControlsEmpty(control)) {
          return false;
        }
      }
    }

    return true;
  }

  isAnyControlEmpty(formGroup: FormGroup | FormArray): boolean {
    for (const controlName in formGroup.controls) {
      const control = formGroup.controls[controlName];

      if (control instanceof FormControl) {
        if (this.isFormControlEmpty(control)) {
          return true;
        }
      } else if (control instanceof FormGroup || control instanceof FormArray) {
        if (this.isAnyControlEmpty(control)) {
          return true;
        }
      }
    }

    return false;
  }

  addDependent(index: number): void {


    //if all controls are empty of the specified index and there is no row after that index then add row
    if (!this.areAllControlsEmpty(this.dependent.at(index) as FormGroup) && !this.dependent.at(index + 1)) {
      this.dependent.push(new FormGroup({
        id: new FormControl(0),
        dependentName: new FormControl(''),
        ssn: new FormControl(''),
        relation: new FormControl(''),
        dob: new FormControl('')
      }));
    }

    //else delete row from that specified index excluding the specified index
    else if (this.areAllControlsEmpty(this.dependent.at(index) as FormGroup)) {
      const elementsToRemove = this.dependent.length - (index + 1);

      // Remove the elements starting from the specified index
      for (let i = 0; i < elementsToRemove; i++) {
        this.dependent.removeAt(index);;
      }
    }
  }

  calculateOtherExpense(): void {
    let el = document.getElementsByClassName('otherExpenseAmount');
    let el2 = document.getElementsByClassName('otherExpenseName');
    this.totalOtherExpense = 0.00;
    for (var i = 0; i < el.length; i++) {
      if ((el[i] as HTMLInputElement).value && (el2[i] as HTMLInputElement).value) {
        this.totalOtherExpense += parseFloat((el[i] as HTMLInputElement).value);
      }
    }
  }


  addIncomeDetail(index: number): void {


    //if all controls are empty of the specified index and there is no row after that index then add row
    if (!this.areAllControlsEmpty(this.incomeDetails.at(index) as FormGroup) && !this.incomeDetails.at(index + 1)) {
      this.incomeDetails.push(new FormGroup({
        id: new FormControl(0),
        incomeDescription: new FormControl(''),
        amount: new FormControl(''),
        federalWH: new FormControl(''),
        stateWH: new FormControl('')
      }));

      this.showOtherExpense = true;
    }

    //else delete row from that specified index excluding the specified index
    else if (this.areAllControlsEmpty(this.incomeDetails.at(index) as FormGroup)) {
      const elementsToRemove = this.incomeDetails.length - (index + 1);

      // Remove the elements starting from the specified index
      for (let i = 0; i < elementsToRemove; i++) {
        this.incomeDetails.removeAt(index);
      }

      if (this.incomeDetails.length == 0) {
        this.showOtherExpense = false;
      }
    }
  }

  addOtherExpense(): void {
    this.otherExpenses.push(new FormGroup({
      expenseName: new FormControl(''),
      expenseAmount: new FormControl('')
    }));
  }

  savePersonalTax(): void {
    this.personalTaxObj.dependents = [];
    this.personalTaxObj.incomeDetails = [];
    let expenseArr = [];

    for (var form of this.dependent.controls) {
      if (!form.get('dependentName').value) {
        continue;
      }
      let depObj = new Dependent();
      depObj.id = form.get('id').value;
      depObj.name = form.get('dependentName').value;
      depObj.ssn = form.get('ssn').value;
      depObj.relation = form.get('relation').value;
      depObj.dateOfBirth = form.get('dob').value;
      this.personalTaxObj.dependents.push(depObj);
    }


    for (var form of this.incomeDetails.controls) {
      if (this.isAnyControlEmpty(form as FormGroup)) {
        continue;
      }
      let incomeDetailObj = new IncomeDetails();
      incomeDetailObj.incomeDescription = form.get('incomeDescription').value;
      incomeDetailObj.id = form.get('id').value;
      incomeDetailObj.amount = form.get('amount').value;
      incomeDetailObj.federalWH = form.get('federalWH').value;
      incomeDetailObj.stateWH = form.get('stateWH').value;

      this.personalTaxObj.incomeDetails.push(incomeDetailObj);
    }



    for (var form of this.otherExpenses.controls) {
      if (this.isAnyControlEmpty(form as FormGroup)) {
        continue;
      }

      let expenseObj = {
        expenseName: form.get('expenseName').value,
        expenseAmount: form.get('expenseAmount').value
      }

      expenseArr.push(expenseObj);

    }

    this.personalTaxObj.otherExpense = expenseArr.length > 0 ? JSON.stringify(expenseArr) : null;

    this.personalTaxObj.customerId = this.customerId;

    console.log(this.personalTaxObj);

    this.personalTaxService.savePersonalTax(this.personalTaxObj).subscribe((res) => {
      this.notify.success("Personal Tax Saved");
      this.getpersonalTaxData();
    },
      ({ error }) => {
        this.notify.error("Cannot Save Personal Tax");
      })

  }

}
