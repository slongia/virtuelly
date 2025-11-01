import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../environments/environment';
import { Observable } from 'rxjs';

export interface HealthDto {
  service?: string;
  status?: string;
  timestampUtc?: string;
}

@Injectable({ providedIn: 'root' })
export class HealthService {
  private readonly base = environment.apiBaseUrl;

  constructor(private _http: HttpClient) {}

  ping(): Observable<HealthDto> {
    return this._http.get<HealthDto>(`${this.base}/health`);
  }
}
