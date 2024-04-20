import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BalancealertComponent } from './balancealert.component';

describe('BalancealertComponent', () => {
  let component: BalancealertComponent;
  let fixture: ComponentFixture<BalancealertComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ BalancealertComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(BalancealertComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
