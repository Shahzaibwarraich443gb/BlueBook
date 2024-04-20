import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReporticonComponent } from './reporticon.component';

describe('ReporticonComponent', () => {
  let component: ReporticonComponent;
  let fixture: ComponentFixture<ReporticonComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ReporticonComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ReporticonComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
