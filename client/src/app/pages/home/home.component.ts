import { Component } from '@angular/core';
import {MenuComponent} from "../../components/menu/menu.component";
import {HomeOrderComponent} from "../../components/home-order/home-order.component";
import {HomeFavoriteComponent} from "../../components/home-favorite/home-favorite.component";
import {HomeForkComponent} from "../../components/home-fork/home-fork.component";
import {HomeCommentComponent} from "../../components/home-comment/home-comment.component";
import {HomeFooterPhotoComponent} from "../../components/home-footer-photo/home-footer-photo.component";
import {FooterComponent} from "../../components/footer/footer.component";

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [MenuComponent,
    HomeOrderComponent,
    HomeFavoriteComponent,
    HomeForkComponent,
    HomeCommentComponent,
    HomeFooterPhotoComponent,
    FooterComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent {

}
