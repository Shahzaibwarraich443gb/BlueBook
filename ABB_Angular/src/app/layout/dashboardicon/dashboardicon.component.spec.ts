import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DashboardiconComponent } from './dashboardicon.component';

describe('DashboardiconComponent', () => {
  let component: DashboardiconComponent;
  let fixture: ComponentFixture<DashboardiconComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DashboardiconComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DashboardiconComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
