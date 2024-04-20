import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientJsonpModule } from '@angular/common/http';
import { HttpClientModule } from '@angular/common/http';
import { ModalModule } from 'ngx-bootstrap/modal';
import { AccountRoutingModule } from './account-routing.module';
import { ServiceProxyModule } from '@shared/service-proxies/service-proxy.module';
import { SharedModule } from '@shared/shared.module';
import { AccountComponent } from './account.component';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { AccountLanguagesComponent } from './layout/account-languages.component';
import { AccountHeaderComponent } from './layout/account-header.component';
import { AccountFooterComponent } from './layout/account-footer.component';
import { SignupComponent } from './Sign-Up/Sign-Up.component'
import { CookieService } from 'ngx-cookie-service';


// tenants
import { TenantChangeComponent } from './tenant/tenant-change.component';
import { TenantChangeDialogComponent } from './tenant/tenant-change-dialog.component';

//Material
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';

import { MatCheckboxModule } from '@angular/material/checkbox';
import { ConfirmEmailComponent } from './EmailActivation/Confirm-Email.component';
import { CheckboxModule } from "primeng/checkbox";
import { DropdownModule } from "primeng/dropdown";
import { NgOtpInputModule } from 'ng-otp-input';
@NgModule({
    imports: [
        CommonModule,
        NgOtpInputModule,
        FormsModule,
        HttpClientModule,
        MatDatepickerModule,
        MatNativeDateModule,
        HttpClientJsonpModule,
        SharedModule,
        ServiceProxyModule,
        AccountRoutingModule,
        MatIconModule,
        MatFormFieldModule,
        MatInputModule,
        CheckboxModule,

        MatCheckboxModule,
        DropdownModule,
        MatButtonModule,
        ModalModule.forChild(),
        MatSelectModule
    ],
    declarations: [
        AccountComponent,
        LoginComponent,
        RegisterComponent,
        AccountLanguagesComponent,
        AccountHeaderComponent,
        AccountFooterComponent,
        SignupComponent,
        ConfirmEmailComponent,

        // tenant
        TenantChangeComponent,
        TenantChangeDialogComponent,
    ]
})
export class AccountModule {

}
