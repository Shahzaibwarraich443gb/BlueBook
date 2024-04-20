import { NgModule } from "@angular/core";
import { RouterModule } from "@angular/router";
import { ChartOfAccountComponent } from "./chart-of-account.component";
import { CreateChartOfAccountComponent } from "./create-chart-of-account/create-chart-of-account.component";

@NgModule({
  imports: [
    RouterModule.forChild([
      {
        path: "",
        component: ChartOfAccountComponent,
      }
    ]),
  ],
  exports: [RouterModule],
})
export class ChartOfAccountRoutingModule {}
