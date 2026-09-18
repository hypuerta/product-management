import { Component } from '@angular/core';
import { AuthPanelComponent } from './auth-panel/auth-panel.component';
import { ProductsWorkspaceComponent } from './products-workspace/products-workspace.component';

@Component({
  selector: 'app-root',
  imports: [AuthPanelComponent, ProductsWorkspaceComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
}
