import { Component, ElementRef, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { ToastController, LoadingController, AlertController } from '@ionic/angular';
import { Chart, registerables } from 'chart.js';
import { StatisticsService } from '../../../services/statistics';
import { AuthService } from '../../../services/auth';
import { GenreStat, YearlyStat, SummaryStats } from '../../../Models/movie';

Chart.register(...registerables);

@Component({
  selector: 'app-statistics',
  templateUrl: './statistics.page.html',
  styleUrls: ['./statistics.page.scss'],
  standalone: false
})
export class StatisticsPage {
  @ViewChild('genreChart') genreChartRef!: ElementRef;
  @ViewChild('yearlyChart') yearlyChartRef!: ElementRef;
  
  genreChart: Chart | null = null;
  yearlyChart: Chart | null = null;
  
  genreStats: GenreStat[] = [];
  yearlyStats: YearlyStat[] = [];
  summaryStats: SummaryStats | null = null;
  isLoading = true;
  
  constructor(
    private statisticsService: StatisticsService,
    private authService: AuthService,
    private router: Router,
    private toastController: ToastController,
    private loadingController: LoadingController,
    private alertController: AlertController
  ) {}
  
  async ionViewWillEnter() {
    await this.loadStatistics();
  }
  
  async loadStatistics() {
    this.isLoading = true;
    
    try {
      const [genreStats, yearlyStats, summaryStats] = await Promise.all([
        this.statisticsService.getGenreStats().toPromise(),
        this.statisticsService.getYearlyStats().toPromise(),
        this.statisticsService.getSummaryStats().toPromise()
      ]);
      
      this.genreStats = genreStats || [];
      this.yearlyStats = yearlyStats || [];
      this.summaryStats = summaryStats || null;
      
      setTimeout(() => {
        this.createGenreChart();
        this.createYearlyChart();
      }, 100);
    } catch (error) {
      console.error('Error loading statistics:', error);
    } finally {
      this.isLoading = false;
    }
  }
  
  createGenreChart() {
    if (this.genreChart) this.genreChart.destroy();
    
    const labels = this.genreStats.map(stat => stat.genre);
    const data = this.genreStats.map(stat => stat.count);
    const colors = ['#FF6384', '#36A2EB', '#FFCE56', '#4BC0C0', '#9966FF', '#FF9F40'];
    
    this.genreChart = new Chart(this.genreChartRef.nativeElement, {
      type: 'pie',
      data: {
        labels: labels,
        datasets: [{
          data: data,
          backgroundColor: colors,
          borderWidth: 2,
          borderColor: '#1e1e2e'
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: true,
        plugins: {
          legend: { position: 'bottom', labels: { color: 'white', font: { size: 12 } } }
        }
      }
    });
  }
  
  createYearlyChart() {
    if (this.yearlyChart) this.yearlyChart.destroy();
    
    const labels = this.yearlyStats.map(stat => stat.year.toString());
    const data = this.yearlyStats.map(stat => stat.count);
    
    this.yearlyChart = new Chart(this.yearlyChartRef.nativeElement, {
      type: 'bar',
      data: {
        labels: labels,
        datasets: [{
          label: 'Movies Watched',
          data: data,
          backgroundColor: 'rgba(226, 183, 20, 0.8)',
          borderColor: '#e2b714',
          borderWidth: 1,
          borderRadius: 8
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: true,
        scales: {
          y: { beginAtZero: true, ticks: { color: 'white', stepSize: 1 } },
          x: { ticks: { color: 'white',  maxRotation: 45, minRotation: 45 } }
        }
      }
    });
  }
  
  getTotalWatched(): number {
    return this.yearlyStats.reduce((sum, stat) => sum + stat.count, 0);
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
            
            const toast = await this.toastController.create({
              message: 'Logged out successfully',
              duration: 2500,
              color: 'success',
              position: 'top'
            });
            await toast.present();
            
            this.router.navigate(['/login'], { replaceUrl: true });
          }
        }
      ]
    });
    await alert.present();
  }
  
  ionViewWillLeave() {
    if (this.genreChart) this.genreChart.destroy();
    if (this.yearlyChart) this.yearlyChart.destroy();
  }
}