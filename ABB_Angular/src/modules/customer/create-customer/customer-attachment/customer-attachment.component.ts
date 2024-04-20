import { HttpClient, HttpEventType } from '@angular/common/http';
import { Component, ElementRef, EventEmitter, Inject, Injector, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { Sort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { Router } from '@angular/router';
import { AppComponentBase } from '@shared/app-component-base';
import { EntityDto } from '@shared/paged-listing-component-base';
import { AttachmentServiceProxy } from '@shared/service-proxies/service-proxies';
import { NotifyService } from 'abp-ng2-module';
import { CustomerEmailAttachmentComponent } from '../customer-email-attachment/customer-email-attachment.component';
import { NgxSpinnerService } from 'ngx-spinner';


@Component({
    selector: 'app-customer-attachments',
    templateUrl: './customer-attachment.component.html',
    styleUrls: ['./customer-attachment.component.scss']
})
export class CustomerAttachments extends AppComponentBase implements OnInit {
    @Input() customerId: number;
    @Input() comment: string;
    @Output() onSave = new EventEmitter<any>();
    @Output() activeTab = new EventEmitter<any>();

    @ViewChild('customerAttachmentPaginator') customerAttachmentPaginator: MatPaginator;
    @ViewChild('inputFile') inputFile: ElementRef;

    progress: number;
    notify: NotifyService;
    createPasswordForm: FormGroup;
    showPass: boolean = false;
    customerAttachmentDS: any;
    customerAttachmentArr: any = [];
    isEdit: boolean = false;

    passwordColumn: string[] = ['fileName', 'Date', 'AddedBy', 'ShowtoCustomer', 'SendviaEmail', 'action'];
    selectedFiles: any = [];
    isShow: boolean = true;
    data = null;
    dialogRef = null;

    constructor(
        public injector: Injector,
        private _router: Router,
        private http: HttpClient,
        // public dialogRef: MatDialogRef<CustomerAttachments>,
        private _attachmentServiceProxy: AttachmentServiceProxy,
        public _dialog: MatDialog,
        private spinner: NgxSpinnerService,
    ) {
        super(injector);
        this.data = injector.get(MAT_DIALOG_DATA, null);
        this.dialogRef = injector.get(MatDialogRef<CustomerAttachments>, null);
    }


    ngOnInit(): void {
        this.spinner.show();
        this.createPasswordForm = new FormGroup({
            type: new FormControl('', Validators.required),
            userName: new FormControl('', Validators.required),
            password: new FormControl('', Validators.required),
            url: new FormControl(''),
            description: new FormControl(''),
        });
        if (this.data?.id) {
            this.customerId = this.data.id;
            this.isShow = false;
        }
        this.getCustomerAttachments();
    }

    backToListView(): void {
        this.dialogRef.close();
        this._router.navigate(['/app/customers']);
    }



    invokeAddFile(): void {
        (this.inputFile.nativeElement as HTMLInputElement).click();
    }

    compare(a: number | string, b: number | string, isAsc: boolean): any {
        return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
    }

    sortCustomerPassData(sort: Sort): void {
        const data = this.customerAttachmentArr.slice();
        if (!sort.active || sort.direction === 'asc') {
            this.customerAttachmentDS = data.sort((a, b) => a.id - b.id);
            return;
        }
        else if (!sort.active || sort.direction === "desc") {

            this.sortCustomerPassData = data.sort((a, b) => b.id - a.id);
            return
        }
    }

    downloadAttachment(fileName: string): void {
        this.http.get("https://localhost:44311/api/Attachment/GetFile/" + fileName, { responseType: 'blob' }).subscribe(
            (res: Blob) => {
                // Create a URL for the blob data
                const blobUrl = URL.createObjectURL(res);

                // Create a temporary link element
                const link = document.createElement('a');
                link.href = blobUrl;
                link.download = fileName.split('_').pop();

                // Trigger the download
                link.click();

                // Clean up the created URL
                URL.revokeObjectURL(blobUrl);
            },
            (error) => {
                // this.notify.error("Cannot retrieve Attachments");
            }
        );
    }

    deleteAttachment(fileName: string): void {
        this.http.delete("https://localhost:44311/api/Attachment/DeleteFile/" + fileName).subscribe(
            (res) => {
                this.notify.success('File Deleted');
                this.getCustomerAttachments();
            },
            ({ error }) => {
                this.notify.error("Cannot Delete File");
            }
        );
    }

    addFile(): void {
        let files = (this.inputFile.nativeElement as HTMLInputElement).files;
        let fileToUpload = <File>files[0];
        const formData = new FormData();
        const date = new Date(); // Create a new Date object with the current date and time
        const timestamp = date.getTime()
        formData.append(fileToUpload.name + "_" + this.customerId, fileToUpload, this.customerId + "_" + timestamp + "_" + fileToUpload.name);

        this.http.post('https://localhost:44311/api/Attachment/upload', formData, { reportProgress: true, observe: 'events' })
            .subscribe(event => {
                if (event.type === HttpEventType.UploadProgress)
                    this.progress = Math.round(100 * event.loaded / event.total);
                else if (event.type === HttpEventType.Response) {
                    //uploaded
                    this.getCustomerAttachments();
                }


            }
            );
    }

    getCustomerAttachments(): void {
        let eDto: EntityDto = new EntityDto();
        eDto.id = this.customerId;
        this.customerAttachmentArr = [];
        this.http.post("https://localhost:44311/api/Attachment/CustomerAttachmentsGet", eDto).subscribe(
            (res: any) => {
                for (var data of res.result) {
                    const parts = data.split('_');
                    const timestamp = parts[1];
                    const date = new Date(parseInt(timestamp, 10));
                    const year = date.getFullYear();
                    const month = date.getMonth() + 1;
                    const day = date.getDate();
                    const formattedDate = `${year}-${month.toString().padStart(2, '0')}-${day.toString().padStart(2, '0')}`;

                    let obj = {
                        fileName: data,
                        fileNameAlt: data.split('_').pop(),
                        date: formattedDate,
                    };
                    this.customerAttachmentArr.push(obj);
                }
                this.customerAttachmentDS = new MatTableDataSource<any>(this.customerAttachmentArr.reverse());
                this.customerAttachmentDS.paginator = this.customerAttachmentPaginator;
                this.spinner.hide();
            },
            (error) => {
                this.notify.error("Cannot retrieve Attachments");
            }
        );
    }

    applyFilter(event: Event) {
        const filterValue = (event.target as HTMLInputElement).value;
        this.customerAttachmentDS.filter = filterValue.trim().toLowerCase();
    }

    selectAttachment(item: any, i: number) {
        this.selectedFiles = this.customerAttachmentDS.filteredData.filter((obj: any) => {
            return obj.emailSelected === true;
        });
    }

    sendMail() {
        this.showEmailAttachment(this.customerId);
    }

    ngOnDestroy(): void {
        this._dialog.closeAll();
    }

    private showEmailAttachment(id?: number): void {
        const dialogRef = this._dialog.open(CustomerEmailAttachmentComponent, {
            data: { id: id, selectedEmails: this.selectedFiles },
        });

        dialogRef.afterClosed().subscribe((result) => {
            dialogRef.close();
        });
    }


    continueAdd(): void {
        this.activeTab.emit('Diary');
        this.onSave.emit(this.comment ? this.comment : "");
    }

}
