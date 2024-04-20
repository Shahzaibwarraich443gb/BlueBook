import { Component, Injector, ViewEncapsulation } from '@angular/core';
import { AbpSessionService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/app-component-base';
import { accountModuleAnimation } from '@shared/animations/routerTransition';
import { AppAuthService } from '@shared/auth/app-auth.service';
import { AccountServiceProxy, IsTenantAvailableInput } from '@shared/service-proxies/service-proxies';
import { AppTenantAvailabilityState } from '@shared/AppEnums';
import { Router } from '@angular/router';
import { CookieService } from 'ngx-cookie-service';

@Component({
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class LoginComponent extends AppComponentBase {

  hide = true;
  submitting = false;

  constructor(
    injector: Injector,
    public authService: AppAuthService,
    private _sessionService: AbpSessionService,
    private _accountService: AccountServiceProxy,
    private _router: Router,
    private cookieService: CookieService,


  ) {
    super(injector);
  }

  get multiTenancySideIsTeanant(): boolean {
    return this._sessionService.tenantId > 0;
  }

  get isSelfRegistrationAllowed(): boolean {
    if (!this._sessionService.tenantId) {
      return false;
    }

    return true;
  }


  async login() {
    this.submitting = true;
    let body = new IsTenantAvailableInput();
    body.emailAddress = this.authService.authenticateModel.userNameOrEmailAddress;
    let result = await this._accountService.isTenantAvailable(body).toPromise();

    if (result.state == AppTenantAvailabilityState.Available) {
      abp.multiTenancy.setTenantIdCookie(result.tenantId);
    }
    else if (result.state == AppTenantAvailabilityState.NotFound) {
      //to do: install to remove cookie from browser by using below line
      this.cookieService.delete("Abp.TenantId");
    }

    else if (result.state == AppTenantAvailabilityState.InActive) {
      this.message.warn(this.l("Your tenant is not active  "));
      this.submitting = false;
      if (result.isPaidTenant) {
        this._router.navigate(['account/package-extend/stripe', result.tenantId]);
      }
      return;
    }

    this.authService.authenticate(() => (this.submitting = false));
  }
}
