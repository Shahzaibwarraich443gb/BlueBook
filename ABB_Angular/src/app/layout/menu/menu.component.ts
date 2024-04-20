import { P } from '@angular/cdk/keycodes';
import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { PermissionCheckerService } from 'abp-ng2-module';
@Component({
  selector: 'app-menu',
  templateUrl: './menu.component.html',
  // template: ` 
  //  <div style="margin: 10px 5px 0px 10px;">
  // <ul class="cashier-menu px-0.5 menuContainer">
  //   <li
  //     app-menuitem
  //     *ngFor="let item of model; let i = index"
  //     [item]="item"
  //     [index]="i"
  //     [root]="true"
  //   ></li>

  // </ul>
  // </div>`,
  styleUrls: ['./menu.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class MenuComponent implements OnInit {

  userName: string = "";
  userImg: any = null;
  //short menu activation start
  proShortcutActive: boolean = false;
  model: any[];
  prodropdown() {
    if (this.proShortcutActive == false) {
      this.proShortcutActive = true;
    }
    else {
      this.proShortcutActive = false;

    }
  }
  //short menu activation end

  constructor(private _permissionChecker: PermissionCheckerService) { }

  ngOnInit(): void {
    this.userName = JSON.parse(localStorage.getItem('user')).name;
  }

  breadCrumbChainArr = [];

  isGranted(permission: string): boolean {
    return this._permissionChecker.isGranted(permission)
  }
  
  addBreadCrumbParent(element): void {
    if (!element) {
      return;
    }

    if (element.children.length > 0) {
      element = element.children[0];
    }

    if (element.tagName.toLower == 'a') {
      this.breadCrumbChainArr.push(element.textContent);
    }

    if (!element.parentNode) {
      return;
    }

    this.addBreadCrumbParent(element.parentNode);

  }

  breadCrumbChain = [];

  updateBreadcrumb(arr): void {

    localStorage.setItem('BreadCrumbData', JSON.stringify(arr));



  }

}
