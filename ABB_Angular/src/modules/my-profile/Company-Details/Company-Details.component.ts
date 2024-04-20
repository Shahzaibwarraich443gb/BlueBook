import {
  ChangeDetectorRef,
  Component,
  Injector,
  Input,
  OnInit,
} from "@angular/core";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { AppComponentBase } from "@shared/app-component-base";
import { CompanyDetailsServiceProxy, CreateOrEditMerchantInputDto } from "@shared/service-proxies/service-proxies";
import { AppSessionService } from "@shared/session/app-session.service";
import { finalize } from "rxjs/operators";

import { BsDatepickerConfig } from "ngx-bootstrap/datepicker";

@Component({
  selector: "app-Company-Details",
  templateUrl: "./Company-Details.component.html",
  styleUrls: ["./Company-Details.component.scss"],
})
export class CompanyDetailsComponent
  extends AppComponentBase
  implements OnInit {

  createoreditMerchantDto = new CreateOrEditMerchantInputDto();
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
  merchants = [];
  constructor(
    injector: Injector,
    public _appSessionService: AppSessionService,
    public _companyDetails: CompanyDetailsServiceProxy,
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
    this.getMerchants();


    await this.get();
  }
  disableForm() {
    this.show = false;
  }
  ngAfterViewInit(): void {
    this._cdr.detectChanges();
  }
  onMerchantChange(event) {
    if (event.value) {
      let merchantId = event.value;
      this._companyDetails.getMerchantOnId(merchantId).subscribe(x => {
        this.createoreditMerchantDto = x;
      })






    }
  }

  async save() {

    /* if (!this.form.valid) {
      await abp.message
        .error("Please fill all the required fields")
        .then((x) => {
          this.form.markAllAsTouched();
          return;
        });
    }
    if (this.formDto.id) {
      //update mode
      abp.message.confirm(
        "Do you want to update billing information",
        "Warning",
        (result: boolean) => {
          if (result) {
            this.createOrUpdate();
          }
        }
      );
    } else {
      //create
      this.formDto.id = 0;
      this.createOrUpdate();
    } */
  }
  /*   createOrUpdate() {
      this.getValuesIntoObject();
      this.isLoading = true;
      this._billingInformationService
        .createOrEdit(this.formDto)
        .pipe(finalize(() => (this.isLoading = false)))
        .subscribe((r) => {
          abp.notify.info("Saved Successfully");
          this.form.reset();
          this.getCountry();
          this.get();
        });
    } */

  get() {

    this._companyDetails.getMerchant()
      .pipe(finalize(() => (this.isLoading = false)))
      .subscribe((x) => {

        this.createoreditMerchantDto = x;


      });
  }
  getMerchants() {
    this._companyDetails.getMerchants().pipe(finalize(() => (this.isLoading = false)))
      .subscribe((x) => {

        this.merchants = x;


      });

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
  savedata() {

    this._companyDetails.updateSelectedMerchant(this.createoreditMerchantDto.id).pipe(finalize(() => (this.isLoading = false)))
      .subscribe((x) => {

        abp.message.success("Successfully Selected")


      });
  }
}
