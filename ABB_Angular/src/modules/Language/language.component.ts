
import {
  ContactPersonTypeDto,
  EntityDto,
  ContactPersonTypeServiceProxy,
  LanguageServiceProxy,
  LanguageDto,
} from "../../shared/service-proxies/service-proxies";
import { Component, Injector, OnInit } from "@angular/core";
import { ThemePalette } from "@angular/material/core";
import { MatDialog } from "@angular/material/dialog";
import { PagedRequestDto } from "@shared/paged-listing-component-base";
import {
  AccountNature,
  ChartOfAccountDto,
} from "@shared/service-proxies/service-proxies";
import { AppComponentBase } from "shared/app-component-base";
import { CreateChartOfAccountComponent } from "modules/chart-of-account/create-chart-of-account/create-chart-of-account.component";
import { CreatelanguageComponent } from "./create-language/create-language.component";
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
  selector: "app-language",
  templateUrl: "./language.component.html",
  styleUrls: ["./language.component.css"],
})
export class languageComponent extends AppComponentBase implements OnInit {
  totalRecords = 0;
  languageDto: LanguageDto[] = [];
  allComplete: boolean = false;
  task: Task = {
    name: "",
    completed: false,
    color: "primary",
  };

  keyword = "";
  isActive: boolean | null;

  someComplete(): boolean {
    if (this.task.subtasks == null) {
      return false;
    }
    return (
      this.task.subtasks.filter((t) => t.completed).length > 0 &&
      !this.allComplete
    );
  }

  updateRows(event) {
    let req = new PagedUsersRequestDto();
    req.maxResultCount = event.maxResultCount;
    req.skipCount = event.skipCount;
  }

  constructor(
    injector: Injector,
    public _dialog: MatDialog,
    public _languageServiceProxy: LanguageServiceProxy
  ) {
    super(injector);
  }

  ngOnInit() {
    this.list();
  }

  protected list() {
    this._languageServiceProxy
      .getAll()
      .subscribe((arg) => (this.languageDto = arg));
  }

  protected delete(languageDto: ContactPersonTypeDto): void {
    abp.message.confirm(
      this.l("Are you sure to delete language"),
      undefined,
      (result: boolean) => {
        if (result) {
          this._languageServiceProxy.delete(languageDto.id).subscribe(() => {
            abp.notify.success(this.l("SuccessfullyDeleted"));
            this.list();
          });
        }
      }
    );
  }

  search(event: any) {
    const val = event.target.value.toLowerCase();
    if (val == "") {
      this.list();
    } else {
      const temp = this.languageDto.filter(function (d) {
        return d.name.toLowerCase().indexOf(val) !== -1;
      });
      this.languageDto = temp;
    }
  }

  editLanguage(languageDto: LanguageDto): void {
    this.showCreateOrEditLanguageDialog(languageDto.id);
  }
  
  createLanguage(id?: number): void {
    this.showCreateOrEditLanguageDialog(id);
  }

  private showCreateOrEditLanguageDialog(id?: number): void {
    const dialogRef = this._dialog.open(CreatelanguageComponent, {
      data: { id: id },
    }); 
    dialogRef.afterClosed().subscribe((result) => {
      dialogRef.close();
      this.list();
    });
  }
}
