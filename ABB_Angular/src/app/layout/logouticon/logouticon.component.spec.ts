/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { LogouticonComponent } from './logouticon.component';

describe('LogouticonComponent', () => {
  let component: LogouticonComponent;
  let fixture: ComponentFixture<LogouticonComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LogouticonComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LogouticonComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
