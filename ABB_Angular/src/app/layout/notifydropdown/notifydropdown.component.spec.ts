import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NotifydropdownComponent } from './notifydropdown.component';

describe('NotifydropdownComponent', () => {
  let component: NotifydropdownComponent;
  let fixture: ComponentFixture<NotifydropdownComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ NotifydropdownComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(NotifydropdownComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
