import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { IonicModule, ToastController } from '@ionic/angular';
import { Router } from '@angular/router';
import { StorageService } from 'src/services/storage';
import { WatchedItem, Movie } from "src/Models/model"


@Component({
  selector: 'app-watched',
  templateUrl: './watched.page.html',
  styleUrls: ['./watched.page.scss'],
  standalone: false,
})
export class WatchedPage {

  watched: WatchedItem[] = [];
  constructor(private router: Router, private storageService: StorageService, private toastController: ToastController) { }

  async ionViewWillEnter(){
    await this.getWatchedList();
  }

  async getWatchedList(){
    this.watched = await this.storageService.getWatched();
  }
  
   goToDetail(movie: Movie) {
    this.router.navigate(['/movie-detail'], {
      state: { movie: movie }
    });
  }

  async resetWatched(event: Event, movie: Movie) {
    event.stopPropagation();
    await this.storageService.resetWatched(movie);
    this.watched = await this.storageService.getWatched(); // refresh
    await this.showToast(`"${movie.title}" moved back to Watchlist`, 'warning');
  }

  async removeFromWatched(event: Event, imdbId: string) {
    event.stopPropagation();
    await this.storageService.removeFromWatched(imdbId);
    this.watched = await this.storageService.getWatched(); // refresh
    await this.showToast('Removed from Watched', 'warning');
  }

  async showToast(message: string, color: 'success' | 'danger' | 'warning') {
    const toast = await this.toastController.create({
      message,
      duration: 2500,
      color,
      position: 'top',
    });
    await toast.present();
  }
  
}