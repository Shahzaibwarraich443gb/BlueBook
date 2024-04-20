import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PaymenticonComponent } from './paymenticon.component';

describe('PaymenticonComponent', () => {
  let component: PaymenticonComponent;
  let fixture: ComponentFixture<PaymenticonComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PaymenticonComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PaymenticonComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
