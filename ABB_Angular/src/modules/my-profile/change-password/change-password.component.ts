
import { Component, ChangeDetectorRef, Injector, OnInit } from "@angular/core";
import { NgForm } from "@angular/forms";
import { AppComponentBase } from "@shared/app-component-base";
import { AbpValidationError } from "@shared/components/validation/abp-validation.api";
import {
  AccountServiceProxy, ChangePasswordDto, UserServiceProxy,

} from "@shared/service-proxies/service-proxies";
import { finalize } from "rxjs/operators";

@Component({
  selector: "app-change-password",
  templateUrl: "./change-password.component.html",
  styleUrls: ["./change-password.component.scss"],
})
export class ChangePasswordComponent extends AppComponentBase {
  // dialog = false;
  model: any;
  changePasswordDto = new ChangePasswordDto();
  saving = false;
  ConfirmNewPassword
  show: boolean = false;
  isActive: boolean = false;
  confirmPasswordValidationErrors: Partial<AbpValidationError>[] = [
    {
      name: "validateEqual",
      localizationKey: "Passwords do not match",
    },
  ];


  constructor(
    injector: Injector,
    private _accountService: AccountServiceProxy,
    private _cdr: ChangeDetectorRef,
    private _userservice: UserServiceProxy

  ) {
    super(injector);
  }
  ngOnInit() {
    abp.event.on("onProfileInformationChanges", () => {
      this.disableForm();
    });
  }
  disableForm() {
    this.show = false;
  }
  ngAfterViewInit(): void {
    this._cdr.detectChanges();
  }

  showScreen(show: boolean) {
    this.show = show;
  }

  confirmPassword(event: string) {
    if (event !== this.changePasswordDto.newPassword) {
      this.isActive = true;
    } else {
      this.isActive = false;
    }

  }

  changePassword() {

    this._userservice
      .changePassword(this.changePasswordDto)
      .pipe(
        finalize(() => {

        })
      )
      .subscribe((success) => {
        if (success) {
          abp.message.success('Password changed successfully', 'Success');

        }
      });


  }
}
