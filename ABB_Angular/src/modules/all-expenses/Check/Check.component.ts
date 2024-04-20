import { Component, Injector, ViewChild } from "@angular/core";
import { MatPaginator } from "@angular/material/paginator";
import { MatSort, Sort } from "@angular/material/sort";
import { MatTableDataSource } from "@angular/material/table";
import { AppComponentBase } from "@shared/app-component-base";
import { BankServiceProxy, CheckDto, CheckServiceProxy, CustomerServiceProxy } from "@shared/service-proxies/service-proxies";
import * as moment from "moment";
import * as Papa from 'papaparse';
import { saveAs } from 'file-saver';
import { NgxSpinnerService } from "ngx-spinner";

@Component({
  selector: 'app-Check',
  templateUrl: 'Check.component.html',
  styleUrls: ['Check.component.scss']
})

export class CheckComponent extends AppComponentBase {


  checkArr: any[] = [];
  bankArr: any[] = [];
  payeeArr: any[] = [];
  checkFooterArr: any[] = [];
  selectedBank: number = 0;
  checkDS: any;

  showDeleted: boolean = false;

  columns: string[] = ["cBox", "date", "payee", "type", "checkNo", "bankName", "amount", "memo", "action"];

  @ViewChild(MatSort) sort: MatSort;
  @ViewChild('checkPaginator') checkPaginator: MatPaginator;

  customers: any[] = [];
  

  selectedChecks: number[] = [];

  constructor(
    private injector: Injector,
    private checkService: CheckServiceProxy,
    private spinner: NgxSpinnerService,
    private bankService: BankServiceProxy,
    private customerService: CustomerServiceProxy
  ) {
    super(injector);

  }


  ngOnInit(): void {
    this.spinner.show();
    this.getCheckFooterList();
    this.getCustomers();
    this.getPayee();
    this.getBank();
    this.getCheck();
  }

  getCustomers(): void {
    this.customerService.getCustomersByTenantId().subscribe((res) => {
      this.customers = res;
    },
      ({ error }) => {
        this.notify.error("cannot retrieve payee");
      })
  }

  getPayee(): void{
    this.checkService.getPayee().subscribe((res)=>{
      this.payeeArr = res;
      this.spinner.hide();
    },
    ({error})=>{
      this.spinner.hide();
    })
  }

  showPayeeName(payeeId: string): string{
    let payee = this.payeeArr.find(x => x.id == payeeId);
    return payee ? payee.payeeName : "";
  }

  showPayeeType(payeeId: string): string{
    let payee  = this.payeeArr.find(x => x.id == payeeId);
    return payee ? payee.payeeType : ""
  }


  getCheckFooterList(): void {
    this.checkService.getCheckFooter().subscribe(res => {
      this.checkFooterArr = res;
    },
      ({ error }) => {
        this.notify.error("cannot Get Check Footer");
      });
  }


  downloadExcel(): void{
    this.spinner.show();

    let dataArr = [];

    for(var data of this.checkArr.reverse()){
      let obj = {
        Date: moment(new Date(data.creationTime)).format('MM/DD/YYYY'),
        Payee: this.payeeArr.find(x => x.id == data.payeeId).payeeName,
        Type: 'Customer',
        CheckNo: data.id.toString().padStart(8, '0'),
        BankName: data.bank.bankName,
        Amount: data.totalAmount.toFixed(2),
        Memo: this.payeeArr.find(x => x.id == data.payeeId).payeeName

      }

      dataArr.push(obj);
    }

    const csv = Papa.unparse(dataArr, {
      header: true,
    });

    this.spinner.hide();    
    const blob = new Blob([csv], { type: 'text/csv;charset=utf-8' });
    saveAs(blob, 'Checks.csv');

}


  getBank(): void {
    this.bankService.getAll().subscribe(res => {
      this.bankArr = res;
    },
      ({ error }) => {
        this.notify.error('cannot Retreieve Banks');
      });
  }

  onCboxChange(id: number): void {
    if (!this.selectedChecks.some(x => x == id)) {
      this.selectedChecks.push(id);
    }
    else {
      this.selectedChecks = this.selectedChecks.filter(x => x != id);
    }
  }

  numberToWords(number: number): string {
    const ones = [
      '', 'one', 'two', 'three', 'four', 'five', 'six', 'seven', 'eight', 'nine',
      'ten', 'eleven', 'twelve', 'thirteen', 'fourteen', 'fifteen', 'sixteen', 'seventeen', 'eighteen', 'nineteen'
    ];

    const tens = ['', '', 'twenty', 'thirty', 'forty', 'fifty', 'sixty', 'seventy', 'eighty', 'ninety'];

    if (number === 0) {
      return 'zero';
    } else if (number < 20) {
      return ones[number];
    } else if (number < 100) {
      return tens[Math.floor(number / 10)] + (number % 10 !== 0 ? ' ' + ones[number % 10] : '');
    } else if (number < 1000) {
      return ones[Math.floor(number / 100)] + ' hundred' + (number % 100 !== 0 ? ' and ' + this.numberToWords(number % 100) : '');
    } else {
      return 'Number out of range';
    }
  }

  convertAmountToWords(amount: string): string {
    const [dollars, cents] = amount.replace(/[^\d.]/g, '').split('.');
    const dollarsInWords = this.numberToWords(parseInt(dollars));
    const centsInWords = this.numberToWords(parseInt(cents) || 0);

    let result = dollarsInWords + ' dollar';
    if (dollars !== '1') {
      result += 's';
    }

    if (centsInWords !== 'zero') {
      result += ' and ' + centsInWords + ' cent';
      if (cents !== '1') {
        result += 's';
      }
    }

    return result;
  }

  printCheck(): void {
    this.spinner.show();
    if (this.selectedChecks.length == 0) {
      this.notify.error("please select any check to print");
      this.spinner.hide();
      return;
    }

    let printData = '';

    let styles = `
      <style>
      @media print{
          @page {
              padding: 0;
              margin: 0;
          }
        }
      </style>
      `;


    this.checkService.getCheckSetup().subscribe((checkSetupObj) => {
      if (checkSetupObj.checkStyle.toLowerCase() == "voucher" && this.selectedChecks.length > 1) {
        this.spinner.hide();
        this.notify.error("only one check can be print in voucher mode. change mode in check setup or select only one");
        return;
      }

      for (var i=0; i<this.selectedChecks.length; i++) {

        let checkDto = this.checkArr.find(x => x.id == this.selectedChecks[i]);


        for (var checkFooter of this.checkFooterArr) {
          switch (checkFooter.value) {
            case 1:
              checkFooter.checkVal = checkDto.id.toString().padStart(8, '0');
              break;
            case 2:
              checkFooter.checkVal = '123123123123';
              break;
            case 3:
              checkFooter.checkVal = '123123123123';
              break;
          }
        }

        printData += `
        ${
          
          i > 0 ? "<div style='margin-top: 1rem'> </div>" : ""
        }
    <table class="print-layout checkDesignTbl"
    style="border:1px solid #00000a;width:100%;height:220px;margin-bottom: 20px;" cellspacing="0"
    cellpadding="0" align="left">
    <tbody>
        <tr>
            <td width="5px"></td>
            <td width="570">
                <table style="width:100%" align="left">
                    <tbody>
                        <tr>
                            <td width="15px">&nbsp;</td>
                            <td width="190px">
                                <p style="float:left;font-size:12px;vertical-align:top">
                                    <b id="banknames"></b>
                                    <br><b id="company_name">${checkSetupObj.companyName}</b>
                                    <br><b id="company_add_line1">${checkSetupObj.addressLine1 ?? " "}</b>
                                    <br><b id="company_add_line2">${checkSetupObj.addressLine2 ?? " "}</b>
                                    <br><b id="company_add_line3">${checkSetupObj.addressLine3 ?? " "}</b>
                                </p>
                            </td>
                            <td width="130px">
                                <p style="float:left;font-size:12px;vertical-align:top">
                                    <b id="banknames"></b>
                                    <br><b id="bank_name">${this.bankArr.find(x => x.id == checkSetupObj.bankId).bankName ?? " "}</b>
                                    <br><b id="bank_address">${this.bankArr.find(x => x.id == checkSetupObj.bankId).address ?? " "}</b>

                                </p>
                            </td>
                            <td>
                                <p style="margin:0px 10px 0 0;padding-left: 160px;    font-size: medium;"
                                   align="right">CHK-${checkDto.id.toString().padStart(8, '0')}</p>
                                <p style="margin:0px 10px 0 0;    font-size: medium;" 
                                    align="right"><b>Date:</b>&nbsp;${moment(new Date()).format('MM/DD/YYYY')}</p>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </td>
        </tr>
        <tr>
            <td width="5px"></td>
            <td align="center">
                <table width="98%" cellspacing="0" cellpadding="0" align="left">
                    <tbody>
                        <tr>
                            <td style="border-bottom: 1px solid black;" width="30%" height="20px"> <b
                                    style=" line-height: 15px;font-size: smaller;    font-weight: 700;">Pay
                                    to the Order of</b>&nbsp;${this.payeeArr.find(x => x.id == checkDto.payeeId).payeeName} </td>
                            <td style="border-bottom: 1px solid;    width: 520px;" width="50%">
                                <p style="margin: 0px;    font-size: medium;">&nbsp;
                                </p>
                            </td>
                            <td style="font-size: x-large;" width="2%"> <b>$</b> </td>
                            <td style="border:1px solid #00000a">
                                <p style="margin: 0px;     width: 110px;    font-size: medium; padding: 0.3rem"
                                    >${checkDto.totalAmount}</p>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" style="border-bottom:1px solid #00000a" height="30px">
                                <p id="checkamountwords" style="margin: 0px;    font-size: medium;">&nbsp; 
                            
                               <span style='margin-left: 10%'> ${this.convertAmountToWords('$' + checkDto.totalAmount)} only </span></p>
                            </td>
                            <td>
                                <p>&nbsp;</p>
                            </td>
                            <td> </td>
                        </tr>
                        <tr>
                            <td colspan="4" style="padding-top:3px"></td>
                        </tr>
                        <tr>
                            <td colspan="2" valign="bottom" height="30px">
                                <table style="padding:0px" width="100%" height="30px" cellspacing="0"
                                    cellpadding="0" align="left">
                                    <tbody>
                                        <tr>
                                            <td width="7%"> <b>Memo</b> </td>
                                            <td style="border-bottom:1px solid #00000a">
                                                <p style="    font-size: medium;">
                                               
                                                &nbsp; ${this.customers.filter(x => checkDto.checkAccountDetails.some(y => y.customerId == x.id)).map(x => x.name).join(',')}
                                                </p>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </td>
                            <td>
                                <p>&nbsp;</p>
                            </td>
                            <td style="border-bottom:1px solid #00000a">
                                <p>&nbsp;</p>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="4" height="30px">
                                <table style="padding:0px" width="100%" height="30px" cellspacing="0"
                                    cellpadding="0" align="left">
                                    <tbody>
                                        <tr id="checkfooter">

                                            <td width="28%"> <span
                                                    style="margin-left:8px;font-size:medium;"
                                                    class="amounts" id="banknos"><span
                                                        id="F_Place">${this.checkFooterArr.find(x => x.value == checkSetupObj.firstFooter).checkVal}</td>
                                            <td width="28%"> <span
                                                    style="margin-left:8px;font-size:medium;"
                                                    class="amounts" id="banknos">C<span
                                                        id="S_Place">${this.checkFooterArr.find(x => x.value == checkSetupObj.secondFooter).checkVal}</span>C</span> </td>
                                            <td> <span style="margin-left:10px;font-size: x-large;"
                                                    id="routingnos" class="amounts"></span> <span
                                                    style="margin-left: 8px;font-size:medium;"
                                                    class="amounts"><span
                                                        id="T_Place">${this.checkFooterArr.find(x => x.value == checkSetupObj.thirdFooter).checkVal}</span></span> </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </td>
        </tr>
    </tbody>
</table>
    `

        if (i == this.selectedChecks.length - 1) {

          let WindowPrt: any = window.open('', '_blank');
          WindowPrt.document.write(styles);
          WindowPrt.document.write(printData);
          WindowPrt.document.title = 'Check';
          setTimeout(() => {
            WindowPrt.document.close();
            WindowPrt.focus();
            WindowPrt.print();
            WindowPrt.close();
          }, 700);
        }

      }


      this.spinner.hide();

    },
      ({ error }) => {
        this.spinner.hide();
        this.notify.error("cannot print check");
      });

  }


  onBankChange(): void {
    this.spinner.show();
    this.getCheck();
  }

  getCheck(): void {
    this.checkService.getCheck(this.showDeleted, this.selectedBank).subscribe((res) => {
      this.checkArr = res;
      this.selectedChecks = [];
      for (var data of this.checkArr) {
        data.checkNo = data.id.toString().padStart(8, '0');
      }
      this.checkDS = new MatTableDataSource<any>(this.checkArr.reverse());
      this.checkDS.paginator = this.checkPaginator;
      this.spinner.hide();
    },
      ({ error }) => {
        this.spinner.hide();
        this.notify.error("cannot retrieve check")
      })
  }

  deleteCheck(id: number) {
    let checkDto = new CheckDto();
    checkDto.id = id;
    this.checkService.deleteCheck(checkDto).subscribe((res) => {
      this.notify.success("check Deleted");
      this.getCheck();
    },
      ({ error }) => {
        this.notify.error("cannot delete check");
      })
  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.checkDS.filter = filterValue.trim().toLowerCase();
  }

  sortCheckData(sort: Sort): void {
    const data = this.checkArr.slice();
    if (!sort.active || sort.direction === 'asc') {
      this.checkDS = data.sort((a, b) => a.id - b.id);
      return;
    }
    else if (!sort.active || sort.direction === "desc") {

      this.checkDS = data.sort((a, b) => b.id - a.id);
      return
    }
  }

  getCheckIncludingDeleted(isChecked): void {
    this.showDeleted = isChecked;
    this.getCheck();
  }

}