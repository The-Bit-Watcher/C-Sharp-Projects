import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { ToastController, LoadingController } from '@ionic/angular';
import { Movie } from '../../../Models/movie';
import { WatchlistService } from '../../../services/watchlist';
import { WatchedService } from '../../../services/watched';

@Component({
  selector: 'app-movie-detail',
  templateUrl: './movie-detail.page.html',
  styleUrls: ['./movie-detail.page.scss'],
  standalone: false
})
export class MovieDetailPage {
  movie: Movie | null = null;

  constructor(
    private router: Router,
    private toastController: ToastController,
    private loadingController: LoadingController,
    private watchlistService: WatchlistService,
    private watchedService: WatchedService
  ) {
    const navigation = this.router.getCurrentNavigation();
    this.movie = navigation?.extras?.state?.['movie'] || null;
  }

  async addToWatchlist() {
    if (!this.movie) {
      await this.showToast('Movie data not found', 'danger');
      return;
    }

    const loading = await this.loadingController.create({
      message: 'Adding to watchlist...'
    });
    await loading.present();

    try {
      await this.watchlistService.addToWatchlist(this.movie).toPromise();
      await this.showToast(`"${this.movie.title}" added to Watchlist`, 'success');
    } catch (error) {
      await this.showToast('Error adding to watchlist', 'danger');
    } finally {
      await loading.dismiss();
    }
  }

  async addToWatched() { 
    if (!this.movie) {
      await this.showToast('Movie data not found', 'danger');
      return;
    }

    const loading = await this.loadingController.create({
      message: 'Adding to watched...'
    });
    await loading.present();

    try {
      await this.watchedService.markAsWatched(this.movie).toPromise();
      await this.showToast(`"${this.movie.title}" added to Watched`, 'success');
    } catch (error) {
      await this.showToast('Error adding to watched', 'danger');
    } finally {
      await loading.dismiss();
    }
  }

  private async showToast(message: string, color: 'success' | 'danger' | 'warning') {
    const toast = await this.toastController.create({
      message,
      duration: 2500,
      color,
      position: 'top'
    });
    await toast.present();
  }
}