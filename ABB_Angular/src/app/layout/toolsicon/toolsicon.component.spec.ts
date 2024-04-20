import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ToolsiconComponent } from './toolsicon.component';

describe('ToolsiconComponent', () => {
  let component: ToolsiconComponent;
  let fixture: ComponentFixture<ToolsiconComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ToolsiconComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ToolsiconComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
