import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RecentclientsComponent } from './recentclients.component';

describe('RecentclientsComponent', () => {
  let component: RecentclientsComponent;
  let fixture: ComponentFixture<RecentclientsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ RecentclientsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RecentclientsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
