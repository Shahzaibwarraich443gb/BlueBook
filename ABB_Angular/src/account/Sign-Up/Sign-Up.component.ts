import { Component, ElementRef, Injector, NgZone, OnInit, QueryList, ViewChild, ViewChildren, ViewEncapsulation } from '@angular/core';
import { AbpSessionService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/app-component-base';
import { accountModuleAnimation } from '@shared/animations/routerTransition';
import { AppAuthService } from '@shared/auth/app-auth.service';
import { AccountServiceProxy, IsTenantAvailableInput, RegisterInput, RegisterOutput, RegisterTenantInputDto, RegisterTenantoutputDto, TenantRegistationServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppTenantAvailabilityState } from '@shared/AppEnums';
import { Router } from '@angular/router';
import { finalize } from 'rxjs';
import { NgForm } from '@angular/forms';
import * as csc from 'country-state-city';

import * as moment from 'moment';
import { HttpClient } from '@angular/common/http';
@Component({
  selector: 'app-Sign-Up',
  templateUrl: 'Sign-Up.component.html',
  styleUrls: ['Sign-Up.component.scss'],
})

export class SignupComponent extends AppComponentBase {
  @ViewChildren('otpInput') otpInputs: QueryList<ElementRef>;
  model: RegisterTenantInputDto = new RegisterTenantInputDto();
  hidePassword = true;
  isLoading: boolean = false;
  otpResult;
  ConfirmNewPassword;
  countryPhoneCode;
  countriesCodes = [];
  isActive: boolean = false;
  isUpperCaseLetter: boolean = false;
  isLowerCaseLetter: boolean = false;
  isSpecialCharacter: boolean = false;
  checked: boolean = false;
  otpValue
  otp: string;
  submitting = false;
  saving = false;
  isNumber: boolean = false;
  phoneNumber;
  passwordValue: string;
  AltfiscalYearStart: any;
  AltfiscalYearEnd: any;
  isMinLength: boolean = false;
  isotpMatch = false;
  showOtpComponent = true;
  formInput = ["input1", "input2", "input3", "input4", "input5", "input6"];
  @ViewChildren("formRow") rows: any;
  constructor(
    injector: Injector,
    public authService: AppAuthService,
    private _sessionService: AbpSessionService,
    private _accountService: AccountServiceProxy,
    private httpClient: HttpClient,
    private _router: Router,
    // private _cookieService: CookieService,
    public _tenatRegistration: TenantRegistationServiceProxy

  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.getuserIP();
  }
  @ViewChild("ngOtpInput", { static: false }) ngOtpInput: any;
  config = {
    allowNumbersOnly: true,
    length: 4,
    isPasswordInput: false,
    disableAutoFocus: false,
    placeholder: "",
    inputStyles: {
      width: "23%",
      height: "50px",
    },
  };
  onOtpChange(otp) {
    this.otp = otp;
  }

  setVal(val) {
    this.ngOtpInput.setValue(val);
  }

  toggleDisable() {
    if (this.ngOtpInput.otpForm) {
      if (this.ngOtpInput.otpForm.disabled) {
        this.ngOtpInput.otpForm.enable();
      } else {
        this.ngOtpInput.otpForm.disable();
      }
    }
  }



  onOtpInput(index: number, event: any) {
    const input = this.otpInputs.toArray();
    const value = event.target.value;

    if (value && index < input.length - 1) {
      input[index + 1].nativeElement.focus();
    }
  }
  navigate() {

    this._router.navigate(['/account/login']);
  }

  confirmPassword(event: string) {
    if (event !== this.model.adminPassword) {
      this.isActive = true;
    } else {
      this.isActive = false;
    }

  }
  validatePassword(value: string) {
    this.passwordValue = value;
    let specialCharacterArray = ["@", "$", "!", "#", "%", "*", "?", "&"];
    const upperCaseRegex = /[A-Z]/g;
    const specialCharacterRegex = /[@$!#%*?&]/g;
    const lowerCaseRegex = /[a-z]/g;
    const numberRegex = /[0-9]/g;
    if (value) {
      const capitalLetterArray = value.match(upperCaseRegex);
      const lowerLetterArray = value.match(lowerCaseRegex);
      const numArray = value.match(numberRegex);
      const specialCharacterArray = value.match(specialCharacterRegex);
      if (capitalLetterArray?.length > 0) {
        this.isUpperCaseLetter = true;
      } else {
        this.isUpperCaseLetter = false;
      }
      if (lowerLetterArray?.length > 0) {
        this.isLowerCaseLetter = true;
      } else {
        this.isLowerCaseLetter = false;
      }
      if (numArray?.length > 0) {
        this.isNumber = true;
      } else {
        this.isNumber = false;
      }
      if (specialCharacterArray?.length > 0) {
        this.isSpecialCharacter = true;
      } else {
        this.isSpecialCharacter = false;
      }
      this.isMinLength = value.length >= 8; // New length check

    } else {
      this.isUpperCaseLetter = false;
      this.isNumber = false;
      this.isSpecialCharacter = false;
      this.isLowerCaseLetter = false;
      this.isMinLength = false; // Reset length check
    }
  }

  calculateDate() {
    const fiscalYearStart = moment(this.AltfiscalYearStart);
    if (!fiscalYearStart.isValid()) {
      console.log("Invalid Fiscal Year Start Date!");
      return;
    }

    const fiscalYearEnd = fiscalYearStart.clone().add(1, 'year');
    this.AltfiscalYearEnd = new Date(fiscalYearEnd.format('YYYY-MM-DD'));
  }

  otpConfig = {
    length: 4, // The length of the OTP (number of input fields)
    inputStyles: {
      allowNumbersOnly: true,
      isPasswordInput: false,
      disableAutoFocus: false,
      width: '3rem',
      height: '3rem',
      'border-radius': '8px',
      'margin-right': '1rem'
    }
  };

  save(form: NgForm): void {

    if (this.checked) {
      let bool = this.checkOTP();
      if (!bool) {
        return abp.message.error("Please Enter Correct OTP");
      }
    }
    if (form.invalid) {
      abp.message.error("please enter all the required fields")
      return;
    }

    this.saving = true;
    this.model.fiscalYearStart = moment(new Date(this.AltfiscalYearStart.getFullYear(), this.AltfiscalYearStart.getMonth(), this.AltfiscalYearStart.getDate(), this.AltfiscalYearStart.getHours(), this.AltfiscalYearStart.getMinutes() - this.AltfiscalYearStart.getTimezoneOffset()).toISOString());
    this.model.fiscalYearEnd = moment(new Date(this.AltfiscalYearEnd.getFullYear(), this.AltfiscalYearEnd.getMonth(), this.AltfiscalYearEnd.getDate(), this.AltfiscalYearEnd.getHours(), this.AltfiscalYearEnd.getMinutes() - this.AltfiscalYearEnd.getTimezoneOffset()).toISOString());
    abp.auth.clearToken();
    /*  abp.utils.setCookieValue(
       AppConsts.authorization.encryptedAuthTokenName,
       undefined,
       undefined,
       abp.appPath
     ); */
    abp.utils.deleteCookie("Abp.TenantId");
    //  this._cookieService.delete("Abp.TenantId");
    //this.model.tenancyName = this.model.name.replace(/[^a-zA-Z ]/g, "");
    // this.model.tenancyName = this.model.tenancyName.replace(" ", "");
    this._tenatRegistration.registerTenant(this.model).subscribe((result: RegisterTenantoutputDto) => {
      // if (!result.canLogin) {
      //   this.notify.success(this.l('SuccessfullyRegistered'));
      //  
      //   return;
      // }

      // Autheticate
      this.message.success(
        this.l("Please have a look in Inbox/Junk/Spam folder of your inbox."),
        this.l("Verification Email sent")


      );
      this._router.navigate(['account/login']);

      // this.authService.authenticateModel.userNameOrEmailAddress = this.model.userName;
      // this.authService.authenticateModel.password = this.model.password;
      // this.authService.authenticate(() => {
      //   this.saving = false;
      // });
    });
  }




  // OTP COde

  enableTwoWayAuthentication(event) {
    if (event && event.checked == true) {
      this.checked = true;
      this.model.isTwoFactorEnabled = true;
      /*      this.countryPhoneCode = this.countriesCodes.find(
        (x) => x.label == "+1 (US)"
      ); */
    } else {
      this.checked = false;
      this.model.isTwoFactorEnabled = false;
      this.model.adminPhoneNumber = null;
    }
  }

  getuserIP() {
    this.countriesCodes = csc.Country.getAllCountries().map((item) => {
      return {
        value: item.phonecode,
        label:
          (item.phonecode.includes("+") ? "" : "+") +
          item.phonecode +
          ` (${item.isoCode})`,
      };
    });
    this.httpClient.get("https://ipinfo.io?token=035137ff516903").subscribe(
      (value: any) => {
        if (value) {
          let country = csc.Country.getCountryByCode(value.country);
          let formatCountry = {
            value: country.phonecode,
            label:
              (country.phonecode.includes("+") ? "" : "+") +
              country.phonecode +
              ` (${country.isoCode})`,
          };
          this.countryPhoneCode = this.countriesCodes.find(
            (x) => x.value == formatCountry.value
          );
        }
      }
    );
  }
  checkOTP(value?) {
    if (this.otp)
      if (this.otp.length == 4) {
        if (this.otp == this.otpResult) {
          this.isotpMatch = true;

          return true;
        }
      }

    return false;
  }
  onConfigChange() {
    this.showOtpComponent = false;
    this.otp = null;
    setTimeout(() => {
      this.showOtpComponent = true;
    }, 0);
  }

  sendOTPCode() {
    if (this.phoneNumber && this.countryPhoneCode) {
      let phoneNo = "+" + this.countryPhoneCode.value + this.phoneNumber;
      this.model.adminPhoneNumber = phoneNo;
      this.isLoading = true;
      this._accountService
        .sendOTP(phoneNo)
        .pipe(
          finalize(() => {
            this.isLoading = false;
            this.saving = false;
          })
        )
        .subscribe((r) => {
          this.otpResult = r;
          this.notify.info("OTP Send Successfully");
          console.log(this.otpResult);
        });
    }
  }


  onSubmit() { }

}
