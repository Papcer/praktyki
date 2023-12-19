import { ComponentFixture, TestBed } from '@angular/core/testing';

import { HomeForkComponent } from './home-fork.component';

describe('HomeForkComponent', () => {
  let component: HomeForkComponent;
  let fixture: ComponentFixture<HomeForkComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HomeForkComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(HomeForkComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
