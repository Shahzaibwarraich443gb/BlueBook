import {
  Component,
  Injector,
  OnInit,
  EventEmitter,
  Output,
  Inject,
  ViewEncapsulation
} from '@angular/core';
import { forEach as _forEach, map as _map } from 'lodash-es';
import { AppComponentBase } from '@shared/app-component-base';
import {
  UserServiceProxy,
  CreateOrEditUserDto,
  RoleDto,
  PermissionDtoListResultDto,
  RoleServiceProxy,
  Int64StringKeyValuePair
} from '@shared/service-proxies/service-proxies';
import { AbpValidationError } from '@shared/components/validation/abp-validation.api';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { PermissionDto } from './../../../shared/service-proxies/service-proxies';
import { parse } from 'path';
export interface DialogData {
  id?: number;
}

@Component({
  templateUrl: './create-user-dialog.component.html',
  styleUrls: ['./create-user-dialog.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class CreateUserDialogComponent extends AppComponentBase
  implements OnInit {
  saving = false;
  hide = true;
  hideConfirmPassword = true;
  user = new CreateOrEditUserDto();

  roles: Int64StringKeyValuePair[] = [];
  rolesCopy: Int64StringKeyValuePair[] = [];
  selectedRole: Int64StringKeyValuePair = new Int64StringKeyValuePair();

  checkedRolesMap: { [key: string]: boolean } = {};
  defaultRoleCheckedStatus = true;
  confirmPassword: string;
  isMatch: boolean = true;


  passwordValidationErrors: Partial<AbpValidationError>[] = [
    {
      name: 'pattern',
      localizationKey:
        'PasswordsMustBeAtLeast8CharactersContainLowercaseUppercaseNumber',
    },
  ];
  confirmPasswordValidationErrors: Partial<AbpValidationError>[] = [
    {
      name: 'validateEqual',
      localizationKey: 'PasswordsDoNotMatch',
    },
  ];

  @Output() onSave = new EventEmitter<any>();
  permissions: PermissionDto[] = [];
  checkedPermissionsMap: { [key: string]: boolean } = {};
  defaultPermissionCheckedStatus = true;

  constructor(
    injector: Injector,
    private _roleService: RoleServiceProxy,
    public _userService: UserServiceProxy,
    public dialogRef: MatDialogRef<CreateUserDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: DialogData,
  ) {
    super(injector);
  }

  filterRoles(event) {
    this.selectedRole = new Int64StringKeyValuePair();
    if (!event.filter) {
      this.roles = this.rolesCopy;
      return;
    }
    this._userService.getRoles(event.filter).subscribe((result2) => {
      this.roles = result2.items;
    });
  }

  ngOnInit(): void {
    this.user = new CreateOrEditUserDto();
    this.user.isActive = true;
    if (this.data != undefined && this.data.id != undefined) {
      this._userService.getUserForEdit(this.data.id).subscribe((result) => {
        this.user = result;
        this.user.password = '';
        this._userService.getRoles('').subscribe((result2) => {
          this.roles = result2.items;
          this.rolesCopy = result2.items;
        });
      });
    }
  }


  isRoleChecked(normalizedName: string): boolean {
    // just return default role checked status
    // it's better to use a setting
    return this.defaultRoleCheckedStatus;
  }

  onRoleChange(role: RoleDto, $event) {
    this.checkedRolesMap[role.normalizedName] = $event.target.checked;
  }

  getCheckedRoles(): string[] {
    const roles: string[] = [];
    _forEach(this.checkedRolesMap, function (value, key) {
      if (value) {
        roles.push(key);
      }
    });
    return roles;
  }

  getCheckedPermissions(): string[] {
    const permissions: string[] = [];
    _forEach(this.checkedPermissionsMap, function (value, key) {
      if (value) {
        permissions.push(key);
      }
    });
    return permissions;
  }

  save(): void {
    this.saving = true;

    this.user.userName = this.user.emailAddress;
    this.user.roleNames = [this.selectedRole.value];
    this.user.grantedPermissions = this.getCheckedPermissions();
    if (!this.data.id) {
      this._userService.create(this.user).subscribe(
        () => {
          this.dialogRef.close();
          this.notify.info(this.l('SavedSuccessfully'));
          this.onSave.emit();
        },
        () => {
          this.saving = false;
        }
      );
    } else {
      this._userService.updateUser(this.user).subscribe(
        () => {
          this.dialogRef.close();
          this.notify.info('User update successfully');
          this.onSave.emit();
        },
        () => {
          this.saving = false;
        }
      );
    }
  }

  match(password: string, confirmPassword: string) {
    if (password == confirmPassword) {
      this.isMatch = true;
      return;
    }
    this.isMatch = false;
  }

  hideDialog() {
    this.onSave.emit();
    this.dialogRef.close();
  }
}
