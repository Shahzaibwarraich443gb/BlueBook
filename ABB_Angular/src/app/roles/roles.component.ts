import { Component, Injector } from "@angular/core";
import { finalize } from "rxjs/operators";
import { BsModalRef, BsModalService } from "ngx-bootstrap/modal";
import { appModuleAnimation } from "@shared/animations/routerTransition";
import {
  PagedListingComponentBase,
  PagedRequestDto,
} from "@shared/paged-listing-component-base";
import {
  RoleServiceProxy,
  RoleDto,
  RoleDtoPagedResultDto,
} from "@shared/service-proxies/service-proxies";
import { CreateRoleDialogComponent } from "./create-role/create-role-dialog.component";
import { EditRoleDialogComponent } from "./edit-role/edit-role-dialog.component";
import { MatDialog } from "@angular/material/dialog";
import { ThemePalette } from "@angular/material/core";

class PagedRolesRequestDto extends PagedRequestDto {
  keyword: string;
}
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
  templateUrl: "./roles.component.html",
  animations: [appModuleAnimation()],
})
export class RolesComponent extends PagedListingComponentBase<RoleDto> {
  roles: RoleDto[] = [];
  keyword = "";
  totalRecords = 0;
  allComplete: boolean = false;
  task: Task = {
    name: "",
    completed: false,
    color: "primary",
  };
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
  displayedColumns: string[] = [
    "name",
    "displayName",
    "description",
    "actions",
  ];
  constructor(
    injector: Injector,
    private _rolesService: RoleServiceProxy,
    public _dialog: MatDialog
  ) {
    super(injector);
  }

  list(
    request: PagedRolesRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    request.keyword = this.keyword;

    this._rolesService
      .getAll(request.keyword, request.skipCount, request.maxResultCount)
      .pipe(
        finalize(() => {
          finishedCallback();
        })
      )
      .subscribe((result: RoleDtoPagedResultDto) => {
        this.roles = result.items;
        this.showPaging(result, pageNumber);
      });
  }

  updateRows(event) {
    let req = new PagedRolesRequestDto();
    req.maxResultCount = event.maxResultCount;
    req.skipCount = event.skipCount;
    // this.list(req, event.pageNo, undefined);
    console.log("user", event);
  }

  delete(role: RoleDto): void {
    abp.message.confirm(
      this.l("RoleDeleteWarningMessage", role.displayName),
      undefined,
      (result: boolean) => {
        if (result) {
          this._rolesService
            .delete(role.id)
            .pipe(
              finalize(() => {
                abp.notify.success(this.l("SuccessfullyDeleted"));
                this.refresh();
              })
            )
            .subscribe(() => {});
        }
      }
    );
  }

  createOrEditRole(id?: number): void {
    this.showCreateOrEditRoleDialog(id);
  }

  editRole(role: RoleDto): void {
    this.showCreateOrEditRoleDialog(role.id);
  }

  showCreateOrEditRoleDialog(id?: number): void {
    const dialogRef = this._dialog.open(CreateRoleDialogComponent, {
      data: { id: id },
    });

    dialogRef.afterClosed().subscribe((result) => {
      this.refresh();
      dialogRef.close();
    });
  }
}
