import { Component, OnInit, signal } from '@angular/core';
import { HealthService, HealthDto } from './health.service';

@Component({
  selector: 'app-root',
  standalone: true,
  template: `
    <main style="font-family: system-ui; padding: 2rem;">
      <h1>Welcome to Virtuelly Angular Web</h1>
      <p>This is a minimal starting point.</p>

      <section style="margin-top:1rem; padding:1rem; border:1px solid #ddd;">
        <h3>API Gateway Health</h3>
        <button (click)="checkHealth()">Check Health</button>
        <div *ngIf="loading()">Checking...</div>
        <pre *ngIf="result() as r" style="white-space:pre-wrap; background:#f8f8f8; padding:.75rem;">
{{ r | json }}
        </pre>
        <div *ngIf="error()" style="color:#c00;">{{ error() }}</div>
      </section>
    </main>
  `
})
export class AppComponent implements OnInit {
  loading = signal(false);
  result = signal<HealthDto | null>(null);
  error = signal<string | null>(null);

  constructor(private _health: HealthService) {}

  ngOnInit(): void {
    this.checkHealth();
  }

  checkHealth(): void {
    this.loading.set(true);
    this.error.set(null);
    this.result.set(null);

    this._health.ping().subscribe({
      next: (r) => {
        this.result.set(r);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(`Health check failed: ${err?.message ?? 'Unknown error'}`);
        this.loading.set(false);
      }
    });
  }
}
