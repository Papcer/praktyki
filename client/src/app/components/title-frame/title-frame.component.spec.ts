import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TitleFrameComponent } from './title-frame.component';

describe('TitleFrameComponent', () => {
  let component: TitleFrameComponent;
  let fixture: ComponentFixture<TitleFrameComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TitleFrameComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(TitleFrameComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
