import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { ToastController, LoadingController, AlertController } from '@ionic/angular';
import { MovieService } from '../../../services/movie';
import { AuthService } from '../../../services/auth';
import { Movie } from '../../../Models/movie';
import { debounceTime, Subject } from 'rxjs';

@Component({
  selector: 'app-search',
  templateUrl: './search.page.html',
  styleUrls: ['./search.page.scss'],
  standalone: false
})
export class SearchPage {
  searchResults: Movie[] = [];
  searchTerm = '';
  isLoading = false;
  private searchSubject = new Subject<string>();
  
  constructor(
    private movieService: MovieService,
    private authService: AuthService,
    private router: Router,
    private toastController: ToastController,
    private loadingController: LoadingController,
    private alertController: AlertController
  ) {
    this.setupSearchDebounce();
  }
  
  private setupSearchDebounce() {
    this.searchSubject.pipe(debounceTime(500)).subscribe(async term => {
      if (term.trim()) {
        await this.performSearch();
      } else {
        this.searchResults = [];
      }
    });
  }
  
  onSearchChange(event: any) {
    this.searchTerm = event.detail.value || '';
    this.searchSubject.next(this.searchTerm);
  }
  
  async performSearch() {
    if (!this.searchTerm.trim()) {
      await this.showToast('Please enter a movie title', 'warning');
      return;
    }
    
    this.isLoading = true;
    
    try {
      this.searchResults = await this.movieService.searchMovies(this.searchTerm).toPromise() || [];
      
      if (this.searchResults.length === 0) {
        await this.showToast('No movies found', 'warning');
      }
    } catch (error) {
      console.error('Search error:', error);
      await this.showToast('Error searching movies', 'danger');
    } finally {
      this.isLoading = false;
    }
  }
  
  goToDetail(movie: Movie) {
    this.router.navigate(['/movie-detail'], {
      state: { movie }
    });
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