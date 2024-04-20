import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';

import { RecurringinvoiceComponent } from './Recurringinvoice.component';

@NgModule({
    imports: [
        RouterModule.forChild([
           {
               path: '',
               component:RecurringinvoiceComponent,
           },
        ])
    ],
    exports: [RouterModule]
})
export class RecurringinvoiceRoutingModule { }
