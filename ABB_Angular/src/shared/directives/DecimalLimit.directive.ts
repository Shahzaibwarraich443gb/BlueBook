import { Directive, ElementRef, HostListener } from '@angular/core';

@Directive({
  selector: '[appDecimalLimit]'
})
export class DecimalLimitDirective {
  private decimalLimit = 3; // Change this value to the desired decimal limit

  constructor(private el: ElementRef) {}

  @HostListener('input', ['$event'])
  onInput(event: KeyboardEvent): void {
    const input = this.el.nativeElement as HTMLInputElement;
    const value = input.value;

    // Use a regular expression to enforce the decimal limit
    const regex = new RegExp(`^-?\\d*(\\.\\d{0,${this.decimalLimit}})?$`);
    if (!regex.test(value)) {
      // If the input doesn't match the desired format, update the input value
      input.value = value.slice(0, -1);
    }
  }

}