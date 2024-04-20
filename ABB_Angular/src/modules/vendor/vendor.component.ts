import { JobTitleDto, VenderServiceProxy, VendorContactInfoServiceProxy } from "./../../shared/service-proxies/service-proxies";
import { Component, Injector, OnInit, ViewChild } from "@angular/core";
import { ThemePalette } from "@angular/material/core";
import { MatDialog } from "@angular/material/dialog";
import { PagedRequestDto } from "@shared/paged-listing-component-base";
import { AppComponentBase } from "shared/app-component-base";
import { CreateOrEditVendorComponent } from "./create-or-edit-vendor/create-or-edit-vendor.component";
import { finalize } from "rxjs";
import { VendorAttachments } from "./vendor-attachment/vendor-attachment.component";
import { NgxSpinnerService } from "ngx-spinner";
import { MatSort, Sort } from "@angular/material/sort";
import { MatTableDataSource } from "@angular/material/table";
import { MatPaginator } from "@angular/material/paginator";

class PagedUsersRequestDto extends PagedRequestDto {
  keyword: string;
  isActive: boolean | null;
}

//for checkbox
export interface Task {
  name: string;
  completed: boolean;
  color: ThemePalette;
  subtasks?: Task[];
}
@Component({
  selector: "app-vendor",
  templateUrl: "./vendor.component.html",
  styleUrls: ["./vendor.component.css"],
})
export class VendorComponent extends AppComponentBase implements OnInit {
  totalRecords = 0;
  vendorDto: JobTitleDto[] = [];
  allComplete: boolean = false;
  task: Task = {
    name: "",
    completed: false,
    color: "primary",
  };
  vendorDS: any;
  keyword = "";
  isActive: boolean | null;
  vendorList: any;

  someComplete(): boolean {
    if (this.task.subtasks == null) {
      return false;
    }
    return (
      this.task.subtasks.filter((t) => t.completed).length > 0 &&
      !this.allComplete
    );
  }

  updateRows(event) {
    let req = new PagedUsersRequestDto();
    req.maxResultCount = event.maxResultCount;
    req.skipCount = event.skipCount;
    console.log("user", event);
  }

  constructor(
    injector: Injector,
    public _dialog: MatDialog,
    public _vendorService: VenderServiceProxy,
    public _vendorContactInfoServiceProxy: VendorContactInfoServiceProxy,
    private spinner: NgxSpinnerService,
  ) {
    super(injector);
  }
  @ViewChild(MatSort) sort: MatSort;
  @ViewChild("VendorPaginator") VendorPaginator: MatPaginator;
  columns: string[] = ['no', 'name', 'company', 'phoneNumber', 'emailAddress', 'file', 'status', 'action'];

  ngOnInit() {
    this.spinner.show();
    this.getVendorList();
  }

  getVendorList() {
    this._vendorService.getAll().pipe(
      finalize(() => { }))
      .subscribe((result) => {
        this.vendorList = result;
        this.vendorDS = new MatTableDataSource<any>(this.vendorList.reverse());
        this.vendorDS.paginator = this.VendorPaginator;
        this.vendorDS.sort = this.sort;
        this.spinner.hide();
      });
  }

  sortVendorData(sort: Sort): void {
    const data = this.vendorList.slice();
    if (!sort.active || sort.direction === 'asc') {
      this.vendorDS = data.sort((a, b) => a.id - b.id);
      return;
    }
    else if (!sort.active || sort.direction === "desc") {

      this.vendorDS = data.sort((a, b) => b.id - a.id);
      return
    }
  }

  protected delete(vendorId: any): void {
    abp.message.confirm(
      this.l("Are you sure to delete vendor details"),
      undefined,
      (result: boolean) => {
        if (result) {
          // delete vendor
          this.spinner.show();
          this._vendorService.delete(vendorId.id)
            .subscribe(() => {
              // delete contact
              this._vendorContactInfoServiceProxy.delete(vendorId.id)
                .subscribe(() => {
                  // delete address
                  this._vendorContactInfoServiceProxy.deleteAddress(vendorId.id)
                    .subscribe(() => {
                      // this.vendorList = this.vendorList.filter((obj) => {
                      //   return obj.id !== vendorId.id
                      // });
                      this.getVendorList();
                      this.spinner.hide();
                      this.notify.success("successfully deleted");
                    });
                });
            });
        }
      }
    );
  }

  search(event: any) {
    const val = event.target.value.toLowerCase();
    if (val == "") {
      this.getVendorList();
    } else {
      const temp = this.vendorDto.filter(function (d) {
        return d.name.toLowerCase().indexOf(val) !== -1;
      });
      this.vendorDto = temp;
    }
  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.vendorDS.filter = filterValue.trim().toLowerCase();
  }

  editJobTitle(vendorDto: any): void {
    this.showCreateOrEditJobTitleDialog(vendorDto.id);
  }

  createJobTitle(id?: number): void {
    this.showCreateOrEditJobTitleDialog(id);
  }

  private showCreateOrEditJobTitleDialog(id?: number): void {
    const dialogRef = this._dialog.open(CreateOrEditVendorComponent, {
      data: { id: id },
    });

    dialogRef.afterClosed().subscribe((result) => {
      dialogRef.close();
      this.getVendorList();
    });
  }

  showAttchment(vendorDto: any): void {
    this.showVendorAttachment(vendorDto.id);
  }

  private showVendorAttachment(id?: number): void {
    const dialogRef = this._dialog.open(VendorAttachments, {
      data: { id: id },
    });

    dialogRef.afterClosed().subscribe((result) => {
      dialogRef.close();
    });
  }

}
