import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-slider',
  standalone: true,
  imports: [],
  templateUrl: './slider.component.html',
  styleUrl: './slider.component.scss'
})
export class SliderComponent {
  @Input() title = '';
  @Input() text = '';

 // photosUrl = "./assets/image/";
  @Input() imagesTab=[
    `./assets/image/zdj1.jpeg`,

   // `${this.photosUrl}zdj8.jpeg`,
  ]

}
