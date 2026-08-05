import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { ToastController, LoadingController, AlertController } from '@ionic/angular';
import { WatchedService } from '../../../services/watched';
import { AuthService } from '../../../services/auth';
import { WatchedItem, Movie } from '../../../Models/movie';

@Component({
  selector: 'app-watched',
  templateUrl: './watched.page.html',
  styleUrls: ['./watched.page.scss'],
  standalone: false
})
export class WatchedPage {
  watchedList: WatchedItem[] = [];
  isLoading = false;
  
  // Statistics properties
  totalWatches = 0;
  averageWatches = 0;
  
  constructor(
    private watchedService: WatchedService,
    private authService: AuthService,
    private router: Router,
    private toastController: ToastController,
    private loadingController: LoadingController,
    private alertController: AlertController
  ) {}
  
  async ionViewWillEnter() {
    await this.loadWatchedList();
  }
  
  async loadWatchedList() {
    this.isLoading = true;
    try {
      this.watchedList = await this.watchedService.getWatchedList().toPromise() || [];
      this.watchedList.sort((a, b) => 
        new Date(b.lastWatchedAt).getTime() - new Date(a.lastWatchedAt).getTime()
      );
      this.calculateStats();
    } catch (error) {
      console.error('Error loading watched list:', error);
      await this.showToast('Error loading watched list', 'danger');
    } finally {
      this.isLoading = false;
    }
  }
  
  calculateStats() {
    this.totalWatches = this.watchedList.reduce((sum, item) => sum + item.timesWatched, 0);
    this.averageWatches = this.watchedList.length > 0 ? this.totalWatches / this.watchedList.length : 0;
  }
  
  goToDetail(movie: Movie) {
    this.router.navigate(['/movie-detail'], {
      state: { movie }
    });
  }
  
  async incrementTimesWatched(item: WatchedItem) {
    const loading = await this.loadingController.create({
      message: 'Updating...'
    });
    await loading.present();
    
    try {
      await this.watchedService.incrementTimesWatched(item.id).toPromise();
      await this.loadWatchedList();
      await this.showToast(`"${item.movie.title}" watched ${item.timesWatched + 1} times!`, 'success');
    } catch (error) {
      await this.showToast('Error updating watch count', 'danger');
    } finally {
      await loading.dismiss();
    }
  }
  
  async removeFromWatched(item: WatchedItem) {
    const alert = await this.alertController.create({
      header: 'Remove from Watched',
      message: `Are you sure you want to remove "${item.movie.title}" from your watched list?`,
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
              await this.watchedService.removeFromWatched(item.id).toPromise();
              await this.loadWatchedList();
              await this.showToast(`"${item.movie.title}" removed from watched list`, 'success');
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
  
  async resetTimesWatched(item: WatchedItem) {
    const alert = await this.alertController.create({
      header: 'Reset Watch Count',
      message: `Reset "${item.movie.title}" watch count from ${item.timesWatched} to 1?`,
      buttons: [
        { text: 'Cancel', role: 'cancel' },
        { 
          text: 'Reset', 
          handler: async () => {
            const loading = await this.loadingController.create({
              message: 'Resetting...'
            });
            await loading.present();
            
            try {
              await this.watchedService.resetTimesWatched(item.id).toPromise();
              await this.loadWatchedList();
              await this.showToast(`"${item.movie.title}" watch count reset to 1`, 'success');
            } catch (error) {
              await this.showToast('Error resetting watch count', 'danger');
            } finally {
              await loading.dismiss();
            }
          }
        }
      ]
    });
    await alert.present();
  }
  
  getRating(rating: string): string {
    if (!rating || rating === 'N/A') return 'No Rating';
    return `${rating}/10`;
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
    await this.loadWatchedList();
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