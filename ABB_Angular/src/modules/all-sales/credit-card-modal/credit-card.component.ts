
import { ValidationService } from '../../../shared/Services/validation.service';
import {
  Component,
  EventEmitter,
  Inject,
  Injector,
  OnInit,
  Output,
  ViewChild,
  ElementRef,
  HostListener
} from "@angular/core";
import {
  MatDialog,
  MatDialogRef,
  MAT_DIALOG_DATA,

} from "@angular/material/dialog";
import { appModuleAnimation } from "@shared/animations/routerTransition";
import { AppComponentBase } from "@shared/app-component-base";
import { SaveReceivedPayment, ReceivedPaymentServiceServiceProxy, ChargeCardDto } from '@shared/service-proxies/service-proxies';

export interface DialogData {
  id: number;
  totalAmount: number;
}

@Component({
  selector: "app-credit-card-modal",
  templateUrl: "./credit-card.component.html",
  styleUrls: ["./credit-card.component.scss"],
  animations: [appModuleAnimation()],
})
export class CreditCardComponent extends AppComponentBase implements OnInit {

  creditCard = new ChargeCardDto();
  totalAmount: number = 0;
  customerName: string;
  constructor(
    injector: Injector,
    public dialog: MatDialog,
    public dialogRef: MatDialogRef<ChargeCardDto>,
    private el: ElementRef,
    @Inject(MAT_DIALOG_DATA) public data: any,
    public validation: ValidationService
  ) {
    super(injector);
  }

  // this code is for MM/YYYY date Format

  // @HostListener('input', ['$event'])
  // onInput(event: any) {
  //   const input = event.target as HTMLInputElement;
  //   let value = input.value.trim().replace(/\s+/g, ''); // Remove any existing spaces
  //   if (value.length > 2) {
  //     value = value.slice(0, 2) + ' / ' + value.slice(2);
  //   }
  //   input.value = value;
  // }

  ngOnInit() {
    if (this.data.customerName) {
      this.creditCard.cardHolderName = this.data.customerName;
    }
    if (this.data.totalAmount) {
      this.totalAmount = this.data.totalAmount;
    }
  }

  save() {
    this.creditCard.cardType = this.getCardType(this.creditCard.cardNumber);
    if (this.creditCard.cardType != 'Unknown') {
      this.dialogRef.close(this.creditCard);
    } else {
      this.notify.error(this.l("Card Number Is Wrong"));
    }
  }

  hideDialog() {
    this.dialogRef.close();
  }

  numberOnly(event) {
    return this.validation.numberOnlyWith(event);
  }

  letterOnly(event) {
    return this.validation.letterOnlyWithSpaceAllowed(event);
  }

  firstSpaceNotAllowed(event) {
    this.validation.letterOnlyWithSpaceAllowed(event);
  }

  getCardType(cardNumber: string): string {
    if (/^4[0-9]{12}(?:[0-9]{3})?$/.test(cardNumber)) {
      return 'Visa';
    } else if (/^5[1-5][0-9]{14}$/.test(cardNumber)) {
      return 'Mastercard';
    } else if (/^3[47][0-9]{13}$/.test(cardNumber)) {
      return 'American Express';
    } else if (/^6(?:011|5[0-9]{2})[0-9]{12}$/.test(cardNumber)) {
      return 'Discover';
    } else if (/^3(?:0[0-5]|[68][0-9])[0-9]{11}$/.test(cardNumber)) {
      return 'Diners Club';
    } else if (/^(?:2131|1800|35\d{3})\d{11}$/.test(cardNumber)) {
      return 'JCB';
    } else {
      return 'Unknown';
    }
  }

}
