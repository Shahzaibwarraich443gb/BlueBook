import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { InvoicesComponent } from './invoice.component';

@NgModule({
    imports: [
        RouterModule.forChild([
           {
               path: '',
               component: InvoicesComponent,
           },
        ])
    ],
    exports: [RouterModule]
})
export class InvoiceRoutingModule { }
