import { Component, HostListener } from '@angular/core';
import {LogoComponent} from "../logo/logo.component";
import {SliderComponent} from "../slider/slider.component";
import { CommonModule } from '@angular/common';
@Component({
  selector: 'app-menu',
  standalone: true,
  imports: [LogoComponent,
    SliderComponent,
    CommonModule,
    ],
  templateUrl: './menu.component.html',
  styleUrl: './menu.component.scss'
})
export class MenuComponent {
  isMenuOpen = false;

  @HostListener('window:resize', ['$event'])
  onResize(event: Event): void {
    // Ustaw isMenuOpen na false, gdy szerokość okna przekroczy 768 pikseli
    this.isMenuOpen = window.innerWidth <= 768 ? this.isMenuOpen : false;
  }
  toggleMenu() {
    this.isMenuOpen = !this.isMenuOpen;
  }
}
