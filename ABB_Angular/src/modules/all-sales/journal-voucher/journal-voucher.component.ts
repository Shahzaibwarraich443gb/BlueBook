import { Component, Injector, OnInit, ViewChild, } from '@angular/core';
import { ChartOfAccountDto, ChartOfAccountsServiceProxy, CustomerServiceProxy, InvoiceDetailDto, InvoiceServiceProxy, JournalVoucherServiceServiceProxy, ProductServiceDto, ProductServiceServiceProxy, PurchaseInvoiceDto, SaveJournalVouchers, VenderServiceProxy, VoucherDetailDto } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs';
import * as moment from 'moment';
import { AppComponentBase } from '@shared/app-component-base';
import { ActivatedRoute } from '@angular/router';
import { PurchaseInvoiceAccountDto } from '../../../shared/service-proxies/service-proxies';
import { JournalVoucherListComponent } from '../journal-voucher-list/journal-voucher-list.component';

@Component({
  selector: 'app-journal-voucher',
  templateUrl: 'journal-voucher.component.html',
  styleUrls: ['journal-voucher.component.scss'],
})

export class JournalVoucherComponent extends AppComponentBase implements OnInit {
  isActive: boolean = false;
  showJournalVoucherList: boolean = true;
  constructor(
    private injector: Injector,
    private _vendor: VenderServiceProxy,
    public _customerServiceProxy: CustomerServiceProxy,
    public _productServiceServiceProxy: ProductServiceServiceProxy,
    public _chartOfAccoountService: ChartOfAccountsServiceProxy,
    public _journalVoucherService: JournalVoucherServiceServiceProxy,
    private _activatedRoute: ActivatedRoute
  ) {
    super(injector);
  }

  @ViewChild('JournalVoucherListComponent', { static: false }) JournalVoucherList: JournalVoucherListComponent;

  selectedProduct: any;
  totalSalePrice: number = 0;
  changeAmount: number = 0;
  invoiceId: any;
  isDisabled = false;
  voucherDto = new SaveJournalVouchers;
  DepositToList: ChartOfAccountDto[] = [];
  invoiceDueDateAlt: any;
  keyword: string;
  filterType: string = 'Name';
  customers: any;
  accountItems: any[] = [
    {
      account: 0,
      debit: 0,
      credit: 0,
      description: '',
      name: 0,
      isDebitEnabled: true, isCreditEnabled: true
    },
    {
      account: 0,
      debit: 0,
      credit: 0,
      description: '',
      name: 0,
      isDebitEnabled: true, isCreditEnabled: true
    }
  ];

  ngOnInit() {
    this.customerList();
    this.getDepositToList();
    this._activatedRoute.params.subscribe(parms => {
      if (parms.id) {
        this.invoiceId = +parms.id;

      } else {
        //this.voucherDto.invoiceDate = 0;
      }
    });
    this.invoiceDueDateAlt = new Date();
  }

  getDepositToList() {
    this._chartOfAccoountService.getChartOfAccountsForJV().pipe(finalize(() => { }))
      .subscribe((result) => {
        this.DepositToList = result;
      });
  }
  customerList() {
    this._customerServiceProxy.getAll(this.keyword, this.filterType, 0, 10).pipe(
      finalize(() => { }))
      .subscribe((result) => {
        this.customers = result.items;
      });
  }

  // For Account Table 
  addNewRow(item: any, index: number, event: any): void {
    if (index === this.accountItems.length - 1 && item.account !== '' && item.account !== undefined) {
      this.accountItems.push(
        {
          account: 0,
          description: '',
          debit: 0,
          credit: 0,
          name: 0,
          isDebitEnabled: true, isCreditEnabled: true
        },
        {
          account: 0,
          description: '',
          debit: 0,
          credit: 0,
          name: 0,
          isDebitEnabled: true, isCreditEnabled: true
        }
      );
    } else if (event === "edit") {
      this.accountItems.push({
        account: item.refChartOfAccountId,
        description: item.description,
        debit: +item.debit,
        credit: +item.credit,
        name: 0
      });
      //this.changeTotalAmonut("item", item.amount, index);
    }
  }

  changeTotalAmonut(type: string, event: number, index: number) {
    this.accountItems[index].amount = event;
    this.changeAmount = this.accountItems.reduce((sum, item) => {
      return sum + item.amount;
    }, 0);
  }

  accountRemoveRow(item: any, i: number) {
    this.accountItems.splice(i - 1, 2);
  }

  onDebitChange(row: any, i: number) {
    if (row.debit !== 0) {
      row.isCreditEnabled = false;
      this.accountItems[i].credit = 0;
      if (i % 2 === 0) {
        this.accountItems[i + 1].isDebitEnabled = false;
      } else {
        this.accountItems[i - 1].isDebitEnabled = false;
      }
    } else {
      row.isCreditEnabled = true;
      if (i % 2 === 0) {
        this.accountItems[i + 1].isDebitEnabled = true;
      } else {
        this.accountItems[i - 1].isDebitEnabled = true;
      }
    }
  }


  onCreditChange(row: any, i: number) {
    if (row.credit !== 0) {
      row.isDebitEnabled = false;
      this.accountItems[i].debit = 0;
      if (i % 2 === 0) {
        this.accountItems[i + 1].isCreditEnabled = false;
      } else {
        this.accountItems[i - 1].isCreditEnabled = false;
      }
    } else {
      row.isDebitEnabled = true;
      if (i % 2 === 0) {
        this.accountItems[i + 1].isCreditEnabled = true;
      } else {
        this.accountItems[i - 1].isCreditEnabled = true;
      }
    }
  }

  // deleteRows() {
  //   this.accountItems.splice(this.accountItems.length - 2, 2);
  // }

  save() {
    this.isActive = true;
    if (!this.invoiceDueDateAlt) {
      this.isActive = false;
      return this.notify.error(this.l("please select any date"));
    }

    if (!this.accountItems[0].account) {
      this.isActive = false;
      return this.notify.error(this.l("please select any account"));
    }

    let isValid = true;
    let hasEmptyField = false;
    for (let i = 0; i < this.accountItems.length; i += 2) {
      const debitValue = this.accountItems[i].debit;
      const creditValue = this.accountItems[i + 1].credit;

      const debitValue_1 = this.accountItems[i].credit;
      const creditValue_1 = this.accountItems[i + 1].debit;

      const row1 = this.accountItems[i].account;
      const row2 = this.accountItems[i + 1].account;
      if (this.accountItems[i].account !== 0) {
        if (row1 === 0 || row2 === 0) {
          this.isActive = false;
          return this.notify.error(this.l("Select minimum two accounts."));
        }
        if ((debitValue !== creditValue)) {
          isValid = false;
          break;
        } else if (debitValue_1 !== creditValue_1) {
          isValid = false;
          break;
        }
        if (debitValue === 0 && debitValue_1 === 0) {
          hasEmptyField = true;
        } else if (creditValue === 0 && creditValue_1 === 0) {
          hasEmptyField = true;
        }
      }
    }

    if (!isValid) {
      this.isActive = false;
      return this.notify.error(this.l("Debit value in a row must be equal to the Credit value in the following row."));
    } else if (hasEmptyField) {
      this.isActive = false;
      return this.notify.error(this.l("Both Debit and Credit values must be entered for all rows."));
    }

    if (this.invoiceId) {
      this.voucherDto.id = this.invoiceId;
    }

    let invoiceDateAlt = this.invoiceDueDateAlt;
    let data = [];
    this.accountItems.forEach((item, index) => {
      let obj = new VoucherDetailDto();
      if (item.account !== 0 && item.debit !== item.credit) {
        obj.chartOfAccountID = item.account;
        obj.description = item.description;
        obj.dAmount = item.debit;
        obj.cAmount = item.credit;
        obj.refCustomerId = item.name;
        data.push(obj);
      } else {
      }
    });
    this.voucherDto.voucherDetails = data;
    // console.log("VourcherList:", data);
    this.isActive = false;
    this.voucherDto.invoiceDate = moment(new Date(invoiceDateAlt.getFullYear(), invoiceDateAlt.getMonth(), invoiceDateAlt.getDate(), invoiceDateAlt.getHours(), invoiceDateAlt.getMinutes() - invoiceDateAlt.getTimezoneOffset()).toISOString());
    this._journalVoucherService.saveJournalVoucher(this.voucherDto).subscribe((res) => {
      if (res === "success") {

        let invoiceList = [];

        for (var data of this.voucherDto.voucherDetails) {
          let obj = new InvoiceDetailDto();
          obj.amount = data.cAmount;
          obj.paidAmount = data.dAmount;
          obj.refChartOfAccountID = data.chartOfAccountID;
          invoiceList.push(obj);
        }

        this._chartOfAccoountService.changeCoaBalance("JournalVoucher", 0, 0, invoiceList).subscribe((res) => {
        },
          ({ error }) => {
            this.notify.error("Cannot Put Impact On Chart Of Account");
            return;
          });

        if (!this.invoiceId) {
          this.clearForm();
          this.JournalVoucherList.getAllVouchers();
          this.notify.info(this.l("Saved Successfully"));
        } else {
          this.notify.info(this.l("Update Successfully"));
        }
        this.isActive = false;
      }
    })
  }

  clearForm() {
    this.accountItems = [];
    this.accountItems = [
      { account: 0, debit: 0, credit: 0, description: '', name: 0, isDebitEnabled: true, isCreditEnabled: true },
      { account: 0, debit: 0, credit: 0, description: '', name: 0, isDebitEnabled: true, isCreditEnabled: true }
    ];
  }

}
