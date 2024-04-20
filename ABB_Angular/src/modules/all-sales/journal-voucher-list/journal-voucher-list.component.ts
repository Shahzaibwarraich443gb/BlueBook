import { Component, Injector, OnInit, ViewChild, } from '@angular/core';
import { InvoiceServiceProxy, JournalVoucherServiceServiceProxy } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs';
import * as moment from 'moment';
import { AppComponentBase } from '@shared/app-component-base';
import { ActivatedRoute } from '@angular/router';
import { MatSort, Sort } from '@angular/material/sort';
import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { NgxSpinnerService } from 'ngx-spinner';
import { MatDialog } from '@angular/material/dialog';
import { ViewHistoryComponent } from '../view-history/view-history.component';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-journal-voucher-list',
  templateUrl: 'journal-voucher-list.component.html',
  styleUrls: ['journal-voucher-list.component.scss'],
})

export class JournalVoucherListComponent extends AppComponentBase implements OnInit {
  isActive: boolean = false;
  dataSource: any;
  voucherList: any;

  constructor(
    private injector: Injector,
    public _dialog: MatDialog,
    private _invoiceService: InvoiceServiceProxy,
    public _journalVoucherService: JournalVoucherServiceServiceProxy,
    private _activatedRoute: ActivatedRoute,
    private spinner: NgxSpinnerService,
  ) {
    super(injector);
  }

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  displayedColumns: string[] = ['Date', 'VoucherNo', 'VoucherType', 'Company', 'InvoiceID', 'SrNo', 'AccountHead', 'CrAmount', 'DrAmount', 'Action'];

  ngOnInit() {
    this.getAllVouchers();
  }

  public getAllVouchers(): void {
    this.spinner.show();
    this._invoiceService.getAllVouchers().subscribe((res) => {
      this.voucherList = res;

      this.spinner.hide();
      for (var data of this.voucherList) {
        data.invoiceDateAlt = moment(data.date).format('MM/DD/YYYY');
      }

      this.dataSource = new MatTableDataSource<any>(this.voucherList);
      this.dataSource.paginator = this.paginator;
    });
  }

  public viewHistoryModal(item?: any): void {
    const dialogRef = this._dialog.open(ViewHistoryComponent, {
      data: { item: item },
    });
    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        console.log(result);
      }
      dialogRef.close();
    });
  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();
  }
  sortJV(sort: Sort): void {
    const data = this.voucherList.slice();
    if (!sort.active || sort.direction === 'asc') {
      this.dataSource.data = data;
      return;
    }

    this.dataSource.data = data.sort((a, b) => {
      const isAsc = sort.direction === 'asc';
      return this.compare(a.Roles, b.Roles, isAsc);
    });
  }
  compare(a: number | string, b: number | string, isAsc: boolean): any {
    return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
  }

  deleteVoucher(item: any) {
    abp.message.confirm(
      this.l(`Are you sure want to delete ` + item.voucherNo),
      undefined,
      (result: boolean) => {
        if (result) {
          this._journalVoucherService.deleteVoucherDetail(item.id).pipe(
            finalize(() => { }))
            .subscribe((result) => {
              if (result === 1) {
                this.getAllVouchers();
                // this.voucherList = this.voucherList.filter(el => {
                //   return el.id !== this.voucherList.id;
                // });
                abp.notify.success(this.l('Successfully Deleted'));
              } else {
                abp.notify.error(this.l('Voucher Already Deleted'));
              }
            });
        }
      }
    );
  }

}
