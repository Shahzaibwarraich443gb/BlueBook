import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ThemeModules } from '../../mat_component_modules/theme.modules';

import {MatIconModule} from '@angular/material/icon';
import { PurchaseInvoiceComponent } from './Purchase-Invoice/Purchase-Invoice.component';
import { PurchasePaymentComponent} from './Purchase-Payment/purchase-payment.component';
import { PurchaseInvoiceRoutingModule } from './PurchaseInvoice.routing.module';
import {CheckComponent} from './Check/Check.component';
import {AddCheckComponent} from './Check/Add-Check.component';
import {CheckSetupComponent} from './Check/Check-Setup.component';


import { NgxPrintElementModule } from 'ngx-print-element';

@NgModule({
    imports: [
    CommonModule,
        ThemeModules,
        MatIconModule,
        PurchaseInvoiceRoutingModule,
        NgxPrintElementModule
    ],
    declarations: [
        PurchaseInvoiceComponent,
        PurchasePaymentComponent,
        CheckComponent,
        AddCheckComponent,
        CheckSetupComponent
    ]
})
export class PurchaseInvoicesModule { }
