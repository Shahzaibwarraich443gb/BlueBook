import { Component, Injector, ViewEncapsulation } from '@angular/core';
import { Router } from '@angular/router';
import { finalize } from 'rxjs/operators';
import { AppComponentBase } from '@shared/app-component-base';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import {
  AccountServiceProxy,
  RegisterInput,
  RegisterOutput
} from '@shared/service-proxies/service-proxies';
import { accountModuleAnimation } from '@shared/animations/routerTransition';
import { AppAuthService } from '@shared/auth/app-auth.service';

@Component({
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss'],
  animations: [accountModuleAnimation()],
  encapsulation: ViewEncapsulation.None,
})
export class RegisterComponent extends AppComponentBase {
  model: RegisterInput = new RegisterInput();
  saving = false;
  hide = true;

  constructor(
    injector: Injector,
    private _accountService: AccountServiceProxy,
    private _router: Router,
    private authService: AppAuthService
  ) {
    super(injector);
  }

  save(): void {
    this.saving = true;

    this._accountService
      .register(this.model)
      .pipe(
        finalize(() => {
          this.saving = false;
        })
      )
      .subscribe((result: RegisterOutput) => {
        if (!result.canLogin) {
          this.notify.success(this.l('SuccessfullyRegistered'));
          this._router.navigate(['/login']);
          return;
        }

        // Autheticate
        this.saving = true;

        this.authService.authenticateModel.userNameOrEmailAddress = this.model.userName;
        this.authService.authenticateModel.password = this.model.password;
        this.authService.authenticate(() => {
          this.saving = false;
        });
      });
  }
}
