import { Injectable } from '@angular/core';
import { StorageService } from './storage';
import { User } from '../Models/model'

@Injectable({ providedIn: 'root' })
export class AuthService {

  constructor(private storageService: StorageService) {}

  async signup(user: User): Promise<{ success: boolean; message: string }> {
    const existing = await this.storageService.getUser();

    if (existing && existing.email === user.email) {
      return { success: false, message: 'An account with this email already exists.' };
    }

    await this.storageService.saveUser(user);
    return { success: true, message: 'Account created successfully.' };
  }

  async login(email: string, password: string): Promise<{ success: boolean; message: string }> {
    const user = await this.storageService.getUser();

    if (!user) {
      return { success: false, message: 'No account found. Please sign up.' };
    }

    if (user.email !== email || user.password !== password) {
      return { success: false, message: 'Incorrect email or password.' };
    }

    return { success: true, message: 'Login successful.' };
  }

  async logout(): Promise<void> {
    await this.storageService.clearUser();
  }

  async isLoggedIn(): Promise<boolean> {
    const user = await this.storageService.getUser();
    return user !== null;
  }

  async getCurrentUser(): Promise<User | null> {
    return await this.storageService.getUser();
  }
}