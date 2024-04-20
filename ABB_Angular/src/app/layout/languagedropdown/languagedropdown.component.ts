import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppAuthService } from '@shared/auth/app-auth.service';

@Component({
  selector: 'app-languagedropdown',
  templateUrl: './languagedropdown.component.html',
  styleUrls: ['./languagedropdown.component.scss'],
  animations: [appModuleAnimation()],
})
export class LanguagedropdownComponent implements OnInit {

  constructor(
    private _appAuthService: AppAuthService,
    private router: Router
  ) { }

  ngOnInit() {
  }

  logout() {
    this._appAuthService.logout();
  }
  navigate()
  {
    this.router.navigateByUrl(
      "app/account-settings/profile"
    );
  }
}
