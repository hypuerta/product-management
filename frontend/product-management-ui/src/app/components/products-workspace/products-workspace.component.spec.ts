import { signal } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of, throwError } from 'rxjs';
import { AuthService } from '../../features/auth/services/auth.service';
import { Product, ProductInput } from '../../features/products/models/product.model';
import { ProductsService } from '../../features/products/services/products.service';
import { ProductsWorkspaceComponent } from './products-workspace.component';

describe('ProductsWorkspaceComponent', () => {
  let fixture: ComponentFixture<ProductsWorkspaceComponent>;
  let component: ProductsWorkspaceComponent;
  let productsService: jasmine.SpyObj<ProductsService>;

  beforeEach(async () => {
    productsService = jasmine.createSpyObj<ProductsService>('ProductsService', [
      'getProducts',
      'createProduct',
      'updateProduct',
      'deleteProduct'
    ]);
    productsService.getProducts.and.returnValue(of([]));

    await TestBed.configureTestingModule({
      imports: [ProductsWorkspaceComponent],
      providers: [
        { provide: ProductsService, useValue: productsService },
        { provide: AuthService, useValue: { isLoggedIn: signal(true) } }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ProductsWorkspaceComponent);
    component = fixture.componentInstance;
  });

  it('loads products on construction', () => {
    expect(productsService.getProducts).toHaveBeenCalled();
  });

  it('copies a product into the editor', () => {
    const product: Product = { id: '5', name: 'Laptop', price: 800, quantity: 4 };

    component.onEdit(product);

    expect(component.selectedId).toBe('5');
    expect(component.draft).toEqual({ name: 'Laptop', price: 800, quantity: 4 });
  });

  it('creates a new product and resets the editor', () => {
    const expectedDraft: ProductInput = { name: 'Mouse', price: 25, quantity: 2 };
    component.draft = { ...expectedDraft };
    productsService.createProduct.and.returnValue(of({ id: '10', ...expectedDraft }));

    component.onSave();

    expect(productsService.createProduct).toHaveBeenCalledWith(expectedDraft);
    expect(component.notice).toBe('Product created.');
    expect(component.draft).toEqual({ name: '', price: 0, quantity: 0 });
  });

  it('updates an existing product', () => {
    const expectedDraft: ProductInput = { name: 'Monitor', price: 199, quantity: 1 };
    component.selectedId = '7';
    component.draft = { ...expectedDraft };
    productsService.updateProduct.and.returnValue(of({ id: '7', ...expectedDraft }));

    component.onSave();

    expect(productsService.updateProduct).toHaveBeenCalledWith('7', expectedDraft);
    expect(component.notice).toBe('Product updated.');
  });

  it('shows an error when saving fails', () => {
    productsService.createProduct.and.returnValue(throwError(() => new Error('bad request')));

    component.onSave();

    expect(component.error).toBe('You must be signed in and provide valid product data.');
  });
});
