import { CreateContactTypePersonComponent } from './create-contact-person-type/create-contact-person-type.component';
import {
  ContactPersonTypeDto,
  EntityDto,
  ContactPersonTypeServiceProxy,
} from "./../../shared/service-proxies/service-proxies";
import { Component, Injector, OnInit } from "@angular/core";
import { ThemePalette } from "@angular/material/core";
import { MatDialog } from "@angular/material/dialog";
import { PagedRequestDto } from "@shared/paged-listing-component-base";
import {
  AccountNature,
  ChartOfAccountDto,
} from "@shared/service-proxies/service-proxies";
import { AppComponentBase } from "shared/app-component-base";
import { CreateChartOfAccountComponent } from "modules/chart-of-account/create-chart-of-account/create-chart-of-account.component";
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
  selector: "app-contact-person-type",
  templateUrl: "./contact-person-type.component.html",
  styleUrls: ["./contact-person-type.component.css"],
})
export class ContactPersonTypeComponent
  extends AppComponentBase
  implements OnInit
{ 
  totalRecords = 0;
  contactPersonTypeDto: ContactPersonTypeDto[] = [];
  natureOfAccount: AccountNature;
  allComplete: boolean = false;
  task: Task = {
    name: "",
    completed: false,
    color: "primary",
  };
  keyword = "";
  isActive: boolean | null;
  
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
    public _contactPersonTypeServiceProxy: ContactPersonTypeServiceProxy
  ) {
    super(injector);
  }

  ngOnInit() {
    this.list();
  }

  protected list() {
    this._contactPersonTypeServiceProxy
      .getAll()
      .subscribe((arg) => (this.contactPersonTypeDto = arg));
  }

  createContactPersonType(id?: number): void {
    this.showCreateOrEditContactPersonTypeDialog(id);
  }

  protected delete(contactPersonTypeDto: ContactPersonTypeDto): void { 
    abp.message.confirm(
      this.l("Are you sure to delete  contact person"),
      undefined,
      (result: boolean) => {
        if (result) {
          this._contactPersonTypeServiceProxy
            .delete(contactPersonTypeDto.id)
            .subscribe(() => {
              abp.notify.success(this.l("SuccessfullyDeleted"));
              this.list();
            });
        }
      }
    );
  }

  editContactPersonType(contactPersonTypeDto: ContactPersonTypeDto): void {
    this.showCreateOrEditContactPersonTypeDialog(contactPersonTypeDto.id);
  }

  private showCreateOrEditContactPersonTypeDialog(id?: number): void {
    const dialogRef = this._dialog.open(CreateContactTypePersonComponent, {
      data: { id: id },
    });

    dialogRef.afterClosed().subscribe((result) => {
      dialogRef.close();
      this.list();
    });
  }

  search(event: any) {
    const val = event.target.value.toLowerCase();
    if (val == "") {
      this.list();
    } else {
      const temp = this.contactPersonTypeDto.filter(function (d) {
        return d.name.toLowerCase().indexOf(val) !== -1; 
      });
      this.contactPersonTypeDto = temp;
    }
  }
}
