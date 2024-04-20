import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RecentvoucherComponent } from './recentvoucher.component';

describe('RecentvoucherComponent', () => {
  let component: RecentvoucherComponent;
  let fixture: ComponentFixture<RecentvoucherComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ RecentvoucherComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RecentvoucherComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
