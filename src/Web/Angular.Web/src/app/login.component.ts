import { Component, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { environment } from '../environments/environment';
import { TokenService } from './auth/token.service';
import { Router } from '@angular/router';

@Component({
  standalone: true,
  selector: 'app-login',
  imports: [FormsModule],
  template: `
    <main style="font-family: system-ui; padding: 2rem; max-width: 420px;">
      <h2>Sign in</h2>
      <form (ngSubmit)="login()">
        <div style="margin:.5rem 0;">
          <label>Email</label><br>
          <input [(ngModel)]="email" name="email" type="email" required />
        </div>
        <div style="margin:.5rem 0;">
          <label>Password</label><br>
          <input [(ngModel)]="password" name="password" type="password" required />
        </div>
        <button type="submit">Login</button>
      </form>

      <p *ngIf="error()" style="color:#c00; margin-top:.5rem;">{{ error() }}</p>
    </main>
  `
})
export class LoginComponent {
  email = '';
  password = '';
  error = signal<string | null>(null);
  private base = environment.apiBaseUrl;

  constructor(
    private _http: HttpClient,
    private _tokens: TokenService,
    private _router: Router
  ) {}

  login() {
    this.error.set(null);
    this._http.post<{ accessToken: string }>(`${this.base}/api/auth/login`, {
      email: this.email,
      password: this.password
    }).subscribe({
      next: (r) => {
        if (r?.accessToken) {
          this._tokens.setToken(r.accessToken);
          this._router.navigateByUrl('/'); // go home
        } else {
          this.error.set('No token returned.');
        }
      },
      error: (e) => this.error.set(e?.error ?? 'Login failed.')
    });
  }
}
