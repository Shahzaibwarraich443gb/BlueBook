import { NgModule } from '@angular/core';



import { CommonModule } from '@angular/common';
import { ThemeModules } from './../../mat_component_modules/theme.modules';

import { RecurringinvoiceComponent } from './Recurringinvoice.component';
import { RecurringinvoiceRoutingModule } from '../Recurring-Invoice/Recurringinvoice.routing.module';
import {MatIconModule} from '@angular/material/icon';
import { CreditCardComponent } from 'modules/Recurring-Invoice/credit-card/credit-card.component';
@NgModule({
    imports: [
        CommonModule,
        ThemeModules,
        MatIconModule,
        RecurringinvoiceRoutingModule,
    ],
    exports: [],
    declarations: [RecurringinvoiceComponent,CreditCardComponent],
    providers: [],
})
export class RecurringInvoicesModule { }
