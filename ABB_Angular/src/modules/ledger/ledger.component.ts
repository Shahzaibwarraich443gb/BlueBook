import { Component, Injector } from "@angular/core";
import { AppComponentBase } from "@shared/app-component-base";

@Component({
    selector: 'app-ledger',
    templateUrl: './ledger.component.html',
    styleUrls: ['./ledger.component.scss']
})
export class LedgerComponent extends AppComponentBase {

    constructor(private injector: Injector) {
        super(injector);
    }
}