import { Injectable, signal } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class AuthStateService {
  private readonly tokenKey = 'product-management-token';
  private readonly loggedIn = signal(Boolean(localStorage.getItem(this.tokenKey)));

  readonly isLoggedIn = this.loggedIn.asReadonly();

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  setToken(token: string): void {
    localStorage.setItem(this.tokenKey, token);
    this.loggedIn.set(true);
  }

  clearToken(): void {
    localStorage.removeItem(this.tokenKey);
    this.loggedIn.set(false);
  }
}
