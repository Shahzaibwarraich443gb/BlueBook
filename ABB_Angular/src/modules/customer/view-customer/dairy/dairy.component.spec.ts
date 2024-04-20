/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { DairyComponent } from './dairy.component';

describe('DairyComponent', () => {
  let component: DairyComponent;
  let fixture: ComponentFixture<DairyComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DairyComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DairyComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
