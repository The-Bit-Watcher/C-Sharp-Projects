import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';
import { Movie } from '../Models/movie';

@Injectable({ providedIn: 'root' })
export class MovieService {
  private apiUrl = `${environment.apiUrl}/Movies`;
  
  constructor(private http: HttpClient) {}
  
  searchMovies(title: string): Observable<Movie[]> {
    return this.http.get<Movie[]>(`${this.apiUrl}/search?title=${title}`);
  }
  
  getMovieDetails(imdbId: string): Observable<Movie> {
    return this.http.get<Movie>(`${this.apiUrl}/details/${imdbId}`);
  }
}