import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatMenuModule } from '@angular/material/menu';
import { MatRadioModule } from '@angular/material/radio';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatTabsModule } from '@angular/material/tabs';
import { MatSelectModule } from '@angular/material/select';
import { MatSliderModule } from '@angular/material/slider';
import { MatTableModule } from '@angular/material/table'
import { SwiperModule } from 'swiper/angular';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MetismenuAngularModule } from '@metismenu/angular';
import { NgxSliderModule } from '@angular-slider/ngx-slider';
import { HttpClientJsonpModule, HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { NgApexchartsModule } from 'ng-apexcharts';
import { ClipboardModule } from 'ngx-clipboard';
import { CollapseModule } from 'ngx-bootstrap/collapse';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { NgxPaginationModule } from 'ngx-pagination';
import { SharedModule } from '@shared/shared.module';

@NgModule({
  imports: [
  CollapseModule,
    TabsModule,
    NgxPaginationModule,
    RouterModule,
    NgxSliderModule,
    ClipboardModule,
    ReactiveFormsModule,
    NgApexchartsModule,
    CommonModule,
    HttpClientJsonpModule,
    HttpClientModule,
    MetismenuAngularModule,
    FormsModule,
    SwiperModule,
    MatTableModule,
    MatSelectModule,
    MatSliderModule,
    MatIconModule,
    MatCheckboxModule,
    MatFormFieldModule,
    MatDialogModule,
    MatButtonModule,
    MatInputModule,
    MatMenuModule,
    MatRadioModule,
    MatPaginatorModule,
    MatProgressBarModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatTabsModule,
    SharedModule
  ],
  exports: [
    SharedModule,
    CollapseModule,
    TabsModule,
    NgxPaginationModule,
    RouterModule,
    NgxSliderModule,
    ClipboardModule,
    ReactiveFormsModule,
    NgApexchartsModule,
    CommonModule,
    HttpClientJsonpModule,
    HttpClientModule,
    MetismenuAngularModule,
    FormsModule,
    SwiperModule,
    MatTableModule,
    MatSelectModule,
    MatSliderModule,
    MatIconModule,
    MatCheckboxModule,
    MatFormFieldModule,
    MatDialogModule,
    MatButtonModule,
    MatInputModule,
    MatMenuModule,
    MatRadioModule,
    MatPaginatorModule,
    MatProgressBarModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatTabsModule
  ],
  declarations: []
})
export class ThemeModules { }