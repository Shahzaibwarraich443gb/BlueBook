import { NgModule } from '@angular/core';

import { InvoicesComponent } from './invoice.component';
import { InvoiceRoutingModule } from './invoice.routing.module';
import { CommonModule } from '@angular/common';
import { ThemeModules } from './../../mat_component_modules/theme.modules';

// import { NgxPrintElementModule } from 'ngx-print-element';
import { NgxSpinnerModule } from 'ngx-spinner';
@NgModule({
    imports: [
        CommonModule,
        ThemeModules,
        InvoiceRoutingModule,
        // NgxPrintElementModule
    ],
    exports: [],
    declarations: [InvoicesComponent],
    providers: [],
})
export class InvoicesModule { }
