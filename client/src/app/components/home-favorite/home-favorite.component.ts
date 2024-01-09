import {Component, Input} from '@angular/core';

@Component({
  selector: 'app-home-favorite',
  standalone: true,
  imports: [],
  templateUrl: './home-favorite.component.html',
  styleUrl: './home-favorite.component.scss'
})
export class HomeFavoriteComponent {
  @Input() isMobile: boolean = false;

}
