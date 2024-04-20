import { Component, Injector, OnInit } from '@angular/core';
import { finalize } from 'rxjs/operators';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import {
  PagedListingComponentBase,
  PagedRequestDto
} from 'shared/paged-listing-component-base';
import {
  UserServiceProxy,
  UserDto,
  UserDtoPagedResultDto
} from '@shared/service-proxies/service-proxies';
import { CreateUserDialogComponent } from './create-user/create-user-dialog.component';
import { ThemePalette } from '@angular/material/core';
import { MatDialog } from '@angular/material/dialog';

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
  selector: 'app-user',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.html'],
  animations: [appModuleAnimation()]
})

export class UsersComponent extends PagedListingComponentBase<UserDto> implements OnInit {
  users: UserDto[] = [];
  keyword = '';
  isActive: boolean | null;

  totalRecords = 0;

  advancedFiltersVisible = false;
  displayedColumns: string[] = ['name', 'userName', 'emailAddress', 'status', 'actions'];
  //checkbox start
  task: Task = {
    name: '',
    completed: false,
    color: 'primary',

  };
  isComplete: boolean = false;
  allComplete: boolean = false;
  oneComplete: boolean = false;
  twoComplete: boolean = false;
  threeComplete: boolean = false;
  fourComplete: boolean = false;
  fiveComplete: boolean = false;
  sixComplete: boolean = false;
  sevenComplete: boolean = false;
  eightComplete: boolean = false;
  nineComplete: boolean = false;
  tenComplete: boolean = false;
  elevenComplete: boolean = false;
  twelveComplete: boolean = false;
  thirteenComplete: boolean = false;
  fourteenComplete: boolean = false;
  fifteenComplete: boolean = false;
  sixteenComplete: boolean = false;
  disabled_condition = true;
  constructor(
    injector: Injector,
    private _userService: UserServiceProxy,
    public _dialog: MatDialog
  ) {
    super(injector);
  }

  // theme default typescript start

  someComplete(): boolean {
    if (this.task.subtasks == null) {
      return false;
    }
    return this.task.subtasks.filter(t => t.completed).length > 0 && !this.allComplete;
  }

  ngOnInit(): void {
    this.getDataPage(this.pageNumber);
  }

  // theme default typescript end

  createUser(id?: number): void {
    this.showCreateOrEditUserDialog(id);
  }

  editUser(user: UserDto): void {
    this.showCreateOrEditUserDialog(user.id);
  }

  public resetPassword(user: UserDto): void {
    this.showResetPasswordUserDialog(user.id);
  }

  protected list(
    request: PagedUsersRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    request.keyword = this.keyword;
    request.isActive = this.isActive;

    this._userService
      .getAll(
        request.keyword,
        request.isActive,
        request.skipCount,
        request.maxResultCount
      )
      .pipe(
        finalize(() => {
          finishedCallback();
        })
      )
      .subscribe((result: UserDtoPagedResultDto) => {
        this.users = result.items;
        this.totalRecords = result.totalCount;
        abp.event.trigger(this.paginationEvent, result.totalCount);
        // this.showPaging(result, pageNumber);
      });
  }

  updateRows(event) {
    let req = new PagedUsersRequestDto();
    req.maxResultCount = event.maxResultCount;
    req.skipCount = event.skipCount;
    this.list(req, event.pageNo, undefined)
    console.log('user', event);
  }

  protected delete(user: UserDto): void {
    abp.message.confirm(
      this.l('UserDeleteWarningMessage', user.fullName),
      undefined,
      (result: boolean) => {
        if (result) {
          this._userService.delete(user.id).subscribe(() => {
            abp.notify.success(this.l('SuccessfullyDeleted'));
            this.refresh();
          });
        }
      }
    );
  }

  private showResetPasswordUserDialog(id?: number): void {
    // this._modalService.show(ResetPasswordDialogComponent, {
    //   class: 'modal-lg',
    //   initialState: {
    //     id: id,
    //   },
    // });
  }

  private showCreateOrEditUserDialog(id?: number): void {
    const dialogRef = this._dialog.open(CreateUserDialogComponent, {
      data: { id: id }
    });

    dialogRef.afterClosed().subscribe(result => {
      this.refresh();
      dialogRef.close();
    });
  }
}
