import { ComponentFixture, TestBed } from '@angular/core/testing';

import { HomeFavoriteComponent } from './home-favorite.component';

describe('HomeFavoriteComponent', () => {
  let component: HomeFavoriteComponent;
  let fixture: ComponentFixture<HomeFavoriteComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HomeFavoriteComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(HomeFavoriteComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
