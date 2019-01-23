# Rice
2019/01/23
## 目標
讀取組態檔appsettings.json

## 專案初始
建置專案
```shell
$ git clone https://github.com/hyflamewow/Rice.git
Rice$ git checkout -b lab webapi
Moon$ npm i
Sun$ dotnet restore
```
## 除錯模式
```shell
Sun$ dotnet watch run
Moon$ npm start
```
瀏覽 http://localhost:4200/

**程式不用關閉, 直接修改程式**
## 修改Moon專案
### 新增組態檔 assets\appsettings.json
```json
{
    "ApiUrlRoot": "api/"
}
```
### 建立appSettings Service
```shell
Moon$ ng g s services/appSettings
```
### services\app-settings.service.ts
```ts
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
```
### app\app.module.ts
```ts
import { NgModule, APP_INITIALIZER } from '@angular/core';
import { AppSettingsService } from './services/app-settings.service';
import { HttpClientModule } from '@angular/common/http';
...
@NgModule({
  ...
  imports: [
    ...
    HttpClientModule
  ],
  providers: [
    {
      provide: APP_INITIALIZER, useFactory: (
        service: AppSettingsService) => () => service.load(),
      deps: [AppSettingsService], multi: true
    },
  ],
  bootstrap: [AppComponent]
})
```
### 讀取組態檔 totem\values\values.component.ts
```ts
constructor(
    appsettingsService: AppSettingsService,
    private http: HttpClient) {
    this.apiUrlRoot = appsettingsService.AppSettings.ApiUrlRoot;
  }
```
## 版控
```shell
Rice$ git add .
Rice$ git commit -m "appsettings"
Rice$ git tag appsettings
```
## 收尾
```shell
Rice$ git checkout master
Rice$ branch -d lab
```
## 完成品測試
建置專案
```shell
$ git clone https://github.com/hyflamewow/Rice.git
Rice$ git checkout -b lab appsettings
Moon$ npm i
Moon$ npm run build
Sun$ dotnet restore
Sun$ dotnet run
```
瀏覽  
http://localhost:5000/  
https://localhost:5001/

收尾
```shell
Rice$ git checkout master
Rice$ branch -d lab
```