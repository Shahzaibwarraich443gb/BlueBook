import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BilliconComponent } from './billicon.component';

describe('BilliconComponent', () => {
  let component: BilliconComponent;
  let fixture: ComponentFixture<BilliconComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ BilliconComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(BilliconComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
