import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CustomerComponent } from './customer.component';
import { ViewCustomerComponent } from './view-customer/view-customer.component'; 
import { CustomerDetailComponent } from './view-customer/customer-detail/customer-detail.component';

@NgModule({
    imports: [
        RouterModule.forChild([
           {
               path: '',
               component: CustomerComponent,
           },
        ])
    ],
    exports: [RouterModule]
})
export class CustomerRoutingModule { }
