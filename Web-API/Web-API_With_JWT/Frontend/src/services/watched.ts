import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';
import { WatchedItem, Movie } from '../Models/movie';

@Injectable({ providedIn: 'root' })
export class WatchedService {
  private apiUrl = `${environment.apiUrl}/watched`;
  
  constructor(private http: HttpClient) {}
  
  getWatchedList(): Observable<WatchedItem[]> {
    return this.http.get<WatchedItem[]>(this.apiUrl);
  }
  
  markAsWatched(movie: Movie): Observable<any> {
    return this.http.post(this.apiUrl, movie);
  }
  
  incrementTimesWatched(id: number): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}/increment`, {});
  }
  
  removeFromWatched(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
  
  resetTimesWatched(id: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/${id}/reset`, {});
  }
}