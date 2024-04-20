import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EmailiconComponent } from './emailicon.component';

describe('EmailiconComponent', () => {
  let component: EmailiconComponent;
  let fixture: ComponentFixture<EmailiconComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EmailiconComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EmailiconComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
