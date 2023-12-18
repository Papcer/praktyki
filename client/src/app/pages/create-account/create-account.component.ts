import { Component } from '@angular/core';
import {SidenavComponent} from "../../components/sidenav/sidenav.component";

@Component({
  selector: 'app-create-account',
  standalone: true,
    imports: [
        SidenavComponent
    ],
  templateUrl: './create-account.component.html',
  styleUrl: './create-account.component.scss'
})
export class CreateAccountComponent {

}
