import { ComponentFixture, TestBed } from '@angular/core/testing';

import { QuickdropdownComponent } from './quickdropdown.component';

describe('QuickdropdownComponent', () => {
  let component: QuickdropdownComponent;
  let fixture: ComponentFixture<QuickdropdownComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ QuickdropdownComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(QuickdropdownComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
