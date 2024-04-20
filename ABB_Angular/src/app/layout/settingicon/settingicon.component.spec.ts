/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { SettingiconComponent } from './settingicon.component';

describe('SettingiconComponent', () => {
  let component: SettingiconComponent;
  let fixture: ComponentFixture<SettingiconComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SettingiconComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SettingiconComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
