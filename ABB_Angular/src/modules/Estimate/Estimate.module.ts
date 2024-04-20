import { NgModule } from '@angular/core';



import { CommonModule } from '@angular/common';
import { ThemeModules } from './../../mat_component_modules/theme.modules';
import { EstimateRoutingModule } from './Estimate.routing.module';
import { EstimateComponent } from './Estimate.component';

@NgModule({
    imports: [
        CommonModule,
        ThemeModules,
        EstimateRoutingModule
    ],
    exports: [],
    declarations: [EstimateComponent],
    providers: [],
})
export class EstimateModule { }
