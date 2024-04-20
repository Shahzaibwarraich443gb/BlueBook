import { Component, Injector, ChangeDetectionStrategy } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';

@Component({
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
  animations: [appModuleAnimation()],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class HomeComponent extends AppComponentBase {

  //sidebar menu activation start
  menuSidebarActive: boolean = false;

  constructor(injector: Injector) {
    super(injector);
  }
  myfunction(){
    if(this.menuSidebarActive==false){
      this.menuSidebarActive=true;
    }
    else {
      this.menuSidebarActive=false;
    }
  }
  //sidebar menu activation end

 //short menu activation start
menuShortcutActive:boolean=false;
shortmenu(){
 if(this.menuShortcutActive==false){
   this.menuShortcutActive=true;
   this.emailShortcutActive=false;
   this.notifyShortcutActive=false;
   this.langShortcutActive=false;
   this.proShortcutActive=false;
 }
 else {
   this.menuShortcutActive=false;
 }
}
//short menu activation end

//short menu activation start
notifyShortcutActive:boolean=false;
notifydropdown(){
 if(this.notifyShortcutActive==false){
   this.menuShortcutActive=false;
   this.emailShortcutActive=false;
   this.notifyShortcutActive=true;
   this.langShortcutActive=false;
   this.proShortcutActive=false;
 }
 else {
   this.notifyShortcutActive=false;
 }
}
//short menu activation end

//short menu activation start
emailShortcutActive:boolean=false;
emaildropdown(){
 if(this.emailShortcutActive==false){
   this.menuShortcutActive=false;
   this.emailShortcutActive=true;
   this.notifyShortcutActive=false;
   this.langShortcutActive=false;
   this.proShortcutActive=false;
 }
 else {
   this.emailShortcutActive=false;

 }
}
//short menu activation end

//short menu activation start
langShortcutActive:boolean=false;
langdropdown(){
 if(this.langShortcutActive==false){
   this.menuShortcutActive=false;
   this.emailShortcutActive=false;
   this.notifyShortcutActive=false;
   this.langShortcutActive=true;
   this.proShortcutActive=false;
 }
 else {
   this.langShortcutActive=false;

 }
}
//short menu activation end

//short menu activation start
proShortcutActive:boolean=false;
prodropdown(){
 if(this.proShortcutActive==false){
   this.menuShortcutActive=false;
   this.emailShortcutActive=false;
   this.notifyShortcutActive=false;
   this.langShortcutActive=false;
   this.proShortcutActive=true;
 }
 else {
   this.proShortcutActive=false;

 }
}
}
