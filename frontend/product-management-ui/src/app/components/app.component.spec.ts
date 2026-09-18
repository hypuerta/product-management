import { signal } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { AuthService } from '../features/auth/services/auth.service';
import { ProductsService } from '../features/products/services/products.service';
import { AppComponent } from './app.component';

describe('AppComponent', () => {
  beforeEach(async () => {
    const authService = {
      isLoggedIn: signal(false)
    };
    const productsService = {
      getProducts: jasmine.createSpy('getProducts').and.returnValue(of([]))
    };

    await TestBed.configureTestingModule({
      imports: [AppComponent],
      providers: [
        { provide: AuthService, useValue: authService },
        { provide: ProductsService, useValue: productsService }
      ]
    }).compileComponents();
  });

  it('renders the auth and products feature containers', () => {
    const fixture = TestBed.createComponent(AppComponent);

    fixture.detectChanges();

    expect(fixture.nativeElement.querySelector('app-auth-panel')).not.toBeNull();
    expect(fixture.nativeElement.querySelector('app-products-workspace')).not.toBeNull();
  });
});
