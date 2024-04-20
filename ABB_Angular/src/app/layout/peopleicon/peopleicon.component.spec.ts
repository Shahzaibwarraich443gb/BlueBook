import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PeopleiconComponent } from './peopleicon.component';

describe('PeopleiconComponent', () => {
  let component: PeopleiconComponent;
  let fixture: ComponentFixture<PeopleiconComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PeopleiconComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PeopleiconComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
