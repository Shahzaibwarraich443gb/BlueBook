import { Component, Inject, Injector, OnInit, Renderer2, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { CustomerDto, CustomerServiceProxy } from './../../shared/service-proxies/service-proxies';
import { finalize } from 'rxjs';
import { Router } from '@angular/router';
import { MatSort, Sort } from "@angular/material/sort";
import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { CustomerAttachments } from './create-customer/customer-attachment/customer-attachment.component';

@Component({
  selector: 'app-customer',
  templateUrl: './customer.component.html',
  styleUrls: ['./customer.component.scss']
})
export class CustomerComponent extends AppComponentBase implements OnInit {
  displayedColumns: string[] = ["name", "customerName"];
  @ViewChild(MatSort) sort: MatSort;
  @ViewChild("customerPaginator") customerPaginator: MatPaginator;

  customers: CustomerDto[] = [];
  customerDS: any;

  keyword: string;
  filterType: string = 'Name';
  sidebarExpanded: boolean;
  //sidebar menu activation start
  menuSidebarActive: boolean = false;
  columns: string[] = ['no', 'name', 'company', 'phoneNumber', 'emailAddress', 'file', 'action'];
  constructor(
    injector: Injector,
    public _dialog: MatDialog,
    private _router: Router,
    // @Inject(MAT_DIALOG_DATA) public data: any,
    public _customerServiceProxy: CustomerServiceProxy
  ) {
    super(injector);
  }
  ngOnInit(): void {
    this.list();
  }

  sortCustomerData(sort: Sort): void {
    const data = this.customers.slice();
    if (!sort.active || sort.direction === 'asc') {
      this.customerDS = data.sort((a, b) => a.id - b.id);
      return;
    }
    else if (!sort.active || sort.direction === "desc") {

      this.customerDS = data.sort((a, b) => b.id - a.id);
      return
    }
  }

  protected list() {
    this._customerServiceProxy.getAll(
      this.keyword,
      this.filterType,
      0, 10
    ).pipe(
      finalize(() => {
      })
    )
      .subscribe((result) => {
        this.customers = result.items;
        this.customerDS = new MatTableDataSource<any>(this.customers.reverse());
        this.customerDS.paginator = this.customerPaginator;
        this.customerDS.sort = this.sort;
      });
  }

  showAttchment(customerDto: any): void {
    this.showCustomerAttachment(customerDto.id);
  }

  private showCustomerAttachment(id?: number): void {
    const dialogRef = this._dialog.open(CustomerAttachments, {
      data: { id: id, isComment: false },
    });

    dialogRef.afterClosed().subscribe((result) => {
      dialogRef.close();
    });
  }

  createCustomers(id?: number) {
    this.showCreateOrEditCustomerDialog(id);
  }

  private showCreateOrEditCustomerDialog(id?: number): void {
    this._router.navigate(['/app/create-customer']);
  }

  delete(item: CustomerDto) {
    abp.message.confirm(
      this.l('Are you sure to delete Customer'),
      undefined,
      (result: boolean) => {
        if (result) {
          this._customerServiceProxy.delete(item.id).pipe(
            finalize(() => {
            })
          )
            .subscribe((result) => {

              abp.notify.success(this.l('SuccessfullyDeleted'));
              this.list();

            });
        }
      }
    );

  }

  edit(item: CustomerDto) {
    this.showCreateOrEditCustomerDialog(item?.id);

  }

  search() {
    let val = this.keyword.trim().toLocaleLowerCase();
    let temp = this.customers;
    if (val) {
      temp = this.customers.filter((d) => {
        for (var [key, data] of Object.entries(d)) {
          if (data) {
            if (data.toString().toLowerCase().indexOf(val) !== -1) {
              return true;
            }
          }
        }

        if (d.customerType) {

          for (var [key, data] of Object.entries(d.customerType)) {
            if (data) {
              if (data.toString().toLowerCase().indexOf(val) !== -1) {
                return true;
              }
            }
          }
        }

        if (d.spouse) {
          for (var [key, data] of Object.entries(d.spouse)) {
            if (data) {
              if (data.toString().toLowerCase().indexOf(val) !== -1) {
                return true;
              }
            }
          }
        }

        return false;
      });

      this.customerDS = new MatTableDataSource<any>(temp.reverse());
      this.customerDS.paginator = this.customerPaginator;
      this.customerDS.sort = this.sort;
    }
  }

  viewDetail(id: any): void {
    this._router.navigate(['/app/customer-detail-view', id]);
  }
}
