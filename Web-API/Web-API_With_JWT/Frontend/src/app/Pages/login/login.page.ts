import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { ToastController } from '@ionic/angular';
import { AuthService } from '../../../services/auth';

@Component({
  selector: 'app-login',
  templateUrl: './login.page.html',
  styleUrls: ['./login.page.scss'],
  standalone: false
})
export class LoginPage {
  isLoginMode = true;
  email = '';
  password = '';
  username = '';
  showPassword = false;
  isLoading = false;
  
  constructor(
    private authService: AuthService,
    private router: Router,
    private toastController: ToastController
  ) {}
  
  async ionViewWillEnter() {
    if (this.authService.isLoggedIn()) {
      this.router.navigate(['/tabs/search']);
    }
  }
  
  toggleMode() {
    this.isLoginMode = !this.isLoginMode;
    this.clearForm();
  }
  
  async onSubmit() {
    if (!this.isFormValid()) {
      await this.showToast('Please fill in all required fields', 'warning');
      return;
    }
    
    this.isLoading = true;
    
    try {
      if (this.isLoginMode) {
        await this.handleLogin();
      } else {
        await this.handleRegister();
      }
    } catch (error: any) {
      await this.showToast(error.error?.message || 'An error occurred', 'danger');
    } finally {
      this.isLoading = false;
    }
  }
  
  private async handleLogin() {
    const result = await this.authService.login({
      email: this.email,
      password: this.password
    }).toPromise();
    
    if (result) {
      await this.showToast('Login successful!', 'success');
      this.router.navigate(['/tabs/search'], { replaceUrl: true });
    }
  }
  
  private async handleRegister() {
    const result = await this.authService.register({
      email: this.email,
      username: this.username,
      password: this.password
    }).toPromise();
    
    if (result) {
      await this.showToast('Account created! You are now logged in.', 'success');
      this.router.navigate(['/tabs/search'], { replaceUrl: true });
    }
  }
  
  private isFormValid(): boolean {
    if (!this.email || !this.password) return false;
    if (!this.isLoginMode && !this.username) return false;
    return true;
  }
  
  private clearForm() {
    this.email = '';
    this.password = '';
    this.username = '';
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