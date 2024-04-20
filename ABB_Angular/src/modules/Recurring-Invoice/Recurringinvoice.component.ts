import { Component, ElementRef, EventEmitter, Injector, Input, OnInit, Output, ViewChild } from '@angular/core';
import { MatChipInputEvent } from '@angular/material/chips';
import { ChargeCardDto, CompanyCardDto, ContactInfoDto, CreateInvoiceDto, CustomerDto, CustomerServiceProxy, Int64EntityDto, Invoice, InvoiceDetailDto, InvoiceDto, InvoiceServiceProxy, ProductServiceDto, ProductServiceServiceProxy, ReceivedPaymentServiceServiceProxy, ReceviedPayment, RecurringInvoice, RecurringInvoiceDto, RecurringInvoiceServiceProxy, SaveReceivedPayment, } from '@shared/service-proxies/service-proxies';
import { ENTER, COMMA, I, SPACE } from '@angular/cdk/keycodes';
import { finalize } from 'rxjs';
import * as moment from 'moment';
import { NotifyService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/app-component-base';
import { setTimeout } from 'timers/promises';
import { ActivatedRoute, Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { CreditCardComponent } from 'modules/Recurring-Invoice/credit-card/credit-card.component';
import { EntityDto } from '@shared/paged-listing-component-base';


@Component({
  selector: 'app-Recurringinvoice',
  templateUrl: 'Recurringinvoice.component.html',
  styleUrls: ['Recurringinvoice.component.scss'],

})

export class RecurringinvoiceComponent extends AppComponentBase implements OnInit {
  constructor(
    private injector: Injector,
    public _customerServiceProxy: CustomerServiceProxy,
    public _productServiceServiceProxy: ProductServiceServiceProxy,
    private _invoiceServiceProxy: InvoiceServiceProxy,
    private _recurringInvoiceServiceProxy: RecurringInvoiceServiceProxy,
    private _receivedPaymentServiceProxy: ReceivedPaymentServiceServiceProxy,
    private _activatedRoute: ActivatedRoute,
    public _dialog: MatDialog,
    private _router: Router
  ) {
    super(injector);

  }
  selectedDuration
  //selectedPaymentMethod:string
  selectedPaymentFrequency
  keyword: string
  filterType: string = 'Name';
  customers: CustomerDto[] = [];
  selectedProduct: any;
  productServiceDto: ProductServiceDto[] = [];
  // createInvoiceKey: CreateInvoiceDto;
  // CreateRecurringInvoiceKey:CreateRecurringInvoiceDto;
  monthlyFrequeny: any[] = [];
  weeklyFrequeny: any[] = [];
  signleCalculatedAmount: number = 0;
  invoiceDateAlt: any;
  invoiceDueDateAlt: any;
  customerId = 0;
  companycarddto: CompanyCardDto[] = [];
  createInvoice: CreateInvoiceDto = new CreateInvoiceDto();
  InvoiceDetail: InvoiceDetailDto[] = [];
  //@Input() invoice = new CreateRecurringInvoiceDto();


  createASetAmountOfTimesDuration;
  createUntilASpecificDuration;
  Annually;
  Monthly;
  Weekly;
  Custom;
  everyDayCount: any = null;
  Customer = new ContactInfoDto();
  ProductService = new ProductServiceDto();
  name: any;
  sendMail: boolean = true;
  addOnBlur = true;
  readonly separatorKeysCodes = [ENTER, COMMA, SPACE] as const;
  emailList: string[] = [];

  totalSalePrice: number = 0.00;
  items: ProductServiceDto[] = [];

  ngOnInit() {
    this.customerList();
    this.GetMonthlyFreqencyList();
    this.productServiceList();
    this._activatedRoute.params.subscribe(parms => {
      if (parms.id) {
        this.customerId = +parms.id;
      }
    });
    this.createInvoice = new CreateInvoiceDto();
    this.createInvoice.invoice = new InvoiceDto();
    this.createInvoice.invoice.invoiceDetails = [];
    let invoiceDtlObj: InvoiceDetailDto = new InvoiceDetailDto();
    this.createInvoice.invoice.invoiceDetails.push(invoiceDtlObj);

    console.log(this.createInvoice.invoice.invoiceDetails);
    this.createInvoice.invoice.refInvoiceType = 1;
    this.createInvoice.invoice.refTermID = 0;
    this.calculateDate();


  }



  initialization() {
    this.createInvoice.invoice.durationId = 1;
    this.createInvoice.invoice.frequencyId = 1;
    this.Monthly = this.monthlyFrequeny[0].key;
    this.Weekly = this.weeklyFrequeny[0].key;
    this.createInvoice.invoice.refPaymentTypeID = 1;
    this.createInvoice.invoice.refCardID = this.companycarddto.length > 0 ? this.companycarddto[0].id : null;
  }


  addEmail(event: MatChipInputEvent, bit: number): void {
    if (event && !undefined) {
      const value = event.value;
      if (value && bit === 0) {
        this.emailList.push(value);
      }
      else if (value && bit === 1) {
        for (let i = 0; i < this.emailList.length; i++) {
          this.emailList.splice(i, 1);
        }
        this.emailList.splice(0, 1);
        this.emailList.push(value);
      }
      if (event.chipInput) {
        event.chipInput!.clear();
      }
    }
  }
  onPaymentMethodChange() {
    if (this.createInvoice.invoice.refPaymentTypeID) {

      if (this.createInvoice.invoice.refPaymentTypeID == 1) {

      }

    }

  }


  GetMonthlyFreqencyList() {
    this._recurringInvoiceServiceProxy.getMonthlyFrequency().subscribe((res) => {

      this.monthlyFrequeny = res;

      this.GetWeeklyFreqencyList();
    });
  }

  GetWeeklyFreqencyList() {
    this._recurringInvoiceServiceProxy.getWeeklyFrequency().subscribe((res) => {

      this.weeklyFrequeny = res;
      this.initialization();
    });
  }


  removeEmail(item: any): void {
    const index = this.emailList.indexOf(item);
    if (index >= 0) {
      this.emailList.splice(index, 1);
    }
  }
  // Table
  addRowOnProductSelect(item: any, index: number, event: any): void {

    if (!this.createInvoice.invoice.invoiceDetails[index].quantity) {
      this.createInvoice.invoice.invoiceDetails[index].quantity = 1;
    }

    if (!this.createInvoice.invoice.invoiceDetails[index + 1]) {
      this.createInvoice.invoice.invoiceDetails.push(new InvoiceDetailDto());
    }

    if (this.createInvoice.invoice.invoiceDetails[index].refProducID) {
      let prodServiceObj = this.productServiceDto.find(x => x.id == this.createInvoice.invoice.invoiceDetails[index].refProducID);
      const rate = prodServiceObj.salePrice;
      const saleTaxPercentage = parseFloat(prodServiceObj.saleTax);

      const quantity = this.createInvoice.invoice.invoiceDetails[index].quantity;
      const amount = (rate * quantity) + ((rate * saleTaxPercentage / 100) * quantity);

      this.createInvoice.invoice.invoiceDetails[index].rate = rate;
      this.createInvoice.invoice.invoiceDetails[index].saleTax = saleTaxPercentage;
      this.createInvoice.invoice.invoiceDetails[index].amount = amount;
    }

    this.totalSalePrice = 0;

    for (var data of this.createInvoice.invoice.invoiceDetails) {
      this.totalSalePrice += data.amount ?? 0;
    }



    // const selectList = this.productServiceDto.filter((obj) => {
    //   return obj.id === item?.name;
    // });
    // this.selectedProduct = selectList;

    // if (selectList.length > 0) {
    //   const mappedItems = selectList.map((selectItem) => {
    //     return {
    //       name: +selectItem.id,
    //       description: '',
    //       quantity: 1,
    //       salePrice: selectItem.salePrice,
    //       saleTax: +selectItem.saleTax,
    //       discount: 0,
    //       amount: 0
    //     };
    //   });
    //   this.items.splice(index, 1, ...mappedItems);

    //   let data = [];
    //   for (let i = 0; i < mappedItems.length; i++) {
    //     let obj = new InvoiceDetailDto();
    //     obj.refProducID = mappedItems[i].name;
    //     obj.quantity = mappedItems[i].quantity;
    //     obj.discount = mappedItems[i].discount;
    //     obj.saleTax = mappedItems[i].saleTax;
    //     obj.amount = mappedItems[i].amount;
    //     obj.rate = mappedItems[i].salePrice;
    //     data.push(obj);
    //   }
    //   this.createInvoice.invoice.invoiceDetails.splice(index, 1, ...data);
    // }

    // if (index === this.items.length - 1 && item.name !== '') {
    //   this.items.push({
    //     name: '',
    //     description: '',
    //     quantity: 1,
    //     salePrice: 0,
    //     saleTax: 0,
    //     discount: 0,
    //     amount: 0
    //   });
    // }

    // this.calculateTotalAmount(index);
  }

  changeValues(name: any, value: any, i: number) {
    if (this.createInvoice.invoice.invoiceDetails.length > 0) {
      if (name === "saleprice") {
        this.createInvoice.invoice.invoiceDetails[i].rate = value;
      } else if (name === "description") {
        this.createInvoice.invoice.invoiceDetails[i].description = value;
      } else if (name === "quantity") {
        this.createInvoice.invoice.invoiceDetails[i].quantity = value;
      } else if (name === "discount") {
        this.createInvoice.invoice.invoiceDetails[i].discount = value;
      } else if (name === "amount") {
        this.createInvoice.invoice.invoiceDetails[i].amount = value;
      }
      this.calculateTotalAmount(i);
    }
  }

  removeRow(item: any, i: number) {
    if (+item.name) {
      this.totalSalePrice -= this.createInvoice.invoice.invoiceDetails[i].amount;
      this.items.splice(i, 1);
      this.createInvoice.invoice.invoiceDetails.splice(i, 1);
    } else {
      this.items.splice(i, 1);
    }
  }

  public openCreditCardDialog(id?: number): void {
    if (this.createInvoice.invoice.refCustomerID) {


      const dialogRef = this._dialog.open(CreditCardComponent, {
        data: { customerId: this.createInvoice.invoice.refCustomerID },
      });
      dialogRef.afterClosed().subscribe((result: CompanyCardDto) => {
        // if (result) { this.saleReceipt.chargeCard = result; }
        this.GetCardInformation();
        dialogRef.close();
      });
    }
    else {
      abp.message.error("Please select customer first");
    }
  }
  GetCardInformation() {
    if (this.createInvoice.invoice.refCustomerID) {
      this._recurringInvoiceServiceProxy.getAllCards(this.createInvoice.invoice.refCustomerID).subscribe((res) => {
        this.companycarddto = res;


      });

    }
    else {
      abp.message.error("Please select cutomer first");
    }
  }



  calculateTotalAmount(i: any) {
    //if (this.createInvoice.invoice.invoiceDetails[i].saleTax && this.createInvoice.invoice.invoiceDetails[i].discount > 0) {
    // Calculate the subtotal
    const subtotal = this.createInvoice.invoice.invoiceDetails[i].quantity * this.createInvoice.invoice.invoiceDetails[i].rate;
    // Calculate the discount amount
    const discountAmount = subtotal * (this.createInvoice.invoice.invoiceDetails[i].discount / 100);
    // Calculate the subtotal after discount
    const subtotalAfterDiscount = subtotal - discountAmount;
    // Calculate the tax amount
    const taxAmount = subtotalAfterDiscount * (this.createInvoice.invoice.invoiceDetails[i].saleTax / 100);
    // Calculate the total amount
    const totalAmount = subtotalAfterDiscount + taxAmount;
    this.signleCalculatedAmount = totalAmount;
    // }
    //  else {
    //   this.signleCalculatedAmount = this.createInvoice.invoice.invoiceDetails[i].rate * this.createInvoice.invoice.invoiceDetails[i].quantity - (this.createInvoice.invoice.invoiceDetails[i].discount / 100) * (this.createInvoice.invoice.invoiceDetails[i].rate * this.createInvoice.invoice.invoiceDetails[i].quantity);
    // }
    this.createInvoice.invoice.invoiceDetails[i].amount = this.signleCalculatedAmount;
    const maxDigits = 2; // Maximum number of decimal places
    const value = this.signleCalculatedAmount; // Your value here
    //this.items[i].amount = value.toFixed(maxDigits);

    this.totalSalePrice = this.createInvoice.invoice.invoiceDetails.reduce((sum, item) => {
      return sum + item.amount;
    }, 0);
  }

  calculateDate() {
    const currentDate = moment();
    if (this.createInvoice.invoice.refTermID) {
      const terms = this.createInvoice.invoice.refTermID;
      const dueDate = moment(currentDate).add(terms, 'days');
      this.createUntilASpecificDuration = new Date(dueDate.format('YYYY-MM-DD'));

      this.Custom = new Date();
      this.Annually = new Date();
    } else {
      this.createUntilASpecificDuration = new Date();
      this.Custom = new Date();
      this.Annually = new Date();
    }
  }


  public deleteSelectedCard(): void {
    if (!this.createInvoice.invoice.refCustomerID) {

      return abp.message.error('Please Select Any Customer', 'Something Wrong!!');
    }

    if (!this.createInvoice.invoice.refPaymentTypeID
    ) {
      return abp.message.error("Please Select payment type");
    }
    if (!this.createInvoice.invoice.refCardID) {
      return abp.message.error("Please select card");
    }
    abp.message.confirm(
      this.l("Are you sure to delete  credit card"),
      undefined,
      (result: boolean) => {
        if (result) {
          this._recurringInvoiceServiceProxy
            .deleteCompanyCardByCustomerIdAndCardId(this.createInvoice.invoice.refCardID)
            .subscribe(() => {
              abp.notify.success(this.l("SuccessfullyDeleted"));
              this.createInvoice.invoice.refCardID = null;
              this.GetCardInformation();

            });
        }
      }
    );
  }



  selectCustomerEmail(customerId: any) {
    const customer = this.customers.find((obj) => obj.id === customerId);
    if (customer) {
      this.addEmail({ value: customer.email } as MatChipInputEvent, 1);
    }
    this.GetCardInformation();
  }

  // Get data lists
  customerList() {
    this._customerServiceProxy.getAll(this.keyword, this.filterType, 0, 10).pipe(
      finalize(() => { }))
      .subscribe((result) => {
        this.customers = result.items;
        if (this.customerId) {
          const Datalist = this.customers.find((obj) => obj.id === this.customerId);
          if (Datalist) {
            this.createInvoice.invoice.refCustomerID = Datalist.id;
            this.addEmail({ value: Datalist.email } as MatChipInputEvent, 1);
          }
        }
      });
  }

  productServiceList() {

    this._productServiceServiceProxy.getAll().subscribe((result) => {
      this.productServiceDto = result
    });
  }

  // Save  
  save(type: any) {
    if (!this.createInvoice.invoice.refCustomerID) {
      // this.notify.info(this.l("Please Select Customer"));
      return abp.message.error('Please Select Any Customer', 'Something Wrong!!');
    }



    this.createInvoice.invoice.total = this.totalSalePrice;
    this.createInvoice.invoice.refInvoiceType = 8;
    //let specificDuration=this.createUntilASpecificDuration;
    this.createInvoice.invoice.email = this.emailList;
    //c-here
    //this.createInvoice.invoice.invoiceDetails = this.createInvoice.invoice.invoiceDetails;

    // for(var item of this.items){
    //   let InvoiceDetailDto:InvoiceDetailDto;

    //   InvoiceDetailDto.amount = item.amount;
    //   InvoiceDetailDto.discount = item.discount;
    //   InvoiceDetailDto.quantity = item.quantity;
    //   InvoiceDetailDto.rate = item.rate;
    //   InvoiceDetailDto.refProducID = item.refProducID;
    //   InvoiceDetailDto.saleTax = item.saleTax;  
    //   InvoiceDetailDto.refCustomerID = this.customerId;
    //   InvoiceDetailDto.paidAmount = item.paidAmount;
    //   InvoiceDetailDto.description = item.descripition;
    //   InvoiceDetailDto.isPaid = item.isPaid;

    // }



    if (this.createInvoice.invoice.durationId != null) {
      //assing duration to invoice Obj
      if (this.createInvoice.invoice.durationId == 1) {

        this.createInvoice.invoice.duration = null;

        //When i Selected Duration id 1 
        this.createUntilASpecificDuration = null;
        this.createASetAmountOfTimesDuration = null;






      }
      else if (this.createInvoice.invoice.durationId == 2) {
        this.createInvoice.invoice.duration = this.createASetAmountOfTimesDuration;

        this.createUntilASpecificDuration = null;
      }


      else if (this.createInvoice.invoice.durationId == 3) {

        this.createInvoice.invoice.duration = this.createUntilASpecificDuration;
        this.createASetAmountOfTimesDuration = null;

      }
    }


    if (this.createInvoice.invoice.frequencyId != null) {
      if (this.createInvoice.invoice.frequencyId == 1) {


        this.createInvoice.invoice.frequency = this.Annually;

        this.Monthly = null;
        this.Weekly = null;
        this.Custom = null;
      }
      else if (this.createInvoice.invoice.frequencyId == 2) {


        this.createInvoice.invoice.frequency = this.Monthly

        this.Annually = null;
        this.Weekly = null;
        this.Custom = null
      }
      else if (this.createInvoice.invoice.frequencyId == 3) {

        this.createInvoice.invoice.frequency = this.Weekly;
        this.Monthly = null;
        this.Annually = null;
        this.Custom = null;
      }



      else if (this.createInvoice.invoice.frequencyId == 4) {

        this.createInvoice.invoice.frequency = this.Custom;
        this.Weekly = null;
        this.Monthly = null;
        this.Annually = null;
      }
    }

    let removalIndexArr = [];

    let i = 0;
    for (var data of this.createInvoice.invoice.invoiceDetails) {
      if (!data.refProducID || !data.quantity || !data.rate || data.rate == 0 || !data.amount || data.amount == 0) {
        removalIndexArr.push(i)
      }
      i++;
    }





    if (this.createInvoice.invoice.invoiceDetails.length == 0 || removalIndexArr.length == this.createInvoice.invoice.invoiceDetails.length) {
      return abp.message.error('Please Select At Least One Product!', 'Something Wrong!!');
    }

    if (!this.createInvoice.invoice.refCardID) {
      return abp.message.error('Please Select Credit Card Number!', 'Something Wrong!!');
    }

    if (removalIndexArr.length > 0) {
      for (let index of removalIndexArr) {
        this.createInvoice.invoice.invoiceDetails.splice(index, 1);
      }
    }


    let recurringInvoice: RecurringInvoiceDto = new RecurringInvoiceDto();

    recurringInvoice.customerId = this.createInvoice.invoice.refCustomerID;
    recurringInvoice.durationId = this.createInvoice.invoice.durationId;
    recurringInvoice.frequencyId = this.createInvoice.invoice.frequencyId;



    this._invoiceServiceProxy.saveInvoice(this.createInvoice).subscribe((result) => {

      let entityDto = new Int64EntityDto();
      entityDto.id = result;

      this._invoiceServiceProxy.invoiceByIdGet(entityDto).subscribe((res) => {

        let receivedPayment = new SaveReceivedPayment();

        receivedPayment.emails = [res.email];
        receivedPayment.paymentDate = moment(new Date());
        receivedPayment.refCustomerID = res.refCustomerId;
        receivedPayment.refInvoiceType = 5;
        receivedPayment.refPaymentMethodID = 1;
        receivedPayment.total = res.total;
        receivedPayment.invoiceNo = res.invoiceNo;
        receivedPayment.receivedPayments = [];

        console.log(res.invoiceDetails);

        for(var data of res.invoiceDetails){
          let recObj = new ReceviedPayment();
          recObj.invoiceID = res.id;
          recObj.invoiceNo = res.invoiceNo;
          recObj.isCheck = true;
          recObj.openBalance = res.total;
          recObj.paidAmount = res.total;
          recObj.invoiceDueDate = res.invoiceDueDate;

          receivedPayment.receivedPayments.push(recObj);
        }


        this._receivedPaymentServiceProxy.saveReceivedPayment(0, receivedPayment).subscribe((res) => {




          let recurringInvoice: RecurringInvoice = new RecurringInvoice();
          recurringInvoice.customerId = this.createInvoice.invoice.refCustomerID;
          recurringInvoice.durationId = this.createInvoice.invoice.durationId;
          recurringInvoice.frequencyId = this.createInvoice.invoice.frequencyId;

          switch (recurringInvoice.durationId) {
            case 2:
              if (!this.createASetAmountOfTimesDuration) {
                return abp.message.error('Please Add Duration Amount of Times!', 'Something Wrong!!');
              }
              recurringInvoice.durationAmount = this.createASetAmountOfTimesDuration;
              break;
            case 3:
              if (!this.createUntilASpecificDuration) {
                return abp.message.error('Please Select Duration Date!', 'Something Wrong!!');
              }
              recurringInvoice.durationDateTill = this.createUntilASpecificDuration;
              break;
          }

          switch (recurringInvoice.frequencyId) {
            case 1:
              if (!this.Annually) {
                return abp.message.error('Please Select Freuquency Annual Date!', 'Something Wrong!!');
              }
              recurringInvoice.frequencyAnnualDate = this.Annually;
              break;
            case 2:
              if (!this.Monthly) {
                return abp.message.error('Please Select Frequency Month!', 'Something Wrong!!');
              }
              recurringInvoice.frequencyMonth = this.Monthly;
              break;
            case 3:
              if (!this.Weekly) {
                return abp.message.error('Please Select Frequency Week!', 'Something Wrong!!');
              }
              recurringInvoice.frequencyWeek = this.Weekly;
              break;
            case 4:
              if (!this.Custom) {
                return abp.message.error('Please Select Frequency Custom Date!', 'Something Wrong!!');
              }
              recurringInvoice.frequencyCustomDate = this.Custom;
              break;
          }

          recurringInvoice.frequencyEveryDayCount = this.everyDayCount;

          recurringInvoice.invoiceData = JSON.stringify(this.createInvoice);

          recurringInvoice.sendMail = true;  //send mail always

          recurringInvoice.customerCardId = this.createInvoice.invoice.refCardID;


          this._recurringInvoiceServiceProxy.saveRecurringData(recurringInvoice).subscribe((res) => {
            this.notify.info(this.l("SavedSuccessfully"));
            this._router.navigate(['app', 'sales-transation']);
            localStorage.setItem('BreadCrumbData', JSON.stringify(['All Sales', 'Sales Transaction']))
            this.clearForm();


          },
            ({ error }) => {
              this.notify.error("Cannot Save Recurring Data");
              return;
            });
        });
      });

    });
    if (type === "print") {
      window.print();
    }
  }

  clearForm() {
    this.items = [];
    this.createInvoice.invoice.refTermID = 0;
    this.calculateDate();
    this.selectedProduct = null;
    this.emailList = [];
    this.createInvoice.invoice.email = null;

    this.totalSalePrice = 0;
    this.createInvoice.invoice.refCustomerID = null;
  }

}
