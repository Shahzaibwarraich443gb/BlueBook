import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { EstimateComponent } from './Estimate.component';

@NgModule({
    imports: [
        RouterModule.forChild([
           {
               path: '',
               component: EstimateComponent,
           },
        ])
    ],
    exports: [RouterModule]
})
export class EstimateRoutingModule { }
