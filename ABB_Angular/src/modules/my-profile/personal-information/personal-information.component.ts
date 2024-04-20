import {
  ChangeDetectorRef,
  Component,
  ElementRef,
  EventEmitter,
  Injector,
  Input,
  OnInit,
  Output,
  ViewChild,
} from "@angular/core";
import * as moment from "moment";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { AppComponentBase } from "@shared/app-component-base";
import { AppSessionService } from "@shared/session/app-session.service";
import { AppConsts } from "@shared/AppConsts";
import { finalize } from "rxjs/operators";

import { BsDatepickerConfig } from "ngx-bootstrap/datepicker";
import { MatDatepicker } from "@angular/material/datepicker";
import { CreateOrEditPersonalInformationInputDto, PersonalInformationServiceProxy } from "@shared/service-proxies/service-proxies";
import { MatDialogRef } from "@angular/material/dialog";
import { NgxSpinnerService } from "ngx-spinner";

@Component({
  selector: "app-personal-information",
  templateUrl: "./personal-information.component.html",
  styleUrls: ["./personal-information.component.scss"],
})
export class PersonalInformationComponent
  extends AppComponentBase
  implements OnInit {


  PersonalInfo = new CreateOrEditPersonalInformationInputDto();
  @ViewChild("inputEl") inputEl: ElementRef<HTMLInputElement>;
  @ViewChild("imageEl") imageEl: ElementRef<HTMLImageElement>;

  //MyData
  dateofBirth;
  hireDate;
  imgUploaded: boolean = false;

  bsConfig: Partial<BsDatepickerConfig>;
  formDto: any;
  form: FormGroup;
  isLoading = false;
  isCheck = false;
  rowsPerPageOptions;
  rows = 10;
  cities = [];
  states = [];
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
  minDateOfBirth = moment().subtract(125, "year").startOf("year").toDate();
  maxDateOfBirth = moment().subtract(18, "year").startOf("year").toDate();
  @ViewChild('releasedAtPicker') releasedAtPicker: MatDatepicker<Date>;
  @ViewChild('releasedAtPicker2') releasedAtPicker2: MatDatepicker<Date>;
  @Output() onSave = new EventEmitter<any>();


  // title = [{
  //   name: "Mr", value: 1
  // },
  // {
  //   name: "Mr", value: 1
  // }, ,
  // {
  //   name: "Mr", value: 1
  // }, ,
  // {
  //   name: "Mr", value: 1
  // },
  // ];
  constructor(
    injector: Injector,
    public _appSessionService: AppSessionService,
    private spinner: NgxSpinnerService,
    public _personalInformationService: PersonalInformationServiceProxy,

    // private _personalInformationAppService: PersonalInformationServiceProxy,
    // private _commonLookUpService: CommonLookupServiceProxy,
    private _cdr: ChangeDetectorRef
  ) {
    super(injector);
  }
  async ngOnInit() {

    await this.dateofBirth;
    await this.hireDate;
    abp.event.on("onProfileInformationChanges", () => {
      this.disableForm();
    });


    // await this.getStates();
    // await this.getCities();
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

    if (this.PersonalInfo.id) {
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
      this.PersonalInfo.id = 0;
      this.createOrUpdate();
    }
  }
  createOrUpdate() {
    let dateofBirth = this.dateofBirth;
    let hireDate = this.hireDate;
    if (this.dateofBirth) {
      this.PersonalInfo.dateofBirth = moment(new Date(dateofBirth.getFullYear(), dateofBirth.getMonth(), dateofBirth.getDate(), dateofBirth.getHours(), dateofBirth.getMinutes() - dateofBirth.getTimezoneOffset()).toISOString());
    }

    if (this.hireDate) {
      this.PersonalInfo.hireDate = moment(new Date(hireDate.getFullYear(), hireDate.getMonth(), hireDate.getDate(), hireDate.getHours(), hireDate.getMinutes() - hireDate.getTimezoneOffset()).toISOString());
    }
    this._personalInformationService.createOrEdit(this.PersonalInfo).subscribe((arg) => {
      abp.notify.success('save Succesfully')


    });

  }

  get() {

    this.spinner.show();

    this._personalInformationService

      .get()
      .pipe(finalize(() => (this.spinner.hide())))
      .subscribe((x) => {
        this.PersonalInfo = x;
        this.dateofBirth = new Date(this.PersonalInfo.dateofBirth.format('YYYY-MM-DD'));
        this.hireDate = new Date(this.PersonalInfo.hireDate.format('YYYY-MM-DD'));

      });
  }


  edit(id: number) {
    /* if (id > 0) {
      this.form.reset();
      this.states = csc.getAllStates();
      this.cities = csc.getAllCities();
      this.isLoading = true;
      this._personalInformationAppService
        .getOnId(id)
        .pipe(finalize(() => (this.isLoading = false)))
        .subscribe((x) => {
          this.formDto = x;
          this.displayInformation(this.formDto);
        });
    } 
  }

  delete(obj: any) {
    /*  if (obj.id > 0) {
       abp.message.confirm(
         "Do you want to delete personal information" +
           " (" +
           obj.name +
           " " +
           obj.lastName +
           ")" +
           "?",
         "Warning",
         (result: boolean) => {
           if (result) {
             this.isLoading = true;
             this.form.reset();
             this.isLoading = true;
             this._personalInformationAppService
               .delete(obj.id)
               .pipe(finalize(() => (this.isLoading = false)))
               .subscribe((x) => {
                 abp.notify.success("Successfully Deleted");
                 this.getAll();
               });
           }
         }
       );
     } */
  }

  getAll(event?) {
    /*  var checkSelectSortFieldCheckOrNot =
       this.checkSelectSortFieldSelectOrNot(event);
     if (checkSelectSortFieldCheckOrNot == true) {
       //First hit geos and when any record exit on table view
       // and then hit goes
       if (event) {
         this.rows = event.rows;
       }
       this.isLoading = true;
       this.skipCount = 0;
       if (event && event.first) {
         this.skipCount = event.first;
       } else {
         this.first = 0;
       }
 
       const sortOrder = this.getSortOrder(event);
       this.form.reset();
       this._personalInformationAppService
         .getAll(
           this.filter,
           sortOrder.order,
           sortOrder.field,
           this.skipCount,
           this.rows
         )
         .pipe(
           finalize(() => {
             this.isLoading = false;
           })
         )
         .subscribe((result) => {
           this.items = result.items;
           this.totalCount = result.totalCount;
           if (this.totalCount > 100) {
             this.rowsPerPageOptions = [10, 20, 50, 100, this.totalCount];
           } else {
             this.rowsPerPageOptions = [10, 20, 50, 100];
           }
         });
     } else {
       return false;
     } */
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
  }
  invokeFileInput(): void {
    this.inputEl.nativeElement.click();
  }
  getUserImg(): void {
    const fileInput: HTMLInputElement = this.inputEl.nativeElement;

    if (fileInput.files && fileInput.files[0]) {
      this.imgUploaded = true;
      const file = fileInput.files[0];
      const reader = new FileReader();

      // Load image file as Data URL (base64 encoded image)
      reader.readAsDataURL(file);

      // Set the source of the img tag once the file is loaded
      reader.onload = () => {
        const image: HTMLImageElement = this.imageEl.nativeElement;
        image.src = reader.result as string;
      };
    }
  }
  handleCountryChange(event) {
    //this.states = csc.getStatesOfCountry(this.form.value.country.isoCode);
  }
  ValidateZipCode(event?) {
    /* if (event && event.target.value && event.target.value.length == 5) {
      let city, state;
      let zipCode = event.target.value;
      this.isLoading = true;
      this._commonLookUpService
        .getState_CityByZip(zipCode)
        .pipe(finalize(() => (this.isLoading = false)))
        .subscribe((result) => {
          if (result.status == true) {
            this.states = [];
            state = csc.getStateByCodeAndCountry(result.state, "US");
            this.states.push(state);
            this.form.controls["state"].setValue(state);
            city = this.cities.find((city) => city.name === result.city);
            this.form.controls["city"].setValue(city);
          }
        });
    } */
  }
  handleCityChange(event) {
    /*  let cityName = event.value.name;
     let stateCode = event.value.stateCode;
     this.states = [];
     let stateResult = csc.getStateByCodeAndCountry(stateCode, "US");
     if (stateResult != null) {
       this.states.push(stateResult);
       this.form.value.state = stateResult.isoCode;
       this.form.controls["state"].setValue(stateResult);
     }
     this.form.value.cityCode = event.value.name; */
  }

  getCities() {
    // this.cities = csc.getCitiesOfCountry("US");
  }
  getState(stateCode, countryCode, isForm) {
    /*   let state = csc.getStateByCodeAndCountry(stateCode, countryCode);
      if (isForm) {
        return state;
      } else {
        return state.isoCode;
      } */
  }
  getCity(stateCode, countryCode, cityCode, isForm) {
    /*  let city = csc
       .getAllCities()
       .find(
         (x) =>
           x.countryCode == countryCode &&
           stateCode == x.stateCode &&
           cityCode == x.name
       );
     if (isForm) {
       return city;
     } else {
       return cityCode;
     } */
  }
  getStates() {
    // this.states = csc.getStatesOfCountry("US");
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
