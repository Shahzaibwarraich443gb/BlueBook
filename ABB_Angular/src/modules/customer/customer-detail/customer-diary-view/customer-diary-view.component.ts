import { Component, EventEmitter, Injector, Input, Output, TemplateRef, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { Sort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { ActivatedRoute, Router } from '@angular/router';
import { AppComponentBase } from '@shared/app-component-base';
import { EntityDto } from '@shared/paged-listing-component-base';
import { CustomerDiary, CustomerDiaryServiceProxy} from '@shared/service-proxies/service-proxies';
import { saveAs } from 'file-saver';
import * as Papa from 'papaparse';
import { NotifyService } from 'abp-ng2-module';


@Component({
    selector: 'app-customer-diary-view',
    templateUrl: './customer-diary-view.component.html',
    styleUrls: ['./customer-diary-view.component.scss']
})
export class CustomerDiaryViewComponent extends AppComponentBase {
    customerId: number;
    @Input() comment: string;
    @Input() totalBalance: number = 0;
    @Output() onSave = new EventEmitter<any>();
    @Output() activeTab = new EventEmitter<any>();

    @ViewChild('DiaryModal') DiaryModal !: TemplateRef<any>;
    @ViewChild('customerPassPaginator') customerPassPaginator: MatPaginator;

    notify: NotifyService;
    createDiaryForm: FormGroup;
    customerDiaryDS: any;
    customerDiaryArr: CustomerDiary[] = [];
    isEdit: boolean = false;
    editPassId: number;

    diaryColumn: string[] = ['DateAdded', 'Diary', 'Action'];

    constructor(injector: Injector, protected _router: Router,
        private _customerDiaryService: CustomerDiaryServiceProxy,
        private dialog: MatDialog,
        private _activatedRoute: ActivatedRoute) {
        super(injector);
    }

    ngOnInit(): void {
        this.createDiaryForm = new FormGroup({
            description: new FormControl('', Validators.required),
        });
    }

    backToListView(): void {
        this._router.navigate(['/app/customers']);
    }

    public getCustomerId(): void{
        this._activatedRoute.params.subscribe(parms => {
            if (parms.id) {
                this.customerId = parms.id;
                this.getDiary();
            }
        });
    }


    saveDiary(): void {
        let diaryObj = new CustomerDiary();
        diaryObj.description = this.createDiaryForm.controls.description.value;
        diaryObj.customerId = this.customerId;

        if (this.isEdit == true) {
            diaryObj.id = this.editPassId;
        }

        this._customerDiaryService.saveCustomerDiary(diaryObj).subscribe({
            next: (res) => {
                this.dialog.closeAll();
                this.notify.success("Diary Saved");
                this.getDiary();
            },
            error: (err) => {
                this.notify.error("Cannot Save Diary");
            }
        });
    }

    editDiary(data: any) {
        console.log(data);
        this.openDiaryDialog();
        this.createDiaryForm.controls.description.setValue(data.description);
        this.editPassId = data.id;
        this.isEdit = true;
    }

    getDiary(): void {
        let entityDto:any = {id: this.customerId};
        this._customerDiaryService.customerDiaryGet(entityDto).subscribe((res) => {
            this.customerDiaryArr = res;
            this.customerDiaryDS = new MatTableDataSource<any>(res.reverse());
        },
            ({ error }) => {
                this.notify.error("Cannot Get Diary");
            })
    }

    deleteDiary(data: any): void {
        let entityDto: any = new EntityDto();
        entityDto.id = data.id;
        this._customerDiaryService.deleteCustomerDiary(entityDto.id).subscribe((res)=>{
            this.notify.success("Diary Deleted");
            this.getDiary();
        },
        (error)=>{
            this.notify.error("Cannot Delete Diary")
        })
    }

    addDiary(): void {
        this.isEdit = false;
        this.openDiaryDialog();
    }

    openDiaryDialog(): void {
        this.createDiaryForm.reset();
        this.dialog.open(this.DiaryModal, {
            width: '50%',
            height: 'auto',
            disableClose: false,
        });
    }

    addCustomerPasswordComment(): void {
        this.activeTab.emit('Info');
        this.onSave.emit(this.comment ? this.comment : null);
    }

    compare(a: number | string, b: number | string, isAsc: boolean): any {
        return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
    }

    sortCustomerPassData(sort: Sort): void {
        const data = this.customerDiaryArr.slice();
        if (!sort.active || sort.direction === 'asc') {
            this.customerDiaryDS = data.sort((a, b) => a.id - b.id);
            return;
        }
        else if (!sort.active || sort.direction === "desc") {

            this.customerDiaryDS = data.sort((a, b) => b.id - a.id);
            return
        }
    }

    applyFilter(event: Event) {
        const filterValue = (event.target as HTMLInputElement).value;
        this.customerDiaryDS.filter = filterValue.trim().toLowerCase();
    }


    downloadDiary(): void{
        let dataArr = [];
        for(var data of this.customerDiaryArr.reverse()){
            let obj = {
                AddedOn: data.addedOn,
                Diary: data.description,
            }
            dataArr.push(obj);
        }
  
        const csv = Papa.unparse(dataArr, {
          header: true,
        });
        
        const blob = new Blob([csv], { type: 'text/csv;charset=utf-8' });
        saveAs(blob, 'Diary.csv');
    }

}
