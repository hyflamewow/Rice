# Dawn
2018/12/06
## 目標
1. Angular組態
2. 啟用CORS
## 專案初始
建置專案
```shell
$ git clone https://github.com/hyflamewow/Dawn.git
Dawn$ git checkout -b lab crud1
Moon$ npm i
Sun$ dotnet restore
```
## 除錯模式
```shell
Sun$ dotnet watch run
Moon$ ng serve -o
```
瀏覽 http://localhost:4200/
## 修改 Moon
```shell
Moon$ ng g s services/appSettings
```
app-settings.service
```cs
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
新增 assets/appsettings.json
```json
{
    "ApiUrlRoot": "http://localhost:5000/api/"
}
```
修改 app.module.ts
```ts
import { NgModule, APP_INITIALIZER } from '@angular/core';
...
providers: [
    {
        provide: APP_INITIALIZER, useFactory: (
        service: AppSettingsService) => () => service.load(),
        deps: [AppSettingsService], multi: true
    },
],
```
修改 ticket.service.ts
```cs
import { AppSettingsService } from '../services/app-settings.service';
...
export class TicketService {
  private apiUrlRoot: string;
  constructor(private http: HttpClient
    , private messageService: MessageService
    , private appsettingsService: AppSettingsService) {
    this.apiUrlRoot = appsettingsService.AppSettings.ApiUrlRoot;
  }

  save(row: TicketElf) {
    return this.http.post(this.apiUrlRoot.concat('ticket'), row, httpOptions)
      .pipe(
        catchError(this.handleError('save', {}))
      );
  }
  getList(): Observable<TicketElf[]> {
    this.log(this.apiUrlRoot.concat('ticket'));
    return this.http.get<TicketElf[]>(this.apiUrlRoot.concat('ticket'))
      .pipe(
        catchError(this.handleError('getList', []))
      );
  }
  private handleError<T>(operation = 'operation', result?: T) {
    return (error: any): Observable<T> => {
      switch (error.status) {
          ...
        case 0: // #存取跨Domain的Server時, 伺服器沒啟動或是沒有啟用CORS
          this.log('0伺服器無回應');
          break;
        default: // #其他
          this.log(`${operation} status:${error.status} failed: ${error.message}`);
          break;
      }
      return of(result as T);
    };
  }
}
```
## 修改 Sun
Startup.cs
```cs
public void ConfigureServices(IServiceCollection services)
{
    services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
        .AddJsonOptions(options =>
        {   // #維持屬性名稱大小寫
            options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
        });
    services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy",
                builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
            );
        });
    services.Configure<MvcOptions>(options =>
    {
        options.Filters.Add(new CorsAuthorizationFilterFactory("CorsPolicy"));
    });
    ...
}

// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseHsts();
    }

    // app.UseHttpsRedirection();
    app.UseMvc();
    app.UseDefaultFiles();
    app.UseStaticFiles();
    // #使用CORS
    app.UseCors("CorsPolicy");
    // #實作SPA
    app.Run(async (context) =>
    {
        if (!Path.HasExtension(context.Request.Path.Value))
        {
            await context.Response.SendFileAsync(Path.Combine(env.WebRootPath, "index.html"));
        }
    });
}
```
## 測試
```shell
Sun$ dotnet watch run
Moon$ ng serve -o
```
瀏覽 http://localhost:4200/
## 版控
```shell
Dawn$ git add .
Dawn$ git commit -m "cors"
Dawn$ git tag cors
```
## 收尾
```sehll
Dawn$ git checkout master
Dawn$ branch -d lab
```
## 完成品測試
建置專案
```shell
$ git clone https://github.com/hyflamewow/Dawn.git
Dawn$ git checkout -b lab cors
Sun$ dotnet restore
Sun$ dotnet run
Moon$ npm i
Moon$ ng serve -o
```
瀏覽  
http://localhost:4200/  
收尾
```shell
Dawn$ git checkout master
Dawn$ branch -d lab
```