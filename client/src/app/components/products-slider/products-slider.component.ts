import {Component, Input} from '@angular/core';
import {ProductComponent} from "../product/product.component";
import {NgIf} from "@angular/common";
import { Product } from '../product/product.component';
import { CommonModule } from '@angular/common';
@Component({
  selector: 'app-products-slider',
  standalone: true,
  imports: [ProductComponent, NgIf, CommonModule],
  templateUrl: './products-slider.component.html',
  styleUrl: './products-slider.component.scss'
})
export class ProductsSliderComponent {
  @Input() title = 'Title';
  @Input() showDots: boolean = true;
  @Input() showArrows: boolean = true;
  @Input() products: Product[] = [];

}
