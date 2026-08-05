import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastController, LoadingController } from '@ionic/angular';
import { AuthService } from 'src/services/auth';
import { User } from 'src/Models/model';

@Component({
  selector: 'app-auth',
  templateUrl: './auth.page.html',
  styleUrls: ['./auth.page.scss'],
  standalone: false,
})
export class AuthPage implements OnInit {
  isLoginMode = true;

  email    = '';
  password = '';
  username = '';      
  age?: number;        

  showPassword = false;

  constructor(
    private authService:     AuthService,
    private router:          Router,
    private toastController: ToastController,
    private loadingCtrl:     LoadingController,
  ) {}

  async ngOnInit() {
    if (await this.authService.isLoggedIn()){
      this.router.navigate(['/tabs/search']);
    }
  }

  toggleMode() {
    this.isLoginMode = !this.isLoginMode;
    this.clearForm();
  }
  async onSubmit() {
    if (!this.isFormValid()) {
      await this.showToast('Please fill in all required fields.', 'warning');
      return;
    }

    const loading = await this.loadingCtrl.create({ message: 'Please wait...' });
    await loading.present();

    try {
      if (this.isLoginMode) {
        await this.handleLogin();
      } else {
        await this.handleSignup();
      }
    } finally {
      await loading.dismiss();
    }
  }

  private async handleLogin() {
    const result = await this.authService.login(this.email, this.password);

    if (result.success) {
      this.router.navigate(['/tabs/search'], { replaceUrl: true });
    } else {
      await this.showToast(result.message, 'danger');
    }
  }

  private async handleSignup() {
    const newUser: User = {
      email:    this.email,
      password: this.password,
      username: this.username,
      age:      this.age,
    };

    const result = await this.authService.signup(newUser);

    if (result.success) {
      await this.showToast('Account created! Please log in.', 'success');
      this.toggleMode();
      this.router.navigate(['/tabs/search'], { replaceUrl: true });
    } else {
      await this.showToast(result.message, 'danger');
    }
  }

  private isFormValid(): boolean {
    if (!this.email || !this.password){
      return false;
    }

    if (this.isLoginMode == false){
      if (!this.username){
        return false;
      }
    }
    return true;
  }

  private clearForm() {
    this.email    = '';
    this.password = '';
    this.username = '';
    this.age      = undefined;
  }

  private async showToast(message: string, color: 'success' | 'danger' | 'warning') {
    const toast = await this.toastController.create({
      message,
      duration: 2500,
      color,
      position: 'bottom',
    });
    await toast.present();
  }
}