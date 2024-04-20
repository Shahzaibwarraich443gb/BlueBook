import { Component, Injector, OnInit, Renderer2 } from '@angular/core';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { AppComponentBase } from '@shared/app-component-base';
import { SignalRAspNetCoreHelper } from '@shared/helpers/SignalRAspNetCoreHelper';
import { LayoutStoreService } from '@shared/layout/layout-store.service';

@Component({
  templateUrl: './app.component.html'
})
export class AppComponent extends AppComponentBase implements OnInit {
  sidebarExpanded: boolean;
  //sidebar menu activation start
  menuSidebarActive: boolean = false;
  constructor(
    injector: Injector,
    private renderer: Renderer2,
    private _layoutStore: LayoutStoreService,
    private router: Router,
    private activatedRoute: ActivatedRoute
  ) {
    super(injector);
  }


  breadCrumbFunc(): void{
    // let breadCrumbArr = JSON.parse(localStorage.getItem('BreadCrumbData'));

    // let pathname = window.location.href.split('/').at(-1);

    // if(!breadCrumbArr || breadCrumbArr.length == 0 || !pathname || pathname.toLowerCase() == 'home'){
    //   return;
    // }


    let pathname = window.location.href.split('/').at(-1);
    
    if(/^-?\d+(\.\d+)?$/.test(pathname)) //is numeric
    {
      pathname = window.location.href.split('/')[window.location.href.split('/').length - 2]
    }

    if(pathname && pathname.toLowerCase() == 'home'){
        return;
    }
  

    document.getElementsByClassName('cashier-header-breadcrumb')[0].children[0].innerHTML = pathname.split('-').map(word => word.charAt(0).toUpperCase() + word.slice(1)).join(' ');

    // let bCrumbSub = document.getElementsByClassName('cashier-header-breadcrumb')[0].children[1];

    // (bCrumbSub as HTMLElement).style.display = 'flex';

    // bCrumbSub.innerHTML = '';

    // for(var i=0; i<breadCrumbArr.length; i++){
    //   bCrumbSub.innerHTML += `<li>${breadCrumbArr[i]}</li>`;

    //   if(i < breadCrumbArr.length - 1){
    //     bCrumbSub.innerHTML += `<li style='margin-left: 0.45rem; margin-right: 0.45rem'> > </li>`;
    //   }

    // }
  }

  breadCrumProc(): void{
    this.router.events.subscribe((val) => {
      if(val instanceof NavigationEnd){
        this.breadCrumbFunc();
      }
  });

  }

  ngOnInit(): void {
    this.breadCrumProc();
    this.renderer.addClass(document.body, 'sidebar-mini');

    SignalRAspNetCoreHelper.initSignalR();

    abp.event.on('abp.notifications.received', (userNotification) => {
      abp.notifications.showUiNotifyForUserNotification(userNotification);
      // Desktop notification
      Push.create('AbpZeroTemplate', {
        body: userNotification.notification.data.message,
        icon: abp.appPath + 'assets/app-logo-small.png',
        timeout: 6000,
        onClick: function () {
          window.focus();
          this.close();
        }
      });
    });

    this._layoutStore.sidebarExpanded.subscribe((value) => {
      this.sidebarExpanded = value;
    });
  }

  ngAfterViewInit(): void{
    this.breadCrumbFunc();
  }

  toggleSidebar(): void {
    this._layoutStore.setSidebarExpanded(!this.sidebarExpanded);
  }
  // Them type script

  myfunction() {
    if (this.menuSidebarActive == false) {
      this.menuSidebarActive = true;
    }
    else {
      this.menuSidebarActive = false;
    }
  }
  //sidebar menu activation end

  //short menu activation start
  menuShortcutActive: boolean = false;
  shortmenu() {
    if (this.menuShortcutActive == false) {
      this.menuShortcutActive = true;
      this.emailShortcutActive = false;
      this.notifyShortcutActive = false;
      this.langShortcutActive = false;
      this.proShortcutActive = false;
    }
    else {
      this.menuShortcutActive = false;
    }
  }
  //short menu activation end

  //short menu activation start
  notifyShortcutActive: boolean = false;
  notifydropdown() {
    if (this.notifyShortcutActive == false) {
      this.menuShortcutActive = false;
      this.emailShortcutActive = false;
      this.notifyShortcutActive = true;
      this.langShortcutActive = false;
      this.proShortcutActive = false;
    }
    else {
      this.notifyShortcutActive = false;
    }
  }
  //short menu activation end

  //short menu activation start
  emailShortcutActive: boolean = false;
  emaildropdown() {
    if (this.emailShortcutActive == false) {
      this.menuShortcutActive = false;
      this.emailShortcutActive = true;
      this.notifyShortcutActive = false;
      this.langShortcutActive = false;
      this.proShortcutActive = false;
    }
    else {
      this.emailShortcutActive = false;

    }
  }
  //short menu activation end

  //short menu activation start
  langShortcutActive: boolean = false;
  langdropdown() {
    if (this.langShortcutActive == false) {
      this.menuShortcutActive = false;
      this.emailShortcutActive = false;
      this.notifyShortcutActive = false;
      this.langShortcutActive = true;
      this.proShortcutActive = false;
    }
    else {
      this.langShortcutActive = false;

    }
  }
  //short menu activation end

  //short menu activation start
  proShortcutActive: boolean = false;
  prodropdown() {
    if (this.proShortcutActive == false) {
      this.menuShortcutActive = false;
      this.emailShortcutActive = false;
      this.notifyShortcutActive = false;
      this.langShortcutActive = false;
      this.proShortcutActive = true;
    }
    else {
      this.proShortcutActive = false;

    }
  }
}
