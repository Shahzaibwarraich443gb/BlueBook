import { DEFAULT_CURRENCY_CODE, Injectable } from "@angular/core";
@Injectable({
  providedIn: "root",
})
export class ValidationService {

  constructor() {}

  letterOnlyWithSpaceAllowed(event): Boolean { 
    if (event.target.selectionStart === 0 && event.code === "Space") {
      event.preventDefault();
    } else {
      const charCode = event.which ? event.which : event.keyCode;
      if (charCode == 32) {
        return true;
      } else if (
        (charCode < 65 || charCode > 90) &&
        (charCode < 97 || charCode > 122)
      ) {
        return false;
      }
      return true;
    }
  }
  numberOnlyWith(event): boolean {
    const charCode = event.which ? event.which : event.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
      return false;
    }
    return true;
  } 
  allowDecimalNumber(event: any) {
    const reg = /^-?\d*(\.\d{0,5})?$/;
  let input = event.target.value + String.fromCharCode(event.charCode);
  if (!reg.test(input)) {
    event.preventDefault();
  }
  }

  firstSpaceNotAllowed(event) {
    if (event.target.selectionStart === 0 && event.code === "Space") {
      event.preventDefault();
    }
  }
  
}
