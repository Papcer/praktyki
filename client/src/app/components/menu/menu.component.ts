import { Component } from '@angular/core';
import {LogoComponent} from "../logo/logo.component";

@Component({
  selector: 'app-menu',
  standalone: true,
  imports: [LogoComponent],
  templateUrl: './menu.component.html',
  styleUrl: './menu.component.scss'
})
export class MenuComponent {

}
