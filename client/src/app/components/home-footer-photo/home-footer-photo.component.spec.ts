import { ComponentFixture, TestBed } from '@angular/core/testing';

import { HomeFooterPhotoComponent } from './home-footer-photo.component';

describe('HomeFooterPhotoComponent', () => {
  let component: HomeFooterPhotoComponent;
  let fixture: ComponentFixture<HomeFooterPhotoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HomeFooterPhotoComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(HomeFooterPhotoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
