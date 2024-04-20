import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { AccountComponent } from './account.component';
import { SignupComponent } from './Sign-Up/Sign-Up.component';
import { ConfirmEmailComponent } from './EmailActivation/Confirm-Email.component'


@NgModule({
    imports: [
        RouterModule.forChild([
            {
                path: '',
                component: AccountComponent,
                children: [
                    { path: 'login', component: LoginComponent },
                    { path: 'signup', component: SignupComponent },
                    { path: 'register', component: RegisterComponent },
                    {
                        path: "confirm-email",
                        component: ConfirmEmailComponent,

                    },
                ]
            }
        ])
    ],
    exports: [
        RouterModule
    ]
})
export class AccountRoutingModule { }
