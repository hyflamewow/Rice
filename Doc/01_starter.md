# Rice
2019/01/23
## 目標
Angular with ASP.NET Core
## 環境
1. NodeJS: v10.15.0
1. NET Core SDK: 2.2.103
1. @angular/cli : v7.2.2
## 檢查
1. $ node --version
1. $ dotnet --version
1. $ ng --version
## 開發工具
1. Visual Studio Code
1. 安裝C#
## 建立專案目錄
建立Rice目錄
## 建立Moon專案
```shell
Rice$ ng new Moon --skip-install --routing --style=scss
Moon$ rm -rf .git
```
## 建立Sun專案
```shell
Rice$ dotnet new webapi -o Sun
Rice$ dotnet dev-certs https --trust
Sun$ curl https://raw.githubusercontent.com/github/gitignore/master/VisualStudio.gitignore --output .gitignore
```
## 加入版控
```shell
Rice$ git init
Rice$ git add .
Rice$ git commit -m "first commit"
```
## 調整Moon程式
修改Moon/angular.json，為了輸出到ASP.NET Core的專案。
```json
"outputPath": "../Sun/wwwroot",
```
新增proxy.conf.json，為了Debug使用
```json
{
  "/api":{
    "target": "http://localhost:5000",
    "secure": false
  }
}
```
修改package.json
```json
{
  "scripts": {
    "start": "ng serve --proxy-config proxy.conf.json -o",
    "build": "ng build --prod",
  },
```
修改src/index.html，為了站台不一定在Root下的問題。
```html
<base href="./">
```
修改 Moon/.gitignore  
找到VSCode
```sh
# IDE - VSCode
.vscode/
```
## 調整Sun程式
修改Startup.cs
1. 為了WebAPI產Json時將屬性保持原名。
2. 為了實現SPA設計。
```cs
using System.IO;
using Microsoft.AspNetCore.Http;
....
public void ConfigureServices(IServiceCollection services)
{
    services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
        .AddJsonOptions(options =>
        {
            options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
        });
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
    app.Run(async (context) =>
    {
        if (!Path.HasExtension(context.Request.Path.Value))
        {
            await context.Response.SendFileAsync(Path.Combine(env.WebRootPath, "index.html"));
        }
    });
    app.UseMvc();
}
```
修改 Sun/.gitignore
1. 找到 #wwwroot/，去掉#。
1. 加上VSCode。
```sh
# Uncomment if you have tasks that create the project's static files in wwwroot
wwwroot/
...
# IDE - VSCode
.vscode/
```
## 版控
```shell
Rice$ git add .
Rice$ git commit -m "starter"
Rice$ git tag starter
```
## 開始建置
```shell
Moon$ npm i
Moon$ npm run build
Sun$ dotnet restore
Sun$ dotnet run
```
如果設定了, 想要使用最新版。
```shell
$ npm config set prefer-offline true
```
可改用以下指令更新套件
```shell
Moon$ npm update
Moon$ npm update -D
Moon$ npm run build
Sun$ dotnet restore
Sun$ dotnet run
```
Ctrl+C 結束程式
### 瀏覽
http://localhost:5000/  
https://localhost:5001/

## 完成品測試
建置專案
```shell
$ git clone https://github.com/hyflamewow/Rice.git
Rice$ git checkout -b lab starter
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