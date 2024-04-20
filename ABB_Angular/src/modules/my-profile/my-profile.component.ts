import {
  ChangeDetectorRef,
  Component,
  Injector,
  Input,
  OnInit,
  ViewChild,
} from "@angular/core";
import { FormControl, FormGroup, Validators } from "@angular/forms";
//import { AppComponentBase } from "@shared/app-component-base";
import { AppSessionService } from "../../shared/Services/validation.service
import { AppComponentBase } from "../../shared/app-component-base";
import { BsDatepickerConfig } from "ngx-bootstrap/datepicker";
//import { PersonalInformationComponent } from "../personal-information/personal-information.component";
import { BillingInformationComponent } from "./biling-information/billing-information.component";
import { ChangePasswordComponent } from "./change-password/change-password.component";
import { PersonalInformationComponent } from "./personal-information/personal-information.component";
@Component({
  selector: "app-my-profile",
  templateUrl: "./my-profile.component.html",
  styleUrls: ["./my-profile.component.scss"],
})
export class MyProfileComponent extends  AppComponentBase implements OnInit {
  @ViewChild("personalInformationModel", { static: true })
  personalInformationModel: PersonalInformationComponent;
  @ViewChild("billingInformationModel", { static: true })
  billingInformationModel: BillingInformationComponent;
  @ViewChild("changePasswordModal", { static: true })
  changePasswordModal: ChangePasswordComponent;
  bsConfig: Partial<BsDatepickerConfig>;
  isLoading = false;
  selected = "personalInformation";
  validSociaSecurityNo: boolean = false;
  constructor(
    injector: Injector,
    //public _appSessionService: AppSessionService,
    private _cdr: ChangeDetectorRef
  ) {
    super(injector);
  }
  async ngOnInit() {
    await this.selectedCard("personalInformation");
  }
  ngAfterViewInit(): void {
    this._cdr.detectChanges();
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
  }
}
