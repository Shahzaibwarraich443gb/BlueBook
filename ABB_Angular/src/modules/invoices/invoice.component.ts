import { Component, Injector, Input, OnInit } from '@angular/core';
import { MatChipInputEvent } from '@angular/material/chips';
import { ChartOfAccountsServiceProxy, ContactInfoDto, CreateInvoiceDto, CustomerDto, CustomerServiceProxy, AddGeneralLedgarInputDto, GeneralLedgerServiceProxy, InvoiceDetailDto, InvoiceDto, InvoiceServiceProxy, ProductServiceDto, ProductServiceServiceProxy, PurchaseInvoiceServiceServiceProxy, SavePurchaseInvoice } from '@shared/service-proxies/service-proxies';
import { ENTER, COMMA, SPACE } from '@angular/cdk/keycodes';
import { finalize } from 'rxjs';
import * as moment from 'moment';
import { AppComponentBase } from '@shared/app-component-base';
import { ActivatedRoute, Router } from '@angular/router';
// import { NgxPrintElementService } from 'ngx-print-element';
import { NgxSpinnerService } from 'ngx-spinner';

@Component({
  selector: 'app-invoice',
  templateUrl: 'invoice.component.html',
  styleUrls: ['invoice.component.scss'],
})

export class InvoicesComponent extends AppComponentBase implements OnInit {
  constructor(
    private injector: Injector,
    public _customerServiceProxy: CustomerServiceProxy,
    public _productServiceServiceProxy: ProductServiceServiceProxy,
    private _invoiceServiceProxy: InvoiceServiceProxy,
    private _activatedRoute: ActivatedRoute,
    private _chartOfAccountService: ChartOfAccountsServiceProxy,
    private spinner: NgxSpinnerService,
    private _purchaseInvoiceService: PurchaseInvoiceServiceServiceProxy,
    private router: Router,
    private _generalLedgerService: GeneralLedgerServiceProxy,
    // public print: NgxPrintElementService
  ) {
    super(injector);
    this.intialization();
  }

  public config = {
    printMode: 'template-popup', // template
    popupProperties: 'toolbar=yes,scrollbars=yes,resizable=yes,top=0,left=0,fullscreen=yes',
    pageTitle: 'Hello World',
    templateString: '<header>I\'m part of the template header</header>{{printBody}}<footer>I\'m part of the template footer</footer>',
    stylesheets: [{ rel: 'stylesheet', href: 'https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0/css/bootstrap.min.css' }],
    styles: ['td { border: 1px solid black; color: green; }', 'table { border: 1px solid black; color: red }', 'header, table, footer { margin: auto; text-align: center; }']
  }

  keyword: string
  filterType: string = 'Name';
  customers: CustomerDto[] = [];
  selectedProduct: any;
  productServiceDto: ProductServiceDto[] = [];
  createInvoiceKey: CreateInvoiceDto;
  signleCalculatedAmount: number = 0;
  invoiceDateAlt: any;
  invoiceDueDateAlt: any;
  previousAmount: number = 0;
  isEdit: boolean = false;
  isSendLater: boolean = false
  isDisabled = false;
  isActive = false;
  customerId = 0;
  Invoice = new InvoiceDto();
  InvoiceDetail: InvoiceDetailDto[] = [];
  @Input() invoice = new CreateInvoiceDto();

  Customer = new ContactInfoDto();
  ProductService = new ProductServiceDto();
  name: any;
  invoiceDetailId: number;
  addOnBlur = true;
  invoiceId: number;
  readonly separatorKeysCodes = [ENTER, COMMA, SPACE] as const;
  emailList: string[] = [];

  totalSalePrice: number = 0;
  items: any[] = [{
    name: '',
    description: '',
    quantity: 1,
    salePrice: 0,
    saleTax: 0,
    discount: 0,
    amount: 0
  }];

  async ngOnInit() {
    this.spinner.show();
    await this.customerList();
    await this.productServiceList();
    await this._activatedRoute.params.subscribe(parms => {
      if (parms.id) {
        this.isEdit = true;
        if (parms.id.includes("_")) {
          let customerId = parms.id.split("_")[1];
          this.Invoice.refCustomerID = parseInt(customerId);
        }
        else {
          this.invoiceId = +parms.id;
          this.getInvoiceDetails(this.invoiceId);
        }
      } else {
        this.Invoice.refInvoiceType = 1;
        this.Invoice.refTermID = 0;
      }
    });

    this.calculateDate();
  }

  intialization() {
    this.createInvoiceKey = new CreateInvoiceDto();
    this.invoice = new CreateInvoiceDto();
    this.createInvoiceKey.invoice = new InvoiceDto();
    this.createInvoiceKey.invoice.invoiceDetails = [];
  }

  getInvoiceDetails(invoiceId: number) {
    this._invoiceServiceProxy.getInvoiceDetails(invoiceId).pipe(
      finalize(() => { }))
      .subscribe((result) => {
        this.Invoice.refCustomerID = result[0].refCustomerId;
        this.Invoice.invoiceNo = result[0].invoiceNo;
        this.isDisabled = true;
        this.Invoice.refTermID = result[0].refTermId;
        this.calculateDate();
        this.Invoice.isSendLater = result[0].isSendLater;
        // this.addEmail({ value: result[0].customerEmail } as MatChipInputEvent, 1);
        this.Invoice.note = result[0].note;
        this.items.splice(0, 1);
        this.items = [];
        result.forEach((item, index) => {
          this.previousAmount += item.amount;
          this.addRowOnProductSelect(item, index, "edit");
        });
      });
  }

  // Email
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

  removeEmail(item: any): void {
    const index = this.emailList.indexOf(item);
    if (index >= 0) {
      this.emailList.splice(index, 1);
    }
  }
  // Table
  addRowOnProductSelect(item: any, index: number, event: any): void {
    const selectList = this.productServiceDto.filter((obj) => {
      return obj.id === item?.name;
    });
    this.selectedProduct = selectList;

    if (selectList.length > 0) {
      const mappedItems = selectList.map((selectItem) => {
        return {
          name: +selectItem.id,
          description: '',
          quantity: 1,
          salePrice: selectItem.salePrice,
          saleTax: +selectItem.saleTax,
          discount: 0,
          amount: 0
        };
      });
      this.items.splice(index, 1, ...mappedItems);

      //const isNow = this.InvoiceDetail.filter((obj) => obj.refProducID === item.name);
      let data = [];
      for (let i = 0; i < mappedItems.length; i++) {
        let obj = new InvoiceDetailDto();
        obj.refProducID = mappedItems[i].name;
        obj.quantity = mappedItems[i].quantity;
        obj.discount = mappedItems[i].discount;
        obj.saleTax = mappedItems[i].saleTax;
        obj.amount = mappedItems[i].amount;
        obj.rate = mappedItems[i].salePrice;
        obj.description = mappedItems[i].description;
        data.push(obj);
      }
      this.InvoiceDetail.splice(index, 1, ...data);
    }

    if (index === this.items.length - 1 && item.name !== '' && item.name !== undefined) {
      this.items.push({
        name: '',
        description: '',
        quantity: 1,
        salePrice: 0,
        saleTax: 0,
        discount: 0,
        amount: 0
      });
    }
    else if (event === "edit") {
      this.items.push({
        name: item.productId,
        description: item.description,
        quantity: item.quantity,
        salePrice: item.rate,
        saleTax: item.saleTax,
        discount: item.discount,
        amount: +item.amount
      });

      let data = [];
      let obj = new InvoiceDetailDto();
      obj.refProducID = item.productId;
      obj.quantity = item.quantity;
      obj.discount = item.discount;
      obj.saleTax = item.saleTax;
      obj.amount = item.amount;
      obj.rate = item.rate;
      obj.description = item.description;
      obj.invoiceDetailId = item.invoiceDetailId;
      data.push(obj);
      this.InvoiceDetail.splice(index, 1, ...data);
    }
    this.calculateTotalAmount(index);
  }

  changeValues(name: any, value: any, i: number) {
    if (this.InvoiceDetail.length > 0 && this.items[i].name !== '') {
      if (name === "saleprice") {
        this.InvoiceDetail[i].rate = value;
      } else if (name === "description") {
        this.InvoiceDetail[i].description = value;
      } else if (name === "quantity") {
        this.InvoiceDetail[i].quantity = value;
      } else if (name === "discount") {
        this.InvoiceDetail[i].discount = value;
      } else if (name === "amount") {
        this.InvoiceDetail[i].amount = value;
      }
      this.calculateTotalAmount(i);
    }
  }

  removeRow(item: any, i: number) {
    if (+item.name) {
      this.totalSalePrice -= this.InvoiceDetail[i].amount;
      this.items.splice(i, 1);
      if (this.invoiceId) { this.InvoiceDetail[i].isPaid = true; }
      else { this.InvoiceDetail.splice(i, 1); }
    } else {
      this.items.splice(i, 1);
    }
  }



  calculateTotalAmount(i: any) {
    //if (this.InvoiceDetail[i].saleTax && this.InvoiceDetail[i].discount > 0) {
    // Calculate the subtotal
    const subtotal = this.InvoiceDetail[i].quantity * this.InvoiceDetail[i].rate;
    // Calculate the discount amount
    const discountAmount = subtotal * (this.InvoiceDetail[i].discount / 100);
    // Calculate the subtotal after discount
    const subtotalAfterDiscount = subtotal - discountAmount;
    // Calculate the tax amount
    const taxAmount = subtotalAfterDiscount * (this.InvoiceDetail[i].saleTax / 100);
    // Calculate the total amount
    const totalAmount = subtotalAfterDiscount + taxAmount;
    this.signleCalculatedAmount = totalAmount;
    // }
    //  else {
    //   this.signleCalculatedAmount = this.InvoiceDetail[i].rate * this.InvoiceDetail[i].quantity - (this.InvoiceDetail[i].discount / 100) * (this.InvoiceDetail[i].rate * this.InvoiceDetail[i].quantity);
    // }
    this.InvoiceDetail[i].amount = this.signleCalculatedAmount;
    const maxDigits = 2; // Maximum number of decimal places
    const value = this.signleCalculatedAmount; // Your value here
    this.items[i].amount = value.toFixed(maxDigits);

    // this.totalSalePrice = this.InvoiceDetail.reduce((sum, item) => {
    //   return sum + item.amount;
    // }, 0);

    // this.totalSalePrice = 0.00;
    // for (var data of this.InvoiceDetail) {
    //   this.totalSalePrice += (data.rate + (data.rate * (data.saleTax ? data.saleTax / 100 : 1)))
    //   this.totalSalePrice -= (this.totalSalePrice * data.discount / 100)
    // }
    this.totalSalePrice = this.InvoiceDetail.reduce((sum, item) => {
      return sum + item.amount;
    }, 0);

    //this.totalSalePrice = +this.decimalPipe.transform(totalamount, '1.2-2');
  }

  calculateDate() {
    const currentDate = moment();
    if (this.Invoice.refTermID) {
      const terms = this.Invoice.refTermID;
      const dueDate = moment(currentDate).add(terms, 'days');
      this.invoiceDueDateAlt = new Date(dueDate.format('YYYY-MM-DD'));
      this.invoiceDateAlt = new Date();
    } else {
      this.invoiceDueDateAlt = new Date();
      this.invoiceDateAlt = new Date();
    }
  }


  selectCustomerEmail(customerId: any) {
    const customer = this.customers.find((obj) => obj.id === customerId);
    this.emailList = [];
    if (customer.email) {
      this.addEmail({ value: customer.email } as MatChipInputEvent, 1);
    }
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
            this.Invoice.refCustomerID = Datalist.id;
            this.addEmail({ value: Datalist.email } as MatChipInputEvent, 1);
          }
        }
        this.spinner.hide();
      });
  }

  productServiceList() {
    this._productServiceServiceProxy.getAllIncome().subscribe((result) => (
      this.productServiceDto = result
    ));
  }

  update() {
    abp.message.confirm(
      this.l("Are you sure want to update invoice"),
      undefined,
      (result: boolean) => {
        if (result) {
          this.save('save');
        }
      }
    );
  }

  // Save  
  save(type: any) {
    this.spinner.show();
    //this.spinner.show();
    this.isActive = true;
    if (!this.Invoice.refCustomerID) {
      this.spinner.hide();
      // this.notify.info(this.l("Please Select Customer"));
      this.isActive = false;
      return abp.message.error('Please Select Any Customer', 'Something Wrong!!');
    }
    else if (!this.selectedProduct) {
      this.spinner.hide();
      this.isActive = false;
      return abp.message.error('Please Select At Least One Product!', 'Something Wrong!!');
    }



    //saving invoice 

    let invoiceDateAlt = this.invoiceDateAlt;
    let invoiceDueDateAlt = this.invoiceDueDateAlt;
    let pAmount = 0;

    this.createInvoiceKey.invoice = this.Invoice;
    this.createInvoiceKey.invoice.total = this.totalSalePrice;
    this.createInvoiceKey.invoice.refInvoiceType = 1;
    this.createInvoiceKey.invoice.invoiceDate = moment(new Date(invoiceDateAlt.getFullYear(), invoiceDateAlt.getMonth(), invoiceDateAlt.getDate(), invoiceDateAlt.getHours(), invoiceDateAlt.getMinutes() - invoiceDateAlt.getTimezoneOffset()).toISOString());
    this.createInvoiceKey.invoice.invoiceDueDate = moment(new Date(invoiceDueDateAlt.getFullYear(), invoiceDueDateAlt.getMonth(), invoiceDueDateAlt.getDate(), invoiceDueDateAlt.getHours(), invoiceDueDateAlt.getMinutes() - invoiceDateAlt.getTimezoneOffset()).toISOString());
    this.createInvoiceKey.invoice.email = this.emailList;
    this.createInvoiceKey.invoice.invoiceDetails = this.InvoiceDetail;

    if (this.isEdit) {
      for (var data of this.InvoiceDetail) {
        pAmount += data.paidAmount;
      }
    }

    if (this.invoiceId) {
      this.createInvoiceKey.invoice.invoiceId = this.invoiceId;
    }


    this._invoiceServiceProxy.saveInvoice(this.createInvoiceKey).subscribe((result) => {




      //saving purchase invoice for invoices having products with automatic expense entry on
      let processedProductId = [];
      for (var data of this.InvoiceDetail) {

        if (processedProductId.indexOf(data.refProducID) == -1 && this.productServiceDto.find(x => x.id == data.refProducID).automaticExpense) {

          processedProductId.push(data.refProducID);

          let prodObj = this.productServiceDto.find(x => x.id == data.refProducID);
          let purchaseInvoiceObj = new SavePurchaseInvoice();
          purchaseInvoiceObj.vendorId = prodObj.vendorId;
          purchaseInvoiceObj.refNo = '0000';
          purchaseInvoiceObj.refTermID = 0;
          purchaseInvoiceObj.purchaseInvoiceAccount = [];

          let purchaseInvoiceList = this.InvoiceDetail.filter(x => x.refProducID == prodObj.id) as any[];


          for (var p of purchaseInvoiceList) {
            p.invoiceDetailID = result;
            p.refinvoiceID = null
          }

          purchaseInvoiceObj.purchaseInvoice = purchaseInvoiceList;

          this._purchaseInvoiceService.savePurchaseInvoice(purchaseInvoiceObj).subscribe((res) => {



            //hitting coa for products in invoice with regards of purchase invoice
            this._chartOfAccountService.changeCoaBalance("PurchaseInvoice", 0, 0, this.InvoiceDetail).subscribe((res) => { });
          },
            ({ error }) => {
              this.notify.error("Cannot Save purchase invoice");
            });

        }

      }

      //saving general ledgers for invoice
      let generalLederInput = new AddGeneralLedgarInputDto();
      generalLederInput.processType = "Invoice";
      generalLederInput.invoiceId = result;

      this._generalLedgerService.addLedger(generalLederInput).subscribe((res) => {

      },
        ({ error }) => {
          this.notify.error("Cannot Add Record In Ledger");
        });





      //hitting coa for products in invoice with regards of invoice
      this._chartOfAccountService.changeCoaBalance('Invoice', 0, this.previousAmount, this.createInvoiceKey.invoice.invoiceDetails).subscribe((res) => { });
      if (!this.invoiceId) {
        if (type === "print") {
          if (result) {
            const list = { id: result }
            const queryParams = list;
            var urlTree = this.router.createUrlTree(['/app/print-invoice/'], { queryParams });
            var url = this.router.serializeUrl(urlTree);
            window.open(url, '_blank');
          }
        }
        this.isActive = false;
        this.notify.info(this.l("Saved Successfully"));
        this.clearForm();
      } else {
        this.isActive = false;
        this.notify.info(this.l("Update Successfully"));
      }

      this.previousAmount = pAmount;
    });

    this.spinner.hide();

  }




  clearForm() {
    this.items = [];
    this.items.push({ name: '', description: '', quantity: 1, salePrice: 0, saleTax: 0, discount: 0, amount: 0 });
    this.Invoice.refTermID = 0;
    this.calculateDate();
    this.selectedProduct = null;
    this.emailList = [];
    this.Invoice.email = null;
    this.createInvoiceKey.invoice.isSendLater = false;
    this.createInvoiceKey.invoice.note = '';
    this.totalSalePrice = 0;
    this.Invoice.refCustomerID = null;
  }

}
