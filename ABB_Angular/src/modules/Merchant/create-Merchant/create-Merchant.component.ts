
import { ValidationService } from '../../../shared/Services/validation.service';
import {
  Component,
  EventEmitter,
  Inject,
  Injector,
  OnInit,
  Output,
  ViewChild,
  ElementRef
} from "@angular/core";
import {
  MatDialog,
  MatDialogRef,
  MAT_DIALOG_DATA,

} from "@angular/material/dialog";
import { appModuleAnimation } from "@shared/animations/routerTransition";
import { AppComponentBase } from "@shared/app-component-base";
import { CreateOrEditGeneralPaymentMethodInputDto, MerchantServiceProxy, PaymentMethodServiceProxy,CreateOrEditMerchantInputDto, GeneralMerchantDto } from '@shared/service-proxies/service-proxies';
import { AppSessionService } from "@shared/session/app-session.service";
import { finalize } from "rxjs";

export interface DialogData {
  id: number;
}

@Component({
  selector: "app-create-Merchant",
  templateUrl:"./create-Merchant.component.html",
  styleUrls: ["./create-Merchant.component.scss"],
  animations: [appModuleAnimation()],
})
export class CreateMerchantComponent
  extends AppComponentBase
  implements OnInit {
  @ViewChild('paymentMethodInput') paymentMethodInput1: ElementRef;

  @Output() onSave = new EventEmitter<any>();
  // createoreditInputTypedto = new CreateOrEditGeneralPaymentMethodInputDto();
  createoreditMerchantDto=new CreateOrEditMerchantInputDto();
  merchantList=[{
    name: "Card Connect", value: "Card Connect"
  },
{
  name: "Pay Easy", value: "Pay Easy"
}]
  selectedMerchant=[{
    name: "Card Connect", value: "Card Connect"
  },];
  constructor(
    injector: Injector,
    public dialog: MatDialog,
    public dialogRef: MatDialogRef<any>,
    
    public _MerchantService:MerchantServiceProxy,

    @Inject(MAT_DIALOG_DATA) public data: DialogData,
    private _sessionService: AppSessionService,
    public validation: ValidationService
  ) {
    super(injector);
  }

  ngOnInit() {
    if (this.data.id) {
      if (this.data.id) {
      this.getMerchantOnId(this.data.id)
      } else
        this.createoreditMerchantDto =
          new CreateOrEditMerchantInputDto();
        this.createoreditMerchantDto.name= this.merchantList.find(x=>x.name=="CardConnect").name;
    }


  
    
  }
  onMerchantChange(event)
    {
     
      if(event.value)
      {
        this._MerchantService.checkMerchantExistOrNot(event.value).subscribe(x=>{
        
          this.createoreditMerchantDto= x;
         
          if(this.createoreditMerchantDto && this.createoreditMerchantDto.id==undefined)
          {
            this.createoreditMerchantDto.name= this.merchantList.find(x=>x.value==event.value).name;
          }

        })
      }
    }
  ngAfterViewInit() {
    if(this.paymentMethodInput1!== undefined){
      this.paymentMethodInput1.nativeElement.focus();
    }
  }
  
  getMerchantOnId(id: number)
  {
    this._MerchantService
    .get(this.data.id)
    .pipe(finalize(() => { }))
    .subscribe((result) => {
      this.createoreditMerchantDto = result;
    });
  }
  save() {
    this._MerchantService
      .createOrEdit(this.createoreditMerchantDto)
      .subscribe((arg) => {
        abp.notify.success('save Succesfully')
        this.dialogRef.close();
        this.onSave.emit();
      });
  }

  hideDialog() {
    this.onSave.emit();
    this.dialogRef.close();
  }

  numberOnly(event) {
    return this.validation.numberOnlyWith(event);
  }

  letterOnly(event) {
    return this.validation.letterOnlyWithSpaceAllowed(event);
  }

  firstSpaceNotAllowed(event) {
    this.validation.letterOnlyWithSpaceAllowed(event);
  }
}
