import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VouchericonComponent } from './vouchericon.component';

describe('VouchericonComponent', () => {
  let component: VouchericonComponent;
  let fixture: ComponentFixture<VouchericonComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ VouchericonComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(VouchericonComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
