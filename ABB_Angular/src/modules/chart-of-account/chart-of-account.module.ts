
import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { ChartOfAccountComponent } from "./chart-of-account.component";
import { ChartOfAccountRoutingModule } from "./chart-of-account-routing.module";
import { ThemeModules } from "./../../mat_component_modules/theme.modules";
import { CreateChartOfAccountComponent } from "./create-chart-of-account/create-chart-of-account.component";
import { CreateAccountTypeComponent } from "./create-account-type/create-account-type.component";
import { CreateMainHeadComponent } from "./create-main-head/create-main-head.component";
import { SharedModule } from "@shared/shared.module";
import { MatTooltipModule } from "@angular/material/tooltip";
import {NgxSpinnerModule} from 'ngx-spinner';
import { TableModule } from 'primeng/table';
import { TreeTableModule } from 'primeng/treetable';
import { ButtonModule } from 'primeng/button';
import { ToggleButtonModule } from 'primeng/togglebutton';

@NgModule({
  imports: [
    CommonModule,
    ThemeModules,
    ChartOfAccountRoutingModule,
    MatTooltipModule,
    NgxSpinnerModule,
    TableModule,
    TreeTableModule,
    ButtonModule,
    ToggleButtonModule
  ],
  declarations: [
    CreateMainHeadComponent,
    CreateAccountTypeComponent,
    CreateChartOfAccountComponent,
    ChartOfAccountComponent
  ],
})
export class ChartOfAccountModule {}
