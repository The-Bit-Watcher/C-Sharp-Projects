import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';
import { WatchlistItem, Movie } from '../Models/movie';

@Injectable({ providedIn: 'root' })
export class WatchlistService {
  private apiUrl = `${environment.apiUrl}/watchlist`;
  
  constructor(private http: HttpClient) {}
  
  getWatchlist(): Observable<WatchlistItem[]> {
    return this.http.get<WatchlistItem[]>(this.apiUrl);
  }
  
  addToWatchlist(movie: Movie): Observable<any> {
    return this.http.post(this.apiUrl, movie);
  }
  
  removeFromWatchlist(imdbId: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${imdbId}`);
  }
}