import { Component } from '@angular/core';
import {MenuComponent} from "../../components/menu/menu.component";
import {FooterComponent} from "../../components/footer/footer.component";
import {SearchBarComponent} from "../../components/search-bar/search-bar.component";
import {SliderComponent} from "../../components/slider/slider.component";
import {ProductComponent} from "../../components/product/product.component";
import {ProductsSliderComponent} from "../../components/products-slider/products-slider.component";

@Component({
  selector: 'app-products',
  standalone: true,
  imports: [MenuComponent,
    FooterComponent,
    SearchBarComponent, SliderComponent,
    ProductComponent,
    ProductsSliderComponent],
  templateUrl: './products.component.html',
  styleUrl: './products.component.scss'
})
export class ProductsComponent {

  recommendedProducts = [{name: 'Product 1', price: '$19.99'},
    {name: 'Product 2', price: '$29.99'},
    {name: 'Product 3', price: '$39.99'},
    {name: 'Product 4', price: '$39.99'}]


bestSellerProducts = [{name: 'Product 1', price: '$19.99'},
  {name: 'Product 2', price: '$29.99'},
  {name: 'Product 3', price: '$39.99'},
  {name: 'Product 4', price: '$39.99'}]

  allProducts = [{name: 'Product 1', price: '$19.99'},
    {name: 'Product 2', price: '$29.99'},
    {name: 'Product 3', price: '$39.99'},
    {name: 'Product 4', price: '$39.99'},
    {name: 'Product 1', price: '$19.99'},
    {name: 'Product 2', price: '$29.99'},
    {name: 'Product 3', price: '$39.99'},
    {name: 'Product 4', price: '$39.99'},
    {name: 'Product 1', price: '$19.99'},
    {name: 'Product 2', price: '$29.99'},
    {name: 'Product 3', price: '$39.99'},
    {name: 'Product 4', price: '$39.99'},
    {name: 'Product 1', price: '$19.99'},
    {name: 'Product 2', price: '$29.99'},
    {name: 'Product 3', price: '$39.99'},
    {name: 'Product 4', price: '$39.99'}]

}
