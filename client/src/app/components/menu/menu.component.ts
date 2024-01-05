import { Component, HostListener } from '@angular/core';
import {LogoComponent} from "../logo/logo.component";
import {SliderComponent} from "../slider/slider.component";
import { CommonModule } from '@angular/common';
import { trigger, state, style, animate, transition } from '@angular/animations';

@Component({
  selector: 'app-menu',
  standalone: true,
  imports: [LogoComponent,
    SliderComponent,
    CommonModule,
    ],
  templateUrl: './menu.component.html',
  styleUrl: './menu.component.scss',
})

export class MenuComponent {


  isMenuOpen = false;
  showPhoneMenu = true;
  menuState = 'show';
  showOverlay = false;
  pageShadow = false;
  // rozwijane menu mobilne
  @HostListener('window:resize', ['$event'])
  //zamkniecie menu przy wiekszych oknach
  onResize(event: Event): void {

    this.showPhoneMenu = window.innerWidth <= 768 ? this.showPhoneMenu : false;
    this.isMenuOpen = window.innerWidth <= 768 ? this.isMenuOpen : false;
    this.pageShadow = window.innerWidth <= 768 ? this.pageShadow : false;
    if(window.innerWidth > 768 ) {
      this.enableScroll();
    }
    this.showPhoneMenu = window.innerWidth > 768 ? this.showPhoneMenu : true;
  }



  toggleMenu() {
    this.isMenuOpen = !this.isMenuOpen;
    this.showOverlay = this.isMenuOpen;

    const menuBox = document.querySelector('.phone-menu-box');

    if (menuBox) {
      menuBox.classList.toggle('expanded', this.isMenuOpen);
    }
    if (this.isMenuOpen) {
      this.disableScroll();
      this.pageShadow = true;
    } else {
      this.enableScroll();
      this.pageShadow = false;
    }
  }
  disableScroll() {
    document.body.style.overflow = 'hidden';
  }
  enableScroll() {
    document.body.style.overflow = '';
  }



  // Chowanie i pokazywanie menu głównego w trakcie scrolowania
  @HostListener('window:scroll', ['$event'])
  checkScroll() {
    const scrollPosition = window.pageYOffset;

    if (scrollPosition > this.lastScrollPosition && scrollPosition > 90) {
      // Przewijanie w dół
      this.menuState = 'hide';
    } else {
      // Przewijanie do góry
      this.menuState = 'show';
    }

    this.lastScrollPosition = scrollPosition;
  }

  private lastScrollPosition = 0;
}
