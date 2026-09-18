import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { AuthStateService } from '../../../core/services/auth-state.service';
import { AuthResponse } from '../models/auth.model';
import { AuthService } from './auth.service';

describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;
  let authState: AuthStateService;

  beforeEach(() => {
    localStorage.clear();

    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule]
    });

    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
    authState = TestBed.inject(AuthStateService);
  });

  afterEach(() => {
    httpMock.verify();
    localStorage.clear();
  });

  it('logs in and stores the token in auth state', () => {
    const response: AuthResponse = {
      token: 'jwt-token',
      email: 'user@example.com'
    };

    let result: AuthResponse | undefined;

    service.login('user@example.com', 'pass123').subscribe(res => {
      result = res;
    });

    const req = httpMock.expectOne('https://localhost:7061/api/auth/login');
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ email: 'user@example.com', password: 'pass123' });

    req.flush(response);

    expect(result).toEqual(response);
    expect(authState.getToken()).toBe('jwt-token');
    expect(service.isLoggedIn()).toBeTrue();
  });

  it('registers a user with email and password', () => {
    service.register('new@example.com', 'pass123').subscribe();

    const req = httpMock.expectOne('https://localhost:7061/api/auth/register');
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ email: 'new@example.com', password: 'pass123' });

    req.flush(null);
  });

  it('clears the token when logging out', () => {
    authState.setToken('existing-token');

    service.logout();

    expect(authState.getToken()).toBeNull();
    expect(service.isLoggedIn()).toBeFalse();
  });
});
