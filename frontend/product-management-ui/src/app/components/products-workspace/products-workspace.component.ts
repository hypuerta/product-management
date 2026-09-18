import { CurrencyPipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../features/auth/services/auth.service';
import { Product, ProductInput } from '../../features/products/models/product.model';
import { ProductsService } from '../../features/products/services/products.service';

@Component({
  selector: 'app-products-workspace',
  imports: [FormsModule, CurrencyPipe],
  templateUrl: './products-workspace.component.html',
  styleUrl: './products-workspace.component.scss'
})
export class ProductsWorkspaceComponent {
  private readonly productsService = inject(ProductsService);
  private readonly authService = inject(AuthService);
  readonly isLoggedIn = this.authService.isLoggedIn;
  products: Product[] = [];
  selectedId: string | null = null;
  draft: ProductInput = { name: '', price: 0, quantity: 0 };
  notice = '';
  error = '';

  constructor() {
    this.loadProducts();
  }

  loadProducts(): void {
    this.productsService.getProducts().subscribe({
      next: products => { this.products = products; this.error = ''; },
      error: () => this.error = 'The API is unavailable. Start the backend and try again.'
    });
  }

  onEdit(product: Product): void {
    this.selectedId = product.id;
    this.draft = { name: product.name, price: product.price, quantity: product.quantity };
  }

  onRemove(product: Product): void {
    if (!confirm(`Delete ${product.name}?`)) return;
    this.productsService.deleteProduct(product.id).subscribe({
      next: () => { this.notice = 'Product deleted.'; this.loadProducts(); },
      error: () => this.error = 'Delete failed.'
    });
  }

  onSave(): void {
    const request = this.selectedId
      ? this.productsService.updateProduct(this.selectedId, this.draft)
      : this.productsService.createProduct(this.draft);

    request.subscribe({
      next: () => {
        this.notice = this.selectedId ? 'Product updated.' : 'Product created.';
        this.onReset();
        this.loadProducts();
      },
      error: () => this.error = 'You must be signed in and provide valid product data.'
    });
  }

  onReset(): void {
    this.selectedId = null;
    this.draft = { name: '', price: 0, quantity: 0 };
  }

  dismissError(): void {
    this.error = '';
  }

  dismissNotice(): void {
    this.notice = '';
  }
}
