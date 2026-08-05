import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';
import { GenreStat, YearlyStat, SummaryStats } from '../Models/movie';

@Injectable({ providedIn: 'root' })
export class StatisticsService {
  private apiUrl = `${environment.apiUrl}/statistics`;
  
  constructor(private http: HttpClient) {}
  
  getGenreStats(): Observable<GenreStat[]> {
    return this.http.get<GenreStat[]>(`${this.apiUrl}/genres`);
  }
  
  getYearlyStats(): Observable<YearlyStat[]> {
    return this.http.get<YearlyStat[]>(`${this.apiUrl}/yearly`);
  }
  
  getSummaryStats(): Observable<SummaryStats> {
    return this.http.get<SummaryStats>(`${this.apiUrl}/summary`);
  }
}