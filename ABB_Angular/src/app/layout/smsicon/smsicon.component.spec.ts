import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SmsiconComponent } from './smsicon.component';

describe('SmsiconComponent', () => {
  let component: SmsiconComponent;
  let fixture: ComponentFixture<SmsiconComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SmsiconComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SmsiconComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
