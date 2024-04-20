import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BariconComponent } from './baricon.component';

describe('BariconComponent', () => {
  let component: BariconComponent;
  let fixture: ComponentFixture<BariconComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ BariconComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(BariconComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
