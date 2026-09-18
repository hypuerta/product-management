import { AuthStateService } from './auth-state.service';

describe('AuthStateService', () => {
  let service: AuthStateService;

  beforeEach(() => {
    localStorage.clear();
    service = new AuthStateService();
  });

  afterEach(() => {
    localStorage.clear();
  });

  it('reads the stored token and initializes the logged-in signal', () => {
    localStorage.setItem('product-management-token', 'stored-token');

    service = new AuthStateService();

    expect(service.getToken()).toBe('stored-token');
    expect(service.isLoggedIn()).toBeTrue();
  });

  it('stores a token and sets the logged-in state to true', () => {
    service.setToken('new-token');

    expect(localStorage.getItem('product-management-token')).toBe('new-token');
    expect(service.getToken()).toBe('new-token');
    expect(service.isLoggedIn()).toBeTrue();
  });

  it('removes the token and sets the logged-in state to false', () => {
    localStorage.setItem('product-management-token', 'existing-token');

    service.clearToken();

    expect(localStorage.getItem('product-management-token')).toBeNull();
    expect(service.getToken()).toBeNull();
    expect(service.isLoggedIn()).toBeFalse();
  });
});
