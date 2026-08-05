import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { IonicModule, ToastController } from '@ionic/angular';
import { Router } from '@angular/router';
import { Movie } from 'src/Models/model';
import { StorageService } from 'src/services/storage';

@Component({
  selector: 'app-movie-detail',
  templateUrl: './movie-detail.page.html',
  styleUrls: ['./movie-detail.page.scss'],
  standalone: false,
})
export class MovieDetailPage {
  movie: Movie | null = null; 

  constructor(
    private router: Router, 
    private toastController: ToastController, 
    private storage: StorageService
  ) {
    const navigation = this.router.getCurrentNavigation();
    this.movie = navigation?.extras?.state?.['movie'] || null;
    
    // Debug log
    console.log('Movie received:', this.movie);
  }

  async addToWatchlist(event: Event) {
    event.stopPropagation();
    if (!this.movie) {
      await this.showToast('Movie data not found', 'danger');
      return;
    }
    
    await this.storage.addToWatchlist(this.movie);
    await this.showToast(`"${this.movie.title}" added to Watchlist`, 'success');
  }

  async addToWatched(event: Event) {
    event.stopPropagation();
    if (!this.movie) {
      await this.showToast('Movie data not found', 'danger');
      return;
    }
    
    await this.storage.addToWatched(this.movie);
    await this.showToast(`"${this.movie.title}" added to Watched successfully!`, 'success');
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