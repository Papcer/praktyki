import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SliderWidthArrowsComponent } from './slider-width-arrows.component';

describe('SliderWidthArrowsComponent', () => {
  let component: SliderWidthArrowsComponent;
  let fixture: ComponentFixture<SliderWidthArrowsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SliderWidthArrowsComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(SliderWidthArrowsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
