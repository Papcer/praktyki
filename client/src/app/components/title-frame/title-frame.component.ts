import {Component, Input} from '@angular/core';

@Component({
  selector: 'app-title-frame',
  standalone: true,
  imports: [],
  templateUrl: './title-frame.component.html',
  styleUrl: './title-frame.component.scss'
})
export class TitleFrameComponent {
  @Input() title = '';
  @Input() text = '';
  @Input() isMobile = false;
}
