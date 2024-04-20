import { HttpClient, HttpEventType } from '@angular/common/http';
import { Component, ElementRef, EventEmitter, Injector, Input, Output, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { Sort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { ActivatedRoute, Router } from '@angular/router';
import { AppComponentBase } from '@shared/app-component-base';
import { EntityDto } from '@shared/paged-listing-component-base';
import { AttachmentServiceProxy } from '@shared/service-proxies/service-proxies';
import { NotifyService } from 'abp-ng2-module';


@Component({
    selector: 'app-customer-attachments-view',
    templateUrl: './customer-attachment-view.component.html',
    styleUrls: ['./customer-attachment-view.component.scss']
})
export class CustomerAttachmentsView extends AppComponentBase {
    @Input() comment: string;
    @Output() onSave = new EventEmitter<any>();
    @Output() activeTab = new EventEmitter<any>();
    @Input() totalBalance: number = 0;

    @ViewChild('customerAttachmentPaginator') customerAttachmentPaginator: MatPaginator;
    @ViewChild('inputFile') inputFile: ElementRef;

    progress: number;
    notify: NotifyService;
    createPasswordForm: FormGroup;
    showPass: boolean = false;
    customerAttachmentDS: any;
    customerAttachmentArr: any = [];
    isEdit: boolean = false;
    customerId: number;

    passwordColumn: string[] = ['fileName', 'action'];

    constructor(injector: Injector, protected _router: Router, private http: HttpClient, private _activatedRoute: ActivatedRoute) {
        super(injector);
    }

    ngOnInit(): void {
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
            }
        });
    }

    backToListView(): void {
        this._router.navigate(['/app/customers']);
    }

    applyFilter(event: Event) {
        const filterValue = (event.target as HTMLInputElement).value;
        this.customerAttachmentDS.filter = filterValue.trim().toLowerCase();
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
            this.customerAttachmentDS = data.sort((a, b) => a.fileName.split("_")[1] - b.fileName.split("_")[1]);
            return;
        }
        else if (!sort.active || sort.direction === "desc") {

            this.customerAttachmentDS = data.sort((a, b) => b.fileName.split("_")[1] - a.fileName.split("_")[1]);
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
                //this.notify.error("Cannot retrieve Attachments");
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
                    let obj = {
                        fileName: data,
                        fileNameAlt: data.split('_').pop()
                    }
                    this.customerAttachmentArr.push(obj);
                }
                this.customerAttachmentDS = new MatTableDataSource<any>(this.customerAttachmentArr.reverse());
                this.customerAttachmentDS.paginator = this.customerAttachmentPaginator;
            },
            (error) => {
                this.notify.error("Cannot retrieve Attachments");
            }
        );
    }

    continueAdd(): void {
        this.activeTab.emit('Diary');
        this.onSave.emit(this.comment);
    }

}
