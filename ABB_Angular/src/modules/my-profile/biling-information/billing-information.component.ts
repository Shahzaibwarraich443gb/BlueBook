import {
  ChangeDetectorRef,
  Component,
  Injector,
  Input,
  OnInit,
} from "@angular/core";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { AppComponentBase } from "@shared/app-component-base";
import { ContactDetailsServiceProxy, CreateOrEditContactDetalsInputDto, CreateOrEditPersonalInformationInputDto } from "@shared/service-proxies/service-proxies";
import { AppSessionService } from "@shared/session/app-session.service";
import { finalize } from "rxjs/operators";

import { BsDatepickerConfig } from "ngx-bootstrap/datepicker";
import { NgxSpinnerService } from "ngx-spinner";

@Component({
  selector: "app-billing-information",
  templateUrl: "./billing-information.component.html",
  styleUrls: ["./billing-information.component.scss"],
})
export class BillingInformationComponent
  extends AppComponentBase
  implements OnInit {

  contactDetaildto = new CreateOrEditContactDetalsInputDto();

  tinType
  genderTitle
  addressType
  emailType
  phoneType
  bsConfig: Partial<BsDatepickerConfig>;
  formDto: any
  form: FormGroup;
  isLoading = false;
  isCheck = false;
  rowsPerPageOptions;
  rows = 10;
  cities = [];
  states = [];
  countries = [];
  countriesCodes = [];
  searchtext;
  skipCount;
  first = 0;
  filter: string;
  totalCount: number = 0;
  items = [];
  selectSortColumn: boolean = false;
  validSociaSecurityNo: boolean = false;
  show: boolean = false;

  constructor(
    injector: Injector,
    public _appSessionService: AppSessionService,
    private spinner: NgxSpinnerService,
    public _ContactDetails: ContactDetailsServiceProxy,
    // private _billingInformationService: BillingInformationServiceProxy,
    // private _commonLookUpService: CommonLookupServiceProxy,
    private _cdr: ChangeDetectorRef
  ) {
    super(injector);
  }
  async ngOnInit() {

    abp.event.on("onProfileInformationChanges", () => {
      this.disableForm();
    });
    await this.get();


  }
  disableForm() {
    this.show = false;
  }
  ngAfterViewInit(): void {
    this._cdr.detectChanges();
  }

  async save() {
    /*     if (!this.form.valid) {
          await abp.message
            .error("Please fill all the required fields")
            .then((x) => {
              this.form.markAllAsTouched();
              return;
            });
          return;
        } */


    if (this.contactDetaildto.id) {
      abp.message.confirm(
        "Do you want to update personal information",
        "Warning",
        (result: boolean) => {
          if (result) {
            this.createOrUpdate();
          }
        }
      );
    } else {
      this.contactDetaildto.id = 0;
      this.createOrUpdate();
    }
  }
  createOrUpdate() {

    this._ContactDetails.createOrEdit(this.contactDetaildto).subscribe((arg) => {
      abp.notify.success('save Succesfully')


    });

  }

  get() {
    this.spinner.show();
    this._ContactDetails
      .get()
      .pipe(finalize(() => (this.spinner.hide())))
      .subscribe((x) => {
        this.contactDetaildto = x;


      });
  }


  checkSelectSortFieldSelectOrNot(event: any): boolean {
    // Check selected Field Selected or Not
    // When any Record exit on table view
    let event1 = event;
    if (event1 == undefined) {
      return true;
    } else if (event.sortField == undefined && this.totalCount == 0) {
      return true;
    } else if (event.sortField != null && this.totalCount == 0) {
      return false;
    } else if (event.sortField != null && this.totalCount != 0) {
      return true;
    } else if (event.sortField == undefined) {
      return true;
    }
  }
  btnClear() {
    this.form.reset();
    // this.isUpdate = false;
  }










  firstWordCapital(event) {
    if (event) {
      //event.target.attributes.getNamedItem("formcontrolname").value;
      var controlName =
        event.target.attributes.getNamedItem("ng-reflect-name").value;
      let first = event.target.value.substring(0, 1).toUpperCase();
      this.form.controls[controlName].setValue(
        first + event.target.value.substring(1)
      );
    }
  }
  showScreen(show: boolean) {
    this.show = show;
  }
}
