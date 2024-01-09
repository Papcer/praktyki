import {Component, Renderer2, ElementRef, Input, OnInit, HostListener} from '@angular/core';
import {NgOptimizedImage} from "@angular/common";
import {CommonModule} from '@angular/common';

@Component({
  selector: 'app-slider-width-arrows',
  standalone: true,
  imports: [
    NgOptimizedImage,
    CommonModule
  ],
  templateUrl: './slider-width-arrows.component.html',
  styleUrl: './slider-width-arrows.component.scss'
})
export class SliderWidthArrowsComponent implements OnInit {
  @Input() imagesTab: string[] = [];
  @Input() isMobile: boolean = false;
  isScrolling = true;

  constructor(private renderer: Renderer2, private el: ElementRef) {
  }

  ngOnInit() {
    this.startAutoScroll();
  }

  startAutoScroll() {
    setInterval(() => {
      if (this.isScrolling) {
        this.scrollSlides();
      }
    }, 5000);
  }

  scrollSlides(arrow?: string) {
    const slidesBox: any = this.el.nativeElement.querySelector('.slides-box');
    const maxScrollLeft = slidesBox.scrollWidth - slidesBox.clientWidth;
    let newScrollLeft;
   let scrollDistance = window.innerWidth < 768 ? 307 : 377;


    if (arrow == 'right') {
      newScrollLeft = slidesBox.scrollLeft + scrollDistance;
      if (newScrollLeft >= maxScrollLeft) {

      }
    } else if (arrow == 'left') {
      newScrollLeft = slidesBox.scrollLeft - scrollDistance;
      if (newScrollLeft >= maxScrollLeft) {

      }
    } else if (arrow == null) {
      newScrollLeft = slidesBox.scrollLeft + scrollDistance;
      if (newScrollLeft >= maxScrollLeft) {
          newScrollLeft = 0;
      }
    }
    this.renderer.setProperty(slidesBox, 'scrollLeft', newScrollLeft);
    if (newScrollLeft >= maxScrollLeft) {
      this.toggleRoundFrameClass('right-arrow', false);
    }
    if (newScrollLeft <= 0) {
      this.toggleRoundFrameClass('left-arrow', false);
    }
    else if(newScrollLeft >0 && newScrollLeft < maxScrollLeft) {
      this.toggleRoundFrameClass('left-arrow', true);
      this.toggleRoundFrameClass('right-arrow', true);
    }
  }

  private toggleRoundFrameClass(arrowId: string, condition: boolean) {
    const arrowElement: any = this.el.nativeElement.querySelector(`#${arrowId}`);

    if (condition) {
      this.renderer.addClass(arrowElement, 'round-frame');
    } else {
      this.renderer.removeClass(arrowElement, 'round-frame');
    }
  }


  @HostListener('mouseenter') onMouseEnter() {
    this.isScrolling = false;
  }

  @HostListener('mouseleave') onMouseLeave() {
    this.isScrolling = true;
  }
}
