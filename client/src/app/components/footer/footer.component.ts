import {Component, Input} from '@angular/core';
import { CommonModule } from '@angular/common';

interface Language {
  id: string;
  name: string;
}

@Component({
  selector: 'app-footer',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './footer.component.html',
  styleUrl: './footer.component.scss',
})

export class FooterComponent {
  @Input() isMobile: boolean = false;
  isMenuOpen = false;
  mail: string = "sweetdeli@gmail.com";



  selectedLanguage: Language = { id: 'polish', name: 'Polski' };

  languages = [
    { id: 'polish', name: 'Polski' },
    { id: 'english', name: 'English' },
    { id: 'german', name: 'Deutsch' },
    { id: 'french', name: 'Français' },
    { id: 'english', name: 'English' },
    { id: 'german', name: 'Deutsch' },
    { id: 'french', name: 'Français' },
    { id: 'english', name: 'English' },
    { id: 'german', name: 'Deutsch' },
    { id: 'french', name: 'Français' },
    { id: 'english', name: 'English' },
    { id: 'german', name: 'Deutsch' },
    { id: 'french', name: 'Français' },

  ];

  toggleMenu() {
    this.isMenuOpen = !this.isMenuOpen;
  }
  selectLanguage(language: Language) {
    this.selectedLanguage = language;
    this.isMenuOpen = false;
  }

}
