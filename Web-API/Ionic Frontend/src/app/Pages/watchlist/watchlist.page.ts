import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { IonicModule, ToastController } from '@ionic/angular';
import { StorageService } from 'src/services/storage';
import { WatchlistItem, Movie } from 'src/Models/model';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-watchlist',
  templateUrl: './watchlist.page.html',
  styleUrls: ['./watchlist.page.scss'],
  standalone: false,
})
export class WatchlistPage {
  constructor(private router: Router, private storage: StorageService, private toastController: ToastController,) { }

  watchList: WatchlistItem[] = [];

  async ionViewWillEnter(){
    this.watchList = await this.storage.getWatchlist();
  }

   goToDetail(movie: Movie) {
    this.router.navigate(['/movie-detail'], {
      state: {movie: movie}});
   }

   async markAsWatched(event: Event, movie: Movie){
      event.stopPropagation();
      await this.storage.addToWatched(movie);
      this.watchList = await this.storage.getWatchlist();
      await this.showToast("Moved to Watched List succesfully", "success");
   }

   async removeFromWatchlist(imdbId: string) {
      await this.storage.removeWatchList(imdbId);
      this.watchList = await this.storage.getWatchlist();
      await this.showToast("Removed from Watchlist", "warning");
    }


     async showToast(message: string, color: 'success' | 'danger' | 'warning') {
        const toast = await this.toastController.create({
            message,
            duration: 250,
            color,
            position: "top"
        });
        await toast.present();
     }
}