import { Component } from '@angular/core';
import {LogoComponent} from "../logo/logo.component";
import {SliderComponent} from "../slider/slider.component";

@Component({
  selector: 'app-menu',
  standalone: true,
  imports: [LogoComponent,
    SliderComponent],
  templateUrl: './menu.component.html',
  styleUrl: './menu.component.scss'
})
export class MenuComponent {

}
