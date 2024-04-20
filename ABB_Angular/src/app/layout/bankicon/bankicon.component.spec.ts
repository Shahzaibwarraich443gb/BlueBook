import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BankiconComponent } from './bankicon.component';

describe('BankiconComponent', () => {
  let component: BankiconComponent;
  let fixture: ComponentFixture<BankiconComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ BankiconComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(BankiconComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
