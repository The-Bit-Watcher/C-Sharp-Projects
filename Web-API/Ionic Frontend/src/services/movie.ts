import { Injectable } from '@angular/core';
import { Movie as MovieModel } from "../Models/model"
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';

  //used to take in api response and then map it to Movie model in models for easier usage.
  interface RawMovie{
    '#TITLE': string;
    '#YEAR': number;
    '#IMDB_ID': string;
    '#RANK': number;
    '#ACTORS': string;
    '#AKA': string;
    '#IMDB_URL': string;
    '#IMDB_IV': string;
    '#IMG_POSTER': string;
    'photo_width': number;
    'photo_height': number;
  }

  interface ApiResponse {
    ok: boolean;
    description: RawMovie[];
}

@Injectable({
  providedIn: 'root',
})
export class Movie {

  private apiUrl:string = "https://imdb.iamidiotareyoutoo.com/search";

  constructor(private http: HttpClient) {}

  searcMovie(movie: string): Observable<MovieModel[]>{
  return this.http.get<ApiResponse>(`${this.apiUrl}?q=${encodeURIComponent(movie)}`).pipe(
    map(response => {
      console.log('API Response:', response); 
      return response.description.map(raw => this.mapMovie(raw));
    })
  );
}

  getTrailer(imbdId: string){
    return this.http.get<any>(`${this.apiUrl}/media/${imbdId}`);
  }

//   export interface Movie {
//     title: string;
//     year: number;
//     imbd_id: string;
//     rank: number;
//     actors: string[];        
//     aka: string;
//     imbd_url: string;
//     imbd_iv: string;
//     img: string;
//     width: number;
//     height: number;
// };

  mapMovie(_movie: RawMovie): MovieModel{
    return {
      imbd_id: _movie['#IMDB_ID'],
      title: _movie['#TITLE'],
      year: _movie['#YEAR'],
      rank: _movie['#RANK'],
      actors: _movie['#ACTORS'].split(",").map(m => m.trim()),
      aka: _movie['#AKA'],
      imbd_url: _movie['#IMDB_URL'],
      imbd_iv: _movie['#IMDB_IV'],
      img: _movie['#IMG_POSTER'],
      width: _movie['photo_width'],
      height: _movie['photo_height']
    }
  }
}
