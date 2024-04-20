/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { SplinechartComponent } from './splinechart.component';

describe('SplinechartComponent', () => {
  let component: SplinechartComponent;
  let fixture: ComponentFixture<SplinechartComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SplinechartComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SplinechartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
