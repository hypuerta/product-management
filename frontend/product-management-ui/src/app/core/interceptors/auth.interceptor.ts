import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthStateService } from '../services/auth-state.service';

export const authInterceptor: HttpInterceptorFn = (request, next) => {
  const token = inject(AuthStateService).getToken();
  return next(token ? request.clone({ setHeaders: { Authorization: `Bearer ${token}` } }) : request);
};
