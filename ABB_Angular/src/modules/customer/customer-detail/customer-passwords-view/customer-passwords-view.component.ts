import { Component, EventEmitter, Injector, Input, Output, TemplateRef, ViewChild } from "@angular/core";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { MatDialog } from "@angular/material/dialog";
import { MatPaginator } from "@angular/material/paginator";
import { MatSort, Sort } from "@angular/material/sort";
import { MatTableDataSource } from "@angular/material/table";
import { ActivatedRoute, Route, Router } from "@angular/router";
import { AppComponentBase } from "@shared/app-component-base";
import { CustomerPassword, CustomerServiceProxy } from "@shared/service-proxies/service-proxies";
import { NotifyService } from "abp-ng2-module";



@Component({
    selector: 'app-customer-passwords-view',
    templateUrl: './customer-passwords-view.component.html',
    styleUrls: ['./customer-passwords-view.component.scss']
})
export class CustomerPasswordsViewComponent extends AppComponentBase {
    @Output() onSave = new EventEmitter<any>();
    @Output() activeTab = new EventEmitter<any>();
    @Input() comment: string;
    @Input() totalBalance: number = 0;

    @ViewChild('passwordModal') passwordModal !: TemplateRef<any>;
    @ViewChild('customerPaginator') customerPaginator: MatPaginator;
    @ViewChild(MatSort) sort: MatSort;

    customerId: number = 0;
    passwordComment: string = null;
    notify: NotifyService;
    createPasswordForm: FormGroup;
    showPass: boolean = false;
    customerPassDS: any;
    customerPassArr: CustomerPassword[] = [];
    isEdit: boolean = false;
    editPassId: number;

    passwordColumn: string[] = ['type', 'userName', 'password', 'url', 'description', 'action'];

    constructor(injector: Injector, 
        protected _router: Router,
        private _activatedRoute: ActivatedRoute,
        private _customerService: CustomerServiceProxy,
        private dialog: MatDialog) {
        super(injector);
    }

    applyFilter(event: Event) {
        const filterValue = (event.target as HTMLInputElement).value;
        this.customerPassDS.filter = filterValue.trim().toLowerCase();
    }


    public initialization(): void {
        this.createPasswordForm = new FormGroup({
            type: new FormControl('', Validators.required),
            userName: new FormControl('', Validators.required),
            password: new FormControl('', Validators.required),
            url: new FormControl(''),
            description: new FormControl(''),
        });
        this._activatedRoute.params.subscribe(x => {
            if (x.id) {
                this.customerId = x.id;
                this.getPasswords();
                if (!this.comment) {
                    this._customerService.getCustomerComment(this.customerId).subscribe({
                        next: (res) => {
                            this.comment = res;
                        }
                    })
                }
            }
        });
    }

    backToListView(): void {
        this._router.navigate(['/app/customers']);
    }

    editPass(data): void {
        this.isEdit = true;
        this.editPassId = data.id;
        this.openPasswordDialog();

        this.createPasswordForm.controls.type.setValue(data.type);
        this.createPasswordForm.controls.userName.setValue(data.userName);
        this.createPasswordForm.controls.password.setValue(data.password);
        this.createPasswordForm.controls.url.setValue(data.url);
    }

    savePassword(): void {
        let passwordObj = new CustomerPassword();
        passwordObj.type = this.createPasswordForm.controls.type.value;
        passwordObj.userName = this.createPasswordForm.controls.userName.value;
        passwordObj.password = this.createPasswordForm.controls.password.value;
        passwordObj.url = this.createPasswordForm.controls.url.value;
        passwordObj.description = this.createPasswordForm.controls.description.value;
        passwordObj.customerId = this.customerId;

        if (this.isEdit == true) {
            passwordObj.id = this.editPassId;
        }

        this._customerService.saveCustomerPassword(passwordObj).subscribe({
            next: (res) => {
                this.getPasswords();
                this.dialog.closeAll();
                this.notify.success("Password Saved");
                this.getPasswords();
            },
            error: (err) => {
                this.notify.error("Cannot Save Password");
            }
        });
    }

    getPasswords(): void {
        this._customerService.customerPasswordGet(this.customerId).subscribe({
            next: (res) => {
                this.customerPassArr = res;
                this.customerPassDS = new MatTableDataSource<any>(res.reverse());
                this.customerPassDS.paginator = this.customerPaginator;
                this.customerPassDS.sort = this.sort;
            },
            error: (err) => {
                this.notify.error("Cannot retrieve password");
            }
        })
    }

    deletePass(data): void {
        this._customerService.deleteCustomerPassword(data.id).subscribe({
            next: (res) => {
                this.notify.info("Password Removed");
                this.getPasswords();
            },
            error: (err) => {
                this.notify.error("Cannot delete password");
            }
        });
    }

    addPass(): void {
        this.isEdit = false;
        this.openPasswordDialog();
    }

    openPasswordDialog(): void {
        this.createPasswordForm.reset();
        this.dialog.open(this.passwordModal, {
            width: '50%',
            height: 'auto',
            disableClose: false,
        });
    }

    addCustomerPasswordComment(): void {
        this.activeTab.emit('License');
        this.onSave.emit(this.comment ? this.comment : "");
    }

    compare(a: number | string, b: number | string, isAsc: boolean): any {
        return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
    }

    sortCustomerPassData(sort: Sort): void {
        const data = this.customerPassArr.slice();
        if (!sort.active || sort.direction === 'asc') {
            this.customerPassDS = data.sort((a, b) => a.id - b.id);
            return;
        }
        else if (!sort.active || sort.direction === "desc") {

            this.customerPassDS = data.sort((a, b) => b.id - a.id);
            return
        }
    }

}
