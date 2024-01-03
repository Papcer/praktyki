import { Component, HostListener } from '@angular/core';
import {LogoComponent} from "../logo/logo.component";
import {SliderComponent} from "../slider/slider.component";
import { CommonModule } from '@angular/common';
import { trigger, state, style, transition, animate } from '@angular/animations';

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


  // rozwijane menu mobilne
  @HostListener('window:resize', ['$event'])
  onResize(event: Event): void {
    this.showPhoneMenu = window.innerWidth <= 768 ? this.showPhoneMenu : false;
    this.isMenuOpen = window.innerWidth <= 768 ? this.isMenuOpen : false;
    this.showPhoneMenu = window.innerWidth > 768 ? this.showPhoneMenu : true;
  }
  toggleMenu() {
    this.isMenuOpen = !this.isMenuOpen;
    const menuBox = document.querySelector('.phone-menu-box');

    if (menuBox) {
      if (this.isMenuOpen) {
        menuBox.classList.add('full');
      } else {
        menuBox.classList.remove('full');
      }
    }
  }

  // scrolowanie
  @HostListener('window:scroll', ['$event'])
  checkScroll() {
    const scrollPosition = window.pageYOffset;

    if (scrollPosition > this.lastScrollPosition && scrollPosition > 10) {
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
