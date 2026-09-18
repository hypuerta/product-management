import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../features/auth/services/auth.service';

@Component({
  selector: 'app-auth-panel',
  imports: [FormsModule],
  templateUrl: './auth-panel.component.html',
  styleUrl: './auth-panel.component.scss'
})
export class AuthPanelComponent {
  private readonly authService = inject(AuthService);
  readonly isLoggedIn = this.authService.isLoggedIn;
  email = 'herleypuerta1';
  password = 'pass1';
  notice = '';
  error = '';

  onLogin(): void {
    this.authService.login(this.email, this.password).subscribe({
      next: () => { this.notice = 'Signed in.'; this.error = ''; },
      error: () => this.error = 'Login failed. Check the credentials.'
    });
  }

  onRegister(): void {
    this.authService.register(this.email, this.password).subscribe({
      next: () => { this.notice = 'User Created.'; this.error = ''; },
      error: () => this.error = 'Registration failed.'
    });
  }

  onLogout(): void {
    this.authService.logout();
  }

  dismissError(): void {
    this.error = '';
  }

  dismissNotice(): void {
    this.notice = '';
  }
}
