import { ComponentFixture, TestBed } from '@angular/core/testing';

import { HomeCommentComponent } from './home-comment.component';

describe('HomeCommentComponent', () => {
  let component: HomeCommentComponent;
  let fixture: ComponentFixture<HomeCommentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HomeCommentComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(HomeCommentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
