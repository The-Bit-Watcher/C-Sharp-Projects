import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { ToastController, LoadingController, AlertController } from '@ionic/angular';
import { WatchlistService } from '../../../services/watchlist';
import { WatchedService } from '../../../services/watched';
import { AuthService } from '../../../services/auth';
import { WatchlistItem, Movie } from '../../../Models/movie';

@Component({
  selector: 'app-watchlist',
  templateUrl: './watchlist.page.html',
  styleUrls: ['./watchlist.page.scss'],
  standalone: false
})
export class WatchlistPage {
  watchlist: WatchlistItem[] = [];
  isLoading = false;
  
  constructor(
    private watchlistService: WatchlistService,
    private watchedService: WatchedService,
    private authService: AuthService,
    private router: Router,
    private toastController: ToastController,
    private loadingController: LoadingController,
    private alertController: AlertController
  ) {}
  
  async ionViewWillEnter() {
    await this.loadWatchlist();
  }
  
  async loadWatchlist() {
    this.isLoading = true;
    try {
      this.watchlist = await this.watchlistService.getWatchlist().toPromise() || [];
    } catch (error) {
      console.error('Error loading watchlist:', error);
      await this.showToast('Error loading watchlist', 'danger');
    } finally {
      this.isLoading = false;
    }
  }
  
  goToDetail(movie: Movie) {
    this.router.navigate(['/movie-detail'], {
      state: { movie }
    });
  }
  
  async markAsWatched(movie: Movie, event: Event) {
    event.stopPropagation();
    
    const alert = await this.alertController.create({
      header: 'Mark as Watched',
      message: `Have you watched "${movie.title}"?`,
      buttons: [
        { text: 'Cancel', role: 'cancel' },
        { 
          text: 'Yes, Watched!', 
          handler: async () => {
            const loading = await this.loadingController.create({
              message: 'Moving to watched...'
            });
            await loading.present();
            
            try {
              await this.watchedService.markAsWatched(movie).toPromise();
              await this.loadWatchlist();
              await this.showToast(`"${movie.title}" moved to watched list`, 'success');
            } catch (error) {
              await this.showToast('Error marking as watched', 'danger');
            } finally {
              await loading.dismiss();
            }
          }
        }
      ]
    });
    await alert.present();
  }
  
  async removeFromWatchlist(movie: Movie, event: Event) {
    event.stopPropagation();
    
    const alert = await this.alertController.create({
      header: 'Remove from Watchlist',
      message: `Are you sure you want to remove "${movie.title}" from your watchlist?`,
      buttons: [
        { text: 'Cancel', role: 'cancel' },
        { 
          text: 'Remove', 
          role: 'destructive',
          handler: async () => {
            const loading = await this.loadingController.create({
              message: 'Removing...'
            });
            await loading.present();
            
            try {
              await this.watchlistService.removeFromWatchlist(movie.imdbId).toPromise();
              await this.loadWatchlist();
              await this.showToast(`"${movie.title}" removed from watchlist`, 'success');
            } catch (error) {
              await this.showToast('Error removing movie', 'danger');
            } finally {
              await loading.dismiss();
            }
          }
        }
      ]
    });
    await alert.present();
  }
  
  async logout() {
    const alert = await this.alertController.create({
      header: 'Logout',
      message: 'Are you sure you want to logout?',
      buttons: [
        { text: 'Cancel', role: 'cancel' },
        {
          text: 'Logout',
          handler: async () => {
            const loading = await this.loadingController.create({
              message: 'Logging out...'
            });
            await loading.present();
            
            this.authService.logout();
            await loading.dismiss();
            await this.showToast('Logged out successfully', 'success');
            this.router.navigate(['/login'], { replaceUrl: true });
          }
        }
      ]
    });
    await alert.present();
  }
  
  async refresh(event: any) {
    await this.loadWatchlist();
    event.target.complete();
  }
  
  private async showToast(message: string, color: 'success' | 'danger' | 'warning') {
    const toast = await this.toastController.create({
      message,
      duration: 2500,
      color,
      position: 'bottom'
    });
    await toast.present();
  }
}