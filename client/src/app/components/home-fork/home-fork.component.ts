import {Component, Input} from '@angular/core';

@Component({
  selector: 'app-home-fork',
  standalone: true,
  imports: [],
  templateUrl: './home-fork.component.html',
  styleUrl: './home-fork.component.scss'
})
export class HomeForkComponent {
  @Input() isMobile: boolean = false;
}
