import {
  ChangeDetectorRef,
  Component,
  Injector,
  Input,
  OnInit,
  ViewChild,
} from "@angular/core";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { AppComponentBase } from "@shared/app-component-base";
import { AppSessionService } from "@shared/session/app-session.service";

import { BsDatepickerConfig } from "ngx-bootstrap/datepicker";
import { PersonalInformationComponent } from "../personal-information/personal-information.component";
import { BillingInformationComponent } from "../biling-information/billing-information.component";
import { ChangePasswordComponent } from "../change-password/change-password.component";
import { CompanyDetailsComponent } from "../Company-Details/Company-Details.component";
import { ContactDetailsServiceProxy, CreateOrEditUserDto, User, UserDto } from "@shared/service-proxies/service-proxies";
import { result } from "lodash-es";

@Component({
  selector: "app-my-profile",
  templateUrl: "./my-profile.component.html",
  styleUrls: ["./my-profile.component.scss"],
})
export class MyProfileComponent extends AppComponentBase implements OnInit {


  users = [
    { name: 'User 1', logic: 'green' },
    { name: 'User 2', logic: 'red' },
    { name: 'User 3', logic: 'green' },
    { name: 'User 4', logic: 'green' },
    { name: 'User 5', logic: 'red' },
    { name: 'User 6', logic: 'red' },
    // Add more user data as needed
  ];
  @ViewChild("personalInformationModel", { static: true })
  personalInformationModel: PersonalInformationComponent;
  @ViewChild("billingInformationModel", { static: true })
  billingInformationModel: BillingInformationComponent;
  @ViewChild("changePasswordModal", { static: true })
  changePasswordModal: ChangePasswordComponent;
  @ViewChild("companyDetailModal", { static: true })
  companyDetailModal: CompanyDetailsComponent;
  bsConfig: Partial<BsDatepickerConfig>;
  Data: User[] = [];
  isLoading = false;
  selected = "personalInformation";
  validSociaSecurityNo: boolean = false;
  adminUser: UserDto;
  constructor(
    injector: Injector,
    public _appSessionService: AppSessionService,
    public _ContactDetails: ContactDetailsServiceProxy,
    private _cdr: ChangeDetectorRef
  ) {
    super(injector);
  }
  async ngOnInit() {
    await this.getAdminUser();
    await this.selectedCard("personalInformation");
  }
  ngAfterViewInit(): void {
    this._cdr.detectChanges();
  }


  getAdminUser() {

    this._ContactDetails.getAdminUser().subscribe(result => {
      this.adminUser = result
    })
  }
  selectedCard(card: string) {

    abp.event.trigger("onProfileInformationChanges");
    let selectedCard = card;
    if (selectedCard == "personalInformation") {
      this.selected = selectedCard;
      this.personalInformationModel.showScreen(true);
    }
    if (selectedCard == "billingInformation") {
      this.selected = selectedCard;
      this.billingInformationModel.showScreen(true);
    }
    if (selectedCard == "changePassword") {
      this.selected = selectedCard;
      this.changePasswordModal.showScreen(true);
    }
    if (selectedCard == "companyDetail") {
      this.selected = selectedCard;
      this.companyDetailModal.showScreen(true);
    }
  }
}
