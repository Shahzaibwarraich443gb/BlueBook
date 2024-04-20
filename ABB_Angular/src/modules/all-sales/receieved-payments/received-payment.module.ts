
import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { MatTooltipModule } from "@angular/material/tooltip";
// import {ReceivedPaymentsComponent} from './received-payments.component';
import { NgxPrintElementModule } from 'ngx-print-element';

@NgModule({
  imports: [
    CommonModule,
    MatTooltipModule,
    NgxPrintElementModule
  ],
  declarations: [
    // ReceivedPaymentsComponent
  ],
})
export class ReceivedPaymentModule {}
