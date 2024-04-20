import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NotifyiconComponent } from './notifyicon.component';

describe('NotifyiconComponent', () => {
  let component: NotifyiconComponent;
  let fixture: ComponentFixture<NotifyiconComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ NotifyiconComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(NotifyiconComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
