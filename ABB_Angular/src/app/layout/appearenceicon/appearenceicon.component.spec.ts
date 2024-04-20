/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { AppearenceiconComponent } from './appearenceicon.component';

describe('AppearenceiconComponent', () => {
  let component: AppearenceiconComponent;
  let fixture: ComponentFixture<AppearenceiconComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AppearenceiconComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AppearenceiconComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
