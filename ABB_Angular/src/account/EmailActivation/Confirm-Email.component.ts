import { Component, ElementRef, Injector, OnInit, QueryList, ViewChildren, ViewEncapsulation } from '@angular/core';
import { AbpSessionService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/app-component-base';
import { accountModuleAnimation } from '@shared/animations/routerTransition';
import { AppAuthService } from '@shared/auth/app-auth.service';
import { AccountServiceProxy, ActivateEmailInput, IsTenantAvailableInput, RegisterInput, RegisterOutput, RegisterTenantInputDto, RegisterTenantoutputDto, ResolveTenantIdInput, TenantRegistationServiceProxy } from '@shared/service-proxies/service-proxies';

import { ActivatedRoute, Router } from '@angular/router';
import { finalize } from 'rxjs';
import { NgForm } from '@angular/forms';
@Component({

  template: `
  <div class="page-wrapper p-t-180 p-b-100 font-robo">
    <div class="card-body">
      <div class="kt-login__form">
        <div class="alert alert-success text-center" role="alert">
          <div class="alert-text">{{ waitMessage }}</div>
        </div>
      </div>
    </div>
  </div>
`,
})

export class ConfirmEmailComponent extends AppComponentBase {
  waitMessage: string;


  model: ActivateEmailInput = new ActivateEmailInput();


  constructor(
    injector: Injector,
    public authService: AppAuthService,
    private _sessionService: AbpSessionService,
    private _accountService: AccountServiceProxy,
    private _router: Router,
    private _activatedRoute: ActivatedRoute,
    // private _cookieService: CookieService,
    public _tenatRegistration: TenantRegistationServiceProxy


  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.waitMessage = "Please wait while confirming your email address...";
    this.model.c = this._activatedRoute.snapshot.queryParams["c"];

    this._accountService
      .resolveTenantId(new ResolveTenantIdInput({ c: this.model.c }))
      .subscribe((tenantId) => {
        //
        let reloadNeeded = this.appSession.changeTenantIfNeeded(tenantId);

        if (reloadNeeded) {
          return;
        }

        this._accountService.activateEmail(this.model).subscribe(() => {
          this.notify.success(this.l("Your Email is Confirmed"),
            //this._router.navigate(["account/login"]);
            "",
            {
              onClose: () => {
                this._router.navigate(["account/login"]);
              },
            });
        });
      });


  }
  parseTenantId(tenantIdAsStr?: string): number {
    let tenantId = !tenantIdAsStr ? undefined : parseInt(tenantIdAsStr, 10);
    if (Number.isNaN(tenantId)) {
      tenantId = undefined;
    }

    return tenantId;
  }




















}
