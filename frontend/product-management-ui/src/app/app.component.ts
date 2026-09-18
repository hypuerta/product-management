import { CurrencyPipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ApiService, Product, ProductInput } from './api.service';

@Component({
  selector: 'app-root',
  imports: [FormsModule, CurrencyPipe],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  private readonly api = inject(ApiService);
  products: Product[] = [];
  selectedId: string | null = null;
  notice = '';
  error = '';
  email = 'demo@example.com';
  password = 'Demo123!';
  isLoggedIn = Boolean(localStorage.getItem('product-management-token'));
  draft: ProductInput = { name: '', price: 0, quantity: 0 };

  constructor() {
    this.loadProducts();
  }

  loadProducts(): void {
    this.api.getProducts().subscribe({ next: products => this.products = products, error: () => this.error = 'The API is unavailable. Start the backend and try again.' });
  }

  login(): void {
    this.api.login(this.email, this.password).subscribe({
      next: response => { localStorage.setItem('product-management-token', response.token); this.isLoggedIn = true; this.notice = 'Signed in.'; },
      error: () => this.error = 'Login failed. Check the credentials.'
    });
  }

  register(): void {
    this.api.register(this.email, this.password).subscribe({
      next: response => { this.notice = 'User Created.'; },
      error: () => this.error = 'Registration failed.'
    });
  }

  saveProduct(): void {
    const request = this.selectedId ? this.api.updateProduct(this.selectedId, this.draft) : this.api.createProduct(this.draft);
    request.subscribe({
      next: () => { this.notice = this.selectedId ? 'Product updated.' : 'Product created.'; this.resetForm(); this.loadProducts(); },
      error: () => this.error = 'You must be signed in and provide valid product data.'
    });
  }

  edit(product: Product): void {
    this.selectedId = product.id;
    this.draft = { name: product.name, price: product.price, quantity: product.quantity };
  }

  remove(product: Product): void {
    if (!confirm(`Delete ${product.name}?`)) return;
    this.api.deleteProduct(product.id).subscribe({ next: () => { this.notice = 'Product deleted.'; this.loadProducts(); }, error: () => this.error = 'Delete failed.' });
  }

  resetForm(): void {
    this.selectedId = null;
    this.draft = { name: '', price: 0, quantity: 0 };
  }

  logout(): void {
    localStorage.removeItem('product-management-token');
    this.isLoggedIn = false;
  }

  dismissError(): void {
    this.error = '';
  }

  dismissNotice(): void {
    this.notice = '';
  }
}
