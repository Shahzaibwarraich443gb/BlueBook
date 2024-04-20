import { Component, Injector, ViewChild } from "@angular/core"
import { MatPaginator } from "@angular/material/paginator";
import { MatTableDataSource } from "@angular/material/table";
import { AppComponentBase } from "@shared/app-component-base";
import { TrialBalanceDto, TrialBalanceInputDto, TrialBalanceServiceProxy } from "@shared/service-proxies/service-proxies";
import { extend } from "lodash-es";
import * as moment from "moment";

@Component({
    selector: 'app-trial-balance',
    templateUrl: './Trial-Balance.component.html',
    styleUrls: ['./Trial-Balance.component.scss']
})
export class TrialBalanceComponent extends AppComponentBase {

    @ViewChild('trialBalancePaginator') trialBalancePaginator: MatPaginator;

    balanceType: number = 1;
    startDate: Date = new Date(new Date().setMonth(new Date().getMonth() - 1));
    endDate: Date = new Date();
    startDateAlt: string = moment(this.startDate).format('MM/DD/YYYY');
    endDateAlt: string = moment(this.endDate).format('MM/DD/YYYY');
    trialBalanceArr: TrialBalanceDto[] = [];
    trialBalanceDS: any;
    trialBalanceColumns = ['AccountName', 'DebitAmount', 'CreditAmount', 'Balance'];

    constructor(private _injector: Injector,
                private trialBalanceService: TrialBalanceServiceProxy) {
        super(_injector);
    }

    ngOnInit(): void{
        this.getTrialBalanceData();
    }

    getTrialBalanceData(): void{
        let key = new TrialBalanceInputDto();
        key.startDate = moment(this.startDate);
        key.endDate = moment(this.endDate);
        this.trialBalanceService.getTrialBalance(key).subscribe((res)=>{
            this.trialBalanceArr = res;
            this.trialBalanceDS = new MatTableDataSource(res);
            this.trialBalanceDS.paginator = this.trialBalancePaginator;
        },
        ({error})=>{
            this.notify.error("Cannot retrieve balance sheet");
        })
    }

}