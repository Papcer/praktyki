import {Component, Input} from '@angular/core';

export interface Product {
  name: string;
  price: string;
}

@Component({
  selector: 'app-product',
  standalone: true,
  imports: [],
  templateUrl: './product.component.html',
  styleUrl: './product.component.scss'
})
export class ProductComponent {

  @Input() product!: Product;
}
