import { signal } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of, throwError } from 'rxjs';
import { AuthService } from '../../features/auth/services/auth.service';
import { AuthPanelComponent } from './auth-panel.component';

describe('AuthPanelComponent', () => {
  let fixture: ComponentFixture<AuthPanelComponent>;
  let component: AuthPanelComponent;
  let authService: {
    isLoggedIn: ReturnType<typeof signal<boolean>>;
    login: jasmine.Spy;
    register: jasmine.Spy;
    logout: jasmine.Spy;
  };

  beforeEach(async () => {
    authService = {
      isLoggedIn: signal(false),
      login: jasmine.createSpy('login'),
      register: jasmine.createSpy('register'),
      logout: jasmine.createSpy('logout')
    };

    await TestBed.configureTestingModule({
      imports: [AuthPanelComponent],
      providers: [{ provide: AuthService, useValue: authService }]
    }).compileComponents();

    fixture = TestBed.createComponent(AuthPanelComponent);
    component = fixture.componentInstance;
  });

  it('logs in with the entered credentials', () => {
    component.email = 'user@example.com';
    component.password = 'pass123';
    authService.login.and.returnValue(of({ token: 'token', email: component.email }));

    component.onLogin();

    expect(authService.login).toHaveBeenCalledWith('user@example.com', 'pass123');
    expect(component.notice).toBe('Signed in.');
  });

  it('shows an error when login fails', () => {
    authService.login.and.returnValue(throwError(() => new Error('unauthorized')));

    component.onLogin();

    expect(component.error).toBe('Login failed. Check the credentials.');
  });

  it('registers with the entered credentials', () => {
    component.email = 'new@example.com';
    component.password = 'pass123';
    authService.register.and.returnValue(of(void 0));

    component.onRegister();

    expect(authService.register).toHaveBeenCalledWith('new@example.com', 'pass123');
    expect(component.notice).toBe('User Created.');
  });

  it('delegates logout to AuthService', () => {
    component.onLogout();

    expect(authService.logout).toHaveBeenCalled();
  });
});
