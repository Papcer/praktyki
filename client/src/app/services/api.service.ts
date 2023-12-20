import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private apiUrl = 'http://localhost:8080';  // Zmień na odpowiedni adres URL swojego serwera

  constructor(private http: HttpClient) {
  }




}
