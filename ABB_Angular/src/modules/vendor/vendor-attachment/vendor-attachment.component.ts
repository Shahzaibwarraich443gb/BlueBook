import { HttpClient, HttpEventType } from '@angular/common/http';
import { Component, ElementRef, EventEmitter, Inject, Injector, Input, Output, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { Sort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { Router } from '@angular/router';
import { AppComponentBase } from '@shared/app-component-base';
import { EntityDto } from '@shared/paged-listing-component-base';
import { AttachmentServiceProxy } from '@shared/service-proxies/service-proxies';
import { NotifyService } from 'abp-ng2-module';
import { NgxSpinnerService } from 'ngx-spinner';
import { EmailAttachmentComponent } from '../email-attachment-modal/email-attachment.component';
export interface DialogData {
    item: any;
}


@Component({
    selector: 'app-vendor-attachments',
    templateUrl: './vendor-attachment.component.html',
    styleUrls: ['./vendor-attachment.component.scss']
})
export class VendorAttachments extends AppComponentBase {
    @Input() vendorId: number;
    @Input() comment: string;
    @Output() onSave = new EventEmitter<any>();
    @Output() activeTab = new EventEmitter<any>();

    @ViewChild('vendorAttachmentPaginator') vendorAttachmentPaginator: MatPaginator;
    @ViewChild('inputFile') inputFile: ElementRef;

    progress: number;
    emailSelected: boolean;
    notify: NotifyService;
    createPasswordForm: FormGroup;
    showPass: boolean = false;
    vendorAttachmentDS: any;
    vendorAttachmentArr: any = [];
    isEdit: boolean = false;

    passwordColumn: string[] = ['fileName', 'Date', 'AddedBy', 'ShowtoVendor', 'SendviaEmail', 'action'];
    selectedFiles: any = [];

    constructor(injector: Injector, private _router: Router, private http: HttpClient, private _attachmentServiceProxy: AttachmentServiceProxy
        , @Inject(MAT_DIALOG_DATA) public data: any,
        public _dialog: MatDialog,
        private spinner: NgxSpinnerService,) {
        super(injector);
    }

    ngOnInit(): void {
        this.spinner.show();
        this.vendorId = this.data.id;
        this.getVendorAttachments();
    }

    backToListView(): void {
        this._router.navigate(['/app/vendor']);
    }

    invokeAddFile(): void {
        (this.inputFile.nativeElement as HTMLInputElement).click();
    }

    compare(a: number | string, b: number | string, isAsc: boolean): any {
        return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
    }

    sortVendorPassData(sort: Sort): void {
        const data = this.vendorAttachmentArr.slice();
        if (!sort.active || sort.direction === 'asc') {
            this.vendorAttachmentDS = data.sort((a, b) => a.id - b.id);
            return;
        }
        else if (!sort.active || sort.direction === "desc") {

            this.sortVendorPassData = data.sort((a, b) => b.id - a.id);
            return
        }
    }

    downloadAttachment(fileName: string): void {
        this.http.get("https://localhost:44311/api/VendorAttachment/GetFile/" + fileName, { responseType: 'blob' }).subscribe(
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
                this.notify.error("Cannot retrieve Attachments");
            }
        );
    }

    deleteAttachment(fileName: string): void {
        this.http.delete("https://localhost:44311/api/VendorAttachment/DeleteFile/" + fileName).subscribe(
            (res) => {
                this.notify.success('File Deleted');
                this.getVendorAttachments();
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
        const vendorId = this.vendorId; // Replace this with the actual vendorId value
        // Append the vendorId to the formData
        formData.append('vendorId', vendorId.toString());

        formData.append(fileToUpload.name + "_" + this.vendorId, fileToUpload, this.vendorId + "_" + timestamp + "_" + fileToUpload.name);
        this.http.post('https://localhost:44311/api/VendorAttachment/upload', formData, { reportProgress: true, observe: 'events' })
            .subscribe(event => {
                if (event.type === HttpEventType.UploadProgress)
                    this.progress = Math.round(100 * event.loaded / event.total);
                else if (event.type === HttpEventType.Response) {
                    //uploaded
                    this.getVendorAttachments();
                }
            }
            );
    }

    getVendorAttachments(): void {
        let eDto: EntityDto = new EntityDto();
        eDto.id = this.vendorId;
        this.vendorAttachmentArr = [];
        this.http.post("https://localhost:44311/api/VendorAttachment/VendorAttachmentsGet", eDto).subscribe(
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
                    this.vendorAttachmentArr.push(obj);
                }
                this.vendorAttachmentDS = new MatTableDataSource<any>(this.vendorAttachmentArr.reverse());
                this.vendorAttachmentDS.paginator = this.vendorAttachmentPaginator;
                this.spinner.hide();
            },
            (error) => {
                this.spinner.hide();
                this.notify.error("Cannot retrieve Attachments");
            }
        );
    }

    applyFilter(event: Event) {
        const filterValue = (event.target as HTMLInputElement).value;
        this.vendorAttachmentDS.filter = filterValue.trim().toLowerCase();
    }

    selectAttachment(item: any, i: number) {
        this.selectedFiles = this.vendorAttachmentDS.filteredData.filter((obj: any) => {
            return obj.emailSelected === true;
        });
    }

    sendMail() {
        this.showEmailAttachment(this.vendorId);
    }

    private showEmailAttachment(id?: number): void {
        const dialogRef = this._dialog.open(EmailAttachmentComponent, {
            data: { id: id, selectedEmails: this.selectedFiles },
        });

        dialogRef.afterClosed().subscribe((result) => {
            dialogRef.close();
        });
    }

}
