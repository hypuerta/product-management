import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { AuthStateService } from '../../../core/services/auth-state.service';
import { AuthResponse } from '../models/auth.model';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly authState = inject(AuthStateService);
  private readonly baseUrl = 'https://localhost:7061/api/auth';
  readonly isLoggedIn = this.authState.isLoggedIn;

  login(email: string, password: string): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.baseUrl}/login`, { email, password }).pipe(
      tap(response => this.authState.setToken(response.token))
    );
  }

  register(email: string, password: string): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/register`, { email, password });
  }

  logout(): void {
    this.authState.clearToken();
  }
}
