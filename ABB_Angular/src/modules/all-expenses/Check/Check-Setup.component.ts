import { Component, Injector } from "@angular/core";
import { Router } from "@angular/router";
import { AppComponentBase } from "@shared/app-component-base";
import { BankDto, BankServiceProxy, CheckServiceProxy, CheckSetupDto } from "@shared/service-proxies/service-proxies";
import { NgxSpinnerService } from "ngx-spinner";

@Component({
    selector: 'app-check-setup',
    templateUrl: 'Check-Setup.component.html',
    styleUrls: ['Check-Setup.component.scss']
})

export class CheckSetupComponent extends AppComponentBase {

    bankArr: any[] = [];
    checkFooterArr: any[] = [];
    footerMasterArr: any[] = [];
    footerValArr: any[] = [];
    checkSetupObj: CheckSetupDto = new CheckSetupDto();
    selectedBank: BankDto = new BankDto();

    constructor(
        private injector: Injector,
        private spinner: NgxSpinnerService,
        private bankService: BankServiceProxy,
        private checkService: CheckServiceProxy,
        private roouter:Router) {
        super(injector);
    }

    ngOnInit(): void {
        this.spinner.show();
        this.getBank();
        this.getCheckFooter();
    }

    getBank(): void {
        this.bankService.getAll().subscribe(res => {
            this.bankArr = res;
            this.selectedBank = res[0];
            this.getCheckSetupData();
        },
            ({ error }) => {
                this.spinner.hide();
                this.notify.error('Cannot Retreieve Banks');
            });
    }

    getCheckSetupData(): void {
        this.checkSetupObj = new CheckSetupDto();
        this.checkService.getCheckSetup().subscribe((res) => {
            this.checkSetupObj = res;
            this.selectedBank = this.bankArr.find(x => x.id == res.bankId);
            this.footerValArr = [this.checkSetupObj.firstFooter, this.checkSetupObj.secondFooter, this.checkSetupObj.thirdFooter];
            this.spinner.hide();
        },
            ({ error }) => {
                this.spinner.hide();
                this.notify.error("Cannot retrieve check setup");
            })
    }

    getCheckFooter(): void {
        this.checkService.getCheckFooter().subscribe((res) => {
            this.checkFooterArr = res;
            this.footerMasterArr = this.checkFooterArr.map(x => x.value);
        },
            ({ error }) => {
                this.notify.error("Cannot retrieve check footers");
            })
    }

    changeCheckFooter(footIndex: number): void {

        let val = this.footerValArr[footIndex];
        for (var i = 0; i < this.footerValArr.length; i++) {
            if (i != footIndex && this.footerValArr[i] == val) {
                this.footerValArr[i] = this.footerMasterArr.filter(x => !this.footerValArr.some(y => y == x)).pop();
            }
        }


    }

    saveCheckSetup(): void{
        this.checkSetupObj.firstFooter = this.footerValArr[0];
        this.checkSetupObj.secondFooter = this.footerValArr[1];
        this.checkSetupObj.thirdFooter = this.footerValArr[2];

        this.checkService.saveCheckSetup(this.checkSetupObj).subscribe((res)=>{
            this.notify.success("check setup added");
            this.roouter.navigate(['/app/all-expenses/check']);
        },
        ({error})=>{
            this.notify.error("cannot add check setup");
        })
    }

    onBankChange(): void{
        this.selectedBank = this.bankArr.find(x => x.id == this.checkSetupObj.bankId);
    }

}