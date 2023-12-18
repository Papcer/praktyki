import { Component } from '@angular/core';
import {SidenavComponent} from "../../components/sidenav/sidenav.component";

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [SidenavComponent,],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {

}
