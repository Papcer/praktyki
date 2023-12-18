import { ComponentFixture, TestBed } from '@angular/core/testing';

import { HomeOrderComponent } from './home-order.component';

describe('HomeOrderComponent', () => {
  let component: HomeOrderComponent;
  let fixture: ComponentFixture<HomeOrderComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HomeOrderComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(HomeOrderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
