import {Component, ViewChild, ElementRef, AfterViewInit, Renderer2, HostListener, OnInit} from '@angular/core';
import {MenuComponent} from "../../components/menu/menu.component";
import {SliderComponent} from "../../components/slider/slider.component";
import {FooterComponent} from "../../components/footer/footer.component";
import {TitleFrameComponent} from "../../components/title-frame/title-frame.component";
import {HomeOrderComponent} from "../../components/home-order/home-order.component";
import {HomeFavoriteComponent} from "../../components/home-favorite/home-favorite.component";
import {HomeForkComponent} from "../../components/home-fork/home-fork.component";
import {HomeCommentComponent} from "../../components/home-comment/home-comment.component";
import {HomeFooterPhotoComponent} from "../../components/home-footer-photo/home-footer-photo.component";
import {SliderWidthArrowsComponent} from "../../components/slider-width-arrows/slider-width-arrows.component";

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    MenuComponent,
    SliderComponent,
    FooterComponent,
    TitleFrameComponent,
    HomeOrderComponent,
    HomeFavoriteComponent,
    HomeForkComponent,
    HomeCommentComponent,
    HomeFooterPhotoComponent,
    SliderWidthArrowsComponent

  ],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent implements OnInit {
  title1='Our Exclusive Cakes';
  text1 = 'Most Popular';
  isMobile: boolean = true;
  photosUrl = "/assets/image/";

  imagesTab=[
    `${this.photosUrl}zdj1.jpeg`,
    `${this.photosUrl}zdj2.jpeg`,
    `${this.photosUrl}zdj4.jpeg`,
    `${this.photosUrl}zdj6.jpeg`,
    `${this.photosUrl}zdj7.jpeg`,
    `${this.photosUrl}zdj2.jpeg`,
    `${this.photosUrl}zdj4.jpeg`,
    `${this.photosUrl}zdj6.jpeg`,
//
  ]

  constructor() {}
  @HostListener('window:resize', ['$event'])
  onResize(event: Event): void {

    this.checkWindowSize();
  }

  private checkWindowSize(): void {
    this.isMobile = window.innerWidth <= 767;
  }

  ngOnInit(): void {
    this.checkWindowSize();
  }
}
