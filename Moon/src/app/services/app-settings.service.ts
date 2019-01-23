import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class AppSettingsService {
  AppSettings: AppSettings;
  constructor(private http: HttpClient) { }
  load() {
    return new Promise((resolve, reject) => {
      this.http.get<AppSettings>('assets/appsettings.json')
        .subscribe(settings => {
          this.AppSettings = settings;
          resolve(true);
        });
    });
  }
}
export class AppSettings {
  ApiUrlRoot: string;
}
