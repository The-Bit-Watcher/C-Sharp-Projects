import { Injectable } from '@angular/core';
import { Storage } from '@ionic/storage-angular';
import { User, Movie, WatchedItem, WatchlistItem } from 'src/Models/model';

const KEYS = {
  USER: 'filmlog_user',
  WATCHLIST: 'filmlog_watchlist',
  WATCHED: 'filmlog_watched',
};

@Injectable({
  providedIn: 'root',
})
export class StorageService {
  private _storage: Storage | null = null;

  constructor(private storage: Storage){
    this.init();
  }

  async init(){
    this._storage = await this.storage.create();;
  }

  async saveUser(user: User): Promise<void>{
    await this._storage?.set(KEYS.USER, user); 
  }

  async getUser(): Promise<User | null>{
    return await this._storage?.get(KEYS.USER) ?? null;
  }

  async clearUser(): Promise<void>{
    await this._storage?.remove(KEYS.USER);
  }

  async getWatchlist(): Promise<WatchlistItem[]> {
    return await this._storage?.get(KEYS.WATCHLIST) ?? [];
  }


  async addToWatchlist(_movie: Movie): Promise<void>{
    const list = await this.getWatchlist();

   const exist = list?.some(m => m.movie.imbd_id == _movie.imbd_id);
   if (exist) return;
  
   const item: WatchlistItem = {
    movie: _movie,
    dateAdded: Date.now().toString(),
   };

   await this._storage?.set(KEYS.WATCHLIST, [...list, item])
  }

  async removeWatchList(imbd_id: string):Promise<void>{
    const list = await this.getWatchlist();

    const updateList = list.filter(m => m.movie.imbd_id !== imbd_id)

    await this._storage?.set(KEYS.WATCHLIST, updateList);
  }

  async isInWatchList(imbd_id: string): Promise<boolean>{
    const list = await this.getWatchlist();
    return await list.some(m => m.movie.imbd_id == imbd_id) ?? false;
  }

  async getWatched(): Promise<WatchedItem[]>{
    return await this._storage?.get(KEYS.WATCHED) ?? [];
  }

  async addToWatched(_movie: Movie): Promise<void> {
  // First remove from watchlist if present
  await this.removeWatchList(_movie.imbd_id);
  
  // Get current watched list
  const list = await this.getWatched();
  
  // Check if movie already exists in watched
  const existingIndex = list.findIndex(item => item.movie.imbd_id === _movie.imbd_id);
  
  if (existingIndex >= 0) {
    // Increment times watched
    list[existingIndex].timesWatched += 1;
    list[existingIndex].dateWatched = Date.now().toString();
    await this._storage?.set(KEYS.WATCHED, list);
  } else {
    // Add new watched item
    const newItem: WatchedItem = {
      movie: _movie,
      timesWatched: 1,
      dateWatched: Date.now().toString(),
    };
    await this._storage?.set(KEYS.WATCHED, [...list, newItem]);
  }
  
  console.log('Added to watched:', _movie.title); // Debug log
}

async removeFromWatched(imdbId: string): Promise<void> {
    const list = await this.getWatched();
    const updated = list.filter(item => item.movie.imbd_id !== imdbId);
    await this._storage?.set(KEYS.WATCHED, updated);
  }

  //move back to watchlist with timesWatched zeroed
  async resetWatched(movie: Movie): Promise<void> {
    await this.removeFromWatched(movie.imbd_id);
    await this.addToWatchlist(movie);
  }
}
