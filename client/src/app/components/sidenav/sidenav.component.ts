import { Component } from '@angular/core';
import {LogoComponent} from "../logo/logo.component";

@Component({
  selector: 'app-sidenav',
  standalone: true,
  imports: [LogoComponent],
  templateUrl: './sidenav.component.html',
  styleUrl: './sidenav.component.scss'
})
export class SidenavComponent {

}
