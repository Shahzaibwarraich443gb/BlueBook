import {
  Component,
  Injector,
  OnInit,
  EventEmitter,
  Output,
  Inject,
  ViewEncapsulation,
  ViewChild,
} from "@angular/core";
import { BsModalRef } from "ngx-bootstrap/modal";
import { AppComponentBase } from "@shared/app-component-base";
import {
  RoleServiceProxy,
  RoleDto,
  PermissionDto,
  CreateRoleDto,
  PermissionDtoListResultDto,
  FlatPermissionDto,
  RoleEditDto,
} from "@shared/service-proxies/service-proxies";
import { forEach as _forEach, map as _map } from "lodash-es";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { ArrayToTreeConverterService } from "@shared/Services/array-to-tree-converter.service";
import { TreeDataHelperService } from "@shared/Services/tree-data-helper.service";
import { PermissionTreeComponent } from "../permission-tree.component";
import { NgxSpinnerService } from "ngx-spinner";
import { COMMA, ENTER, SPACE } from "@angular/cdk/keycodes";
import { MatChipInputEvent } from "@angular/material/chips";
export interface DialogData {
  id?: number;
}
@Component({
  templateUrl: "create-role-dialog.component.html",
  styleUrls: ["create-role-dialog.component.scss"],
  encapsulation: ViewEncapsulation.None,
})
export class CreateRoleDialogComponent
  extends AppComponentBase
  implements OnInit {
  saving = false;
  role: RoleDto = new RoleDto();
  //role: RoleEditDto = new RoleEditDto();
  permissions: PermissionDto[] = [];
  checkedPermissionsMap: { [key: string]: boolean } = {};
  defaultPermissionCheckedStatus = true;

  @Output() onSave = new EventEmitter<any>();

  constructor(
    injector: Injector,
    private _arrayToTreeConverterService: ArrayToTreeConverterService,
    private _treeDataHelperService: TreeDataHelperService,
    private _roleService: RoleServiceProxy,
    public dialogRef: MatDialogRef<CreateRoleDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: DialogData,
    public bsModalRef: BsModalRef,
    private spinner: NgxSpinnerService
  ) {
    super(injector);
  }
  emailList: string[] = [];
  addOnBlur = true;
  readonly separatorKeysCodes = [ENTER, COMMA, SPACE] as const;
  // permissionTree: PermissionTreeComponent;

  @ViewChild(PermissionTreeComponent) permissionTree: PermissionTreeComponent;

  ngOnInit(): void {
    this.loadAllPermissionsToFilterTree();
    if (this.data != undefined && this.data.id != undefined) {
      // this._roleService.get(this.data.id).subscribe((result) => {
      //   this.role = result;
      // });

      this._roleService.getRoleForEdit(this.data.id).subscribe((result) => {
        this.permissionTree.editData = result;
        this.role = result.role;
        if (result.role.ipAddress) {
          const resultList: string[] = result.role.ipAddress.split(",");
          resultList.forEach(element => {
            this.addIpAddress({ value: element } as MatChipInputEvent, 1);
          });
        }
      });

    }

    this._roleService
      .getAllPermissions()
      .subscribe((result: PermissionDtoListResultDto) => {
        this.permissions = result.items;
        // _forEach(this.role.grantedPermissions, function (key) {
        //   this.checkedPermissionsMap.push({ key: key, value: true });
        // });
        // this.setInitialPermissionsStatus();
      });
  }

  setInitialPermissionsStatus(): void {
    _map(this.permissions, (item) => {
      this.checkedPermissionsMap[item.name] = this.isPermissionChecked(
        item.name
      );
    });
  }

  hideDialog() {
    this.onSave.emit();
    this.dialogRef.close();
  }

  addIpAddress(event: MatChipInputEvent, bit: number): void {
    if (event && !undefined) {
      const value = event.value;
      if (value && bit === 0) {
        this.emailList.push(value);
      }
      else if (value && bit === 1) {
        this.emailList.push(value);
      }
      if (event.chipInput) {
        event.chipInput!.clear();
      }
    }
  }

  removeEmail(item: any): void {
    const index = this.emailList.indexOf(item);
    if (index >= 0) {
      this.emailList.splice(index, 1);
    }
  }
  isPermissionChecked(permissionName: string): boolean {
    // just return default permission checked status
    // it's better to use a setting
    if (!this.data.id) return this.defaultPermissionCheckedStatus;
    if (this.data.id) return false;
  }

  onPermissionChange(permission: PermissionDto, $event) {
    this.checkedPermissionsMap[permission.name] = $event.target.checked;
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

  loadAllPermissionsToFilterTree() {
    this.spinner.show();
    let treeModel: FlatPermissionDto[] = [];
    this._roleService.getAllPermissionsWithLevel().subscribe((result) => {
      if (result.items) {
        result.items.forEach((item) => {
          treeModel.push(
            new FlatPermissionDto({
              name: item.name,
              description: item.description,
              displayName: item.displayName,
              isGrantedByDefault: item.isGrantedByDefault,
              parentName: item.parentName,
            })
          );
        });
      }

      this.permissionTree.editData = {
        permissions: treeModel,
        grantedPermissionNames: [],
      };
      this.spinner.hide();
    });
  }

  save(): void {
    const self = this;
    this.saving = true;
    const role = new RoleDto();
    role.name = this.role.name;
    role.displayName = this.role.name;
    role.description = this.role.description;
    role.ipAddress = this.emailList.join(",");
    role.ipRestriction = this.role.ipRestriction;
    role.normalizedName = role.name.toUpperCase();
    role.id = this.role.id;
    if (role.name.toLocaleLowerCase() == "admin") {
      role.isDefault = true;
    }
    role.grantedPermissions = self.permissionTree.getGrantedPermissionNames();
    // if (abp.session.tenantId) {
    //   role.grantedPermissions.unshift("Pages.Tenant.Dashboard");
    // } else {
    //   role.grantedPermissions.unshift("Pages.Administration.HostDashboard");
    // }
    if (!this.data.id) {
      this._roleService.createNew(role).subscribe(
        () => {
          this.notify.info(this.l("SavedSuccessfully"));
          this.dialogRef.close();
          this.onSave.emit();
        },
        () => {
          this.saving = false;
        }
      );
    } else {
      this._roleService.updateNew(role).subscribe(
        () => {
          this.dialogRef.close();
          this.notify.info("role updated successfully");
          this.onSave.emit();
        },
        () => {
          this.saving = false;
        }
      );
    }
  }
}
