import { CommonModule } from '@angular/common';
import { NgModule, ModuleWithProviders } from '@angular/core';
import { RouterModule } from '@angular/router';
import { NgxPaginationModule } from 'ngx-pagination';

import { AppSessionService } from './session/app-session.service';
import { AppUrlService } from './nav/app-url.service';
import { AppAuthService } from './auth/app-auth.service';
import { AppRouteGuard } from './auth/auth-route-guard';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { AbpPaginationControlsComponent } from './components/pagination/abp-pagination-controls.component';
import { AbpValidationSummaryComponent } from './components/validation/abp-validation.summary.component';
import { AbpModalHeaderComponent } from './components/modal/abp-modal-header.component';
import { AbpModalFooterComponent } from './components/modal/abp-modal-footer.component';
import { LayoutStoreService } from './layout/layout-store.service';

import { BusyDirective } from './directives/busy.directive';
import { EqualValidator } from './directives/equal-validator.directive';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';

// Services
import { ValidationService } from './Services/validation.service';
import { ArrayToTreeConverterService } from './Services/array-to-tree-converter.service';
import { TreeDataHelperService } from './Services/tree-data-helper.service';

// Materials
import { PaginationComponent } from './pagination/pagination.component'
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule } from '@angular/material/dialog';
import { TextMaskModule } from 'angular2-text-mask';
import { MatChipsModule } from '@angular/material/chips';
import { NgxSpinnerModule } from 'ngx-spinner';
import { MatCheckboxModule } from '@angular/material/checkbox';
// import { NgxPrintElementModule } from 'ngx-print-element';
import { MatSortModule } from '@angular/material/sort';
import { MatTooltipModule } from '@angular/material/tooltip';
import {DecimalLimitDirective} from './directives/DecimalLimit.directive'

@NgModule({
    imports: [
        CommonModule,
        RouterModule,
        NgxPaginationModule,
        MatIconModule,
        MatFormFieldModule,
        MatSelectModule,
        TextMaskModule,
        MatChipsModule,
        NgxSpinnerModule,
        // NgxPrintElementModule,
        MatCheckboxModule,
    ],
    declarations: [
        AbpPaginationControlsComponent,
        AbpValidationSummaryComponent,
        AbpModalHeaderComponent,
        AbpModalFooterComponent,
        PaginationComponent,
        LocalizePipe,
        BusyDirective,
        EqualValidator,
        DecimalLimitDirective
    ],
    exports: [
        AbpPaginationControlsComponent,
        AbpValidationSummaryComponent,
        AbpModalHeaderComponent,
        AbpModalFooterComponent,
        PaginationComponent,
        LocalizePipe,
        MatSortModule,
        BusyDirective,
        EqualValidator,
        MatButtonModule, MatDialogModule, DragDropModule,
        MatChipsModule,
        TextMaskModule,
        NgxSpinnerModule,
        MatIconModule,
        MatTooltipModule,
        DecimalLimitDirective
    ]
})
export class SharedModule {
    static forRoot(): ModuleWithProviders<SharedModule> {
        return {
            ngModule: SharedModule,
            providers: [
                AppSessionService,
                AppUrlService,
                AppAuthService,
                AppRouteGuard,
                LayoutStoreService,
                ValidationService,
                ArrayToTreeConverterService,
                TreeDataHelperService
            ],
        };
    }
}
