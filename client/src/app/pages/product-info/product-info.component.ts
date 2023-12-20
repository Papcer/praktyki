import { Component } from '@angular/core';
import {MenuComponent} from "../../components/menu/menu.component";
import {SliderComponent} from "../../components/slider/slider.component";
import {FooterComponent} from "../../components/footer/footer.component";

@Component({
  selector: 'app-product-info',
  standalone: true,
  imports: [
    MenuComponent,
    SliderComponent,
    FooterComponent
  ],
  templateUrl: './product-info.component.html',
  styleUrl: './product-info.component.scss'
})
export class ProductInfoComponent {

}
