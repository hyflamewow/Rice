# Rice
2019/01/24
## 目標
1. Angular組態
2. 啟用CORS
## 專案初始
建置專案
```shell
$ git clone https://github.com/hyflamewow/Rice.git
Rice$ git checkout -b lab appsettings
Moon$ npm i
Sun$ dotnet restore
```
## 除錯模式
```shell
Sun$ dotnet watch run
Moon$ npm run startC
```
自動瀏覽 http://localhost:4200/

## 修改 Sun
因為會使用CORS表示網頁不在同一個站台,
所以將程式還原為純WebAPI不提供網頁服務
Startup.cs
```cs
using Microsoft.AspNetCore.Mvc.Cors.Internal;
...
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
    app.UseCors("CorsPolicy");
    // app.UseDefaultFiles();
    // app.UseStaticFiles();
    // app.Run(async (context) =>
    // {
    //     if (!Path.HasExtension(context.Request.Path.Value))
    //     {
    //         await context.Response.SendFileAsync(Path.Combine(env.WebRootPath, "index.html"));
    //     }
    // });
}
```
## 修改 Moon
修改 assets/appsettings.json
```json
{
    "ApiUrlRoot": "http://localhost:5000/api/",
    "ApiUrlRoot_local": "api/"
}
```
## 測試
```shell
Sun$ dotnet run
Moon$ npm start
```
自動瀏覽 http://localhost:4200/
## 版控
```shell
Rice$ git add .
Rice$ git commit -m "cors"
Rice$ git tag cors
```
## 收尾
```sehll
Rice$ git checkout master
Rice$ branch -d lab
```
## 完成品測試
建置專案
```shell
$ git clone https://github.com/hyflamewow/Rice.git
Rice$ git checkout -b lab cors
Sun$ dotnet restore
Sun$ dotnet run
Moon$ npm i
Moon$ npm start
```
自動瀏覽  
http://localhost:4200/  
收尾
```shell
Rice$ git checkout master
Rice$ branch -d lab
```