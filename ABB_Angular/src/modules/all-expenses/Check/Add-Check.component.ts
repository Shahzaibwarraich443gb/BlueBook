import { Component, Injector } from "@angular/core";
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { AppComponentBase } from "@shared/app-component-base";
import { Bank, BankServiceProxy, ChartOfAccountsServiceProxy, CheckAccountDetail, CheckDto, CheckProductDetail, CheckServiceProxy, Customer, CustomerServiceProxy, ProductServiceServiceProxy } from "@shared/service-proxies/service-proxies";
import * as moment from "moment";
import { NgxSpinnerService } from "ngx-spinner";

@Component({
    selector: 'app-add-Check',
    templateUrl: 'Add-Check.component.html',
    styleUrls: ['Add-Check.component.scss']
})

export class AddCheckComponent extends AppComponentBase {

    payeeArr: any[] = [];
    customerArr: any[] = [];
    bankArr: any[] = [];
    coaArr: any[] = [];
    productArr: any[] = [];
    checkFooterArr: any[] = [];
    showError: boolean = false;
    accountForm: FormGroup;
    productForm: FormGroup;
    checkForms: FormGroup;
    checkDateAlt: any = new Date();
    total: number = 0.00;
    checkId: number = 0;
    notes: string = null;
    payeeId: number = 0;
    bankId: number = 0;
    bankBalance: any = "";


    constructor(
        private injector: Injector,
        private customerService: CustomerServiceProxy,
        private bankService: BankServiceProxy,
        private chartOfAccountService: ChartOfAccountsServiceProxy,
        private productService: ProductServiceServiceProxy,
        private fb: FormBuilder,
        private checkService: CheckServiceProxy,
        private spinner: NgxSpinnerService,
        private router: Router,
        private activateRoute: ActivatedRoute
    ) {
        super(injector);

        this.accountForm = new FormGroup({
            id: new FormControl(''),
            account: new FormControl('', [Validators.required, Validators.min(1)]),
            description: new FormControl(''),
            amount: new FormControl('', [Validators.required]),
            customer: new FormControl(0, [Validators.required, Validators.min(1)])
        });

        this.productForm = new FormGroup({
            id: new FormControl(''),
            product_service: new FormControl(0, [Validators.required, Validators.min(1)]),
            description: new FormControl(''),
            qty: new FormControl(1, [Validators.required]),
            rate: new FormControl(0, [Validators.required]),
            saleTax: new FormControl(''),
            amount: new FormControl(0, [Validators.required]),
            customer: new FormControl(0, [Validators.required, Validators.min(1)])
        });


        this.checkForms = this.fb.group({
            accountDetails: this.fb.array([this.accountForm]),
            productDetails: this.fb.array([this.productForm]),
        });
    }

    ngOnInit(): void {
        this.spinner.show();
        this.getCustomer();
        this.getPayee();
        this.getCheckFooterList();
        this.getBank();
        this.getChartOfAccount();
        this.getProducts();
    }

    getCheckData(id: number) {
        let checkDto = new CheckDto();
        checkDto.id = id;
        this.checkService.getCheckById(checkDto).subscribe((res) => {
            this.checkId = id;
            this.payeeId = +res.payeeId;
            this.bankId = res.bankId;
            this.checkDateAlt = res.creationTime.format('YYYY-MM-DD');

            if (res.checkAccountDetails.length > 0) {
                this.accountDetailsFormArray.removeAt(0);
            }
            if (res.checkProductDetails.length > 0) {
                this.productDetailsFormArray.removeAt(0);
            }

            for (let data of res.checkAccountDetails) {
                this.accountDetailsFormArray.push(new FormGroup({
                    id: new FormControl(data.id),
                    account: new FormControl(data.accountId, [Validators.required, Validators.min(1)]),
                    description: new FormControl(data.description),
                    amount: new FormControl(data.amount, [Validators.required, Validators.min(1)]),
                    customer: new FormControl(data.customerId, [Validators.required, Validators.min(1)])
                }));
            }

            for (let data of res.checkProductDetails) {
                this.productDetailsFormArray.push(new FormGroup({
                    id: new FormControl(data.id),
                    product_service: new FormControl(data.productId, [Validators.required, Validators.min(1)]),
                    description: new FormControl(data.description),
                    qty: new FormControl(data.quantity, [Validators.required]),
                    rate: new FormControl(data.rate, [Validators.required]),
                    saleTax: new FormControl(data.saleTax),
                    amount: new FormControl(data.amount, [Validators.required]),
                    customer: new FormControl(data.customerId, [Validators.required, Validators.min(1)])
                }));
            }

            this.calculateTotalAmount();

            this.spinner.hide();


        },
            ({ error }) => {
                this.notify.error("Cannot Get Data");
            });
    }

    getPayee(): void {
        this.checkService.getPayee().subscribe((res) => {
            this.payeeArr = res;
        },
        ({ error }) => {
            this.spinner.hide();
            this.notify.error("Cannot Get Payee");
        });
    }

    getCustomer(): void{
        this.customerService.getCustomersByTenantId().subscribe((res) => {
            this.customerArr = res;
        },
            ({ error }) => {
                this.spinner.hide();
                this.notify.error("Cannot retrieve payee");
            })
    }

    getBank(): void {
        this.bankService.getAll().subscribe(res => {
            this.bankArr = res;
        },
            ({ error }) => {
                this.spinner.hide();
                this.notify.error('Cannot Retreieve Banks');
            });
    }

    getChartOfAccount(): void {
        this.chartOfAccountService.getAll().subscribe((res) => {
            this.coaArr = res;
        },
            ({ error }) => {
                this.spinner.hide();
                this.notify.error("Cannot retreive account");
            })
    }

    getProducts(): void {
        this.productService.getAll().subscribe(res => {
            this.productArr = res;
            this.activateRoute.params.subscribe(params => {
                if (params.id) {
                    this.getCheckData(params.id);
                }
                else {
                    this.spinner.hide();
                }

            });
        },
            ({ error }) => {
                this.spinner.hide();
                this.notify.error("Cannot retreive products");
            })
    }

    get accountDetailsFormArray() {
        return this.checkForms.controls["accountDetails"] as FormArray;
    }

    get productDetailsFormArray() {
        return this.checkForms.controls["productDetails"] as FormArray;
    }



    addAccountDetails(): void {
        this.accountDetailsFormArray.push(new FormGroup({
            id: new FormControl(0),
            account: new FormControl(this.coaArr[0].id, [Validators.required]),
            description: new FormControl(''),
            amount: new FormControl('', [Validators.required]),
            customer: new FormControl(0, [Validators.required, Validators.min(1)])
        }));
    }

    addProductDetails(): void {
        this.productDetailsFormArray.push(new FormGroup({
            id: new FormControl(0),
            product_service: new FormControl(0, [Validators.required, Validators.min(1)]),
            description: new FormControl(''),
            qty: new FormControl(1, [Validators.required]),
            rate: new FormControl(0, [Validators.required]),
            saleTax: new FormControl(''),
            amount: new FormControl(0),
            customer: new FormControl(0, [Validators.required, Validators.min(1)])
        }));
    }

    getproductSaleTax(index: number): void {
        var productObj = this.productArr.find(x => x.id == this.productDetailsFormArray.controls[index].get('product_service').value);
        this.productDetailsFormArray.controls[index].get('saleTax').setValue(productObj.saleTax ? productObj.saleTax : 0);
        this.calculateTotalAmount();
    }

    clearAccountDetails(): void {
        if (this.accountDetailsFormArray.length == 1) {
            return;
        }

        this.checkForms.controls.accountDetails = this.fb.array([this.accountForm]);
    }


    getCheckFooterList(): void {
        this.checkService.getCheckFooter().subscribe(res => {
            this.checkFooterArr = res;
        },
            ({ error }) => {
                this.notify.error("Cannot Get Check Footer");
            });
    }


    clearProductDetails(): void {
        if (this.productDetailsFormArray.length == 1) {
            return;
        }

        this.checkForms.controls.productDetails = this.fb.array([this.productForm]);
    }

    removeAccountDetail(index: number): void {
        if (index == 0) {
            this.notify.error("Cannot remove the only row available");
            return;
        }
        this.accountDetailsFormArray.removeAt(index);
    }


    removeProductDetail(index: number): void {
        if (index == 0) {
            this.notify.error("Cannot remove the only row available");
            return;
        }
        this.productDetailsFormArray.removeAt(index);
    }

    calculateTotalAmount(): void {
        this.total = 0;
        for (var formControl of this.accountDetailsFormArray.controls) {
            this.total += parseFloat(formControl.get('amount').value ? formControl.get('amount').value : 0);
        }

        for (var formControl of this.productDetailsFormArray.controls) {
            formControl.get('amount').setValue(
                ((formControl.get('qty').value ?
                    formControl.get('qty').value : 1)
                    *
                    (
                        parseFloat(formControl.get('rate').value
                            ? formControl.get('rate').value : 0)
                        +

                        (

                            parseFloat(formControl.get('rate').value
                                ? formControl.get('rate').value : 0)
                            *
                            (parseFloat(formControl.get('saleTax').value
                                ? formControl.get('saleTax').value
                                : 0) / 100)
                        )
                    )).toFixed(2));

            this.total += parseFloat(formControl.get('amount').value ? formControl.get('amount').value : 0);
        }
    }


    onChangeBank(): void{
        this.bankBalance = this.bankArr.find(x => x.id == this.bankId).openBalance;
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




    saveCheck(doPrint: boolean): void {
        if (this.payeeId == 0 && this.bankId == 0) {
            return this.notify.error(this.l("please select payee and bank"));
        }

        if (this.payeeId == 0) {
            return this.notify.error(this.l("please select any payee"));
        }

        if (this.bankId == 0) {
            return this.notify.error(this.l("please select any bank"));
        }


        let checkDto = new CheckDto();
        checkDto.id = this.checkId;
        checkDto.payeeId = this.payeeId.toString();
        checkDto.bankId = this.bankId;
        checkDto.checkAccountDetails = [];
        checkDto.checkProductDetails = [];


        for (var formData of this.accountDetailsFormArray.controls) {

            if (formData.status != "VALID") {
                this.showError = true;
                this.notify.error("please fill the required fields");
                return;
            }

            let accountDetailObj = new CheckAccountDetail();
            accountDetailObj.accountId = formData.get('account').value;
            accountDetailObj.description = formData.get('description').value;
            accountDetailObj.amount = formData.get('amount').value;
            accountDetailObj.customerId = formData.get('customer').value;
            checkDto.checkAccountDetails.push(accountDetailObj);
        }


        for (var formData of this.productDetailsFormArray.controls) {

            if (formData.status != "VALID") {
                this.showError = true;
                this.notify.error("please fill the required fields");
                return;
            }

            let productDetailObj = new CheckProductDetail();
            productDetailObj.productId = formData.get('product_service').value;
            productDetailObj.description = formData.get('description').value;
            productDetailObj.quantity = formData.get('qty').value;
            productDetailObj.rate = formData.get('rate').value;
            productDetailObj.saleTax = formData.get('saleTax').value;
            productDetailObj.amount = formData.get('amount').value;
            productDetailObj.customerId = formData.get('customer').value;
            checkDto.checkProductDetails.push(productDetailObj);
        }

        checkDto.totalAmount = this.total;

        checkDto.notes = this.notes;


        this.checkService.addCheck(checkDto).subscribe((res) => {
            this.notify.success("Check Saved");
            this.router.navigate(['/app/all-expenses/check'])



            if (doPrint) {
                var checkSetupObj = this.checkService.getCheckSetup().subscribe((checkSetupObj) => {
                    for (var data of this.checkFooterArr) {
                        switch (data.value) {
                            case 1:
                                data.checkVal = res.id.toString().padStart(8, '0');
                                break;
                            case 2:
                                data.checkVal = '021212121212';
                                break;
                            case 3:
                                data.checkVal = 'A12121213131212';
                                break;
                        }
                    }
                    let bankObj = this.bankService.getAll().subscribe(bankRes => {
                        let styles = `
                        <style>
                        @media print{
                            @page {
                                padding: 0;
                                margin: 0;
                            }
                          }
                        </style>
                        `
                        let printData = `
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
                                                    <br><b id="bank_name">${bankRes.find(x => x.id == checkSetupObj.bankId).bankName ?? " "}</b>
                                                    <br><b id="bank_address">${bankRes.find(x => x.id == checkSetupObj.bankId).address ?? " "}</b>
     
                                                </p>
                                            </td>
                                            <td>
                                                <p style="margin:0px 10px 0 0;padding-left: 160px;    font-size: medium;"
                                                   align="right">CHK-${res.id.toString().padStart(8, '0')}</p>
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
                                                               
                                                                &nbsp; ${this.customerArr.filter(x => checkDto.checkAccountDetails.some(y => y.customerId == x.id)).map(x => x.name).join(',')}
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
                                                                        id="S_Place">${this.checkFooterArr.find(x => x.value == checkSetupObj.secondFooter).checkVal}</td>
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
                    });
                });
            }
        });

    }


}