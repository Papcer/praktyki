import {Component, Input} from '@angular/core';

@Component({
  selector: 'app-home-comment',
  standalone: true,
  imports: [],
  templateUrl: './home-comment.component.html',
  styleUrl: './home-comment.component.scss'
})
export class HomeCommentComponent {
  @Input() isMobile: boolean = false;
}
