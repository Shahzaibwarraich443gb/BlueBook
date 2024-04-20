import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LedgericonComponent } from './ledgericon.component';

describe('LedgericonComponent', () => {
  let component: LedgericonComponent;
  let fixture: ComponentFixture<LedgericonComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ LedgericonComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(LedgericonComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
