# Rice
2019/01/24
## 目標
建置Electron.NET  
單機版以Web開發桌機程式

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
**確認程式正確後關閉**

瀏覽 http://localhost:4200/
## 建立分支
因為Electron.NET不知道未來是否會持續更新,
所以不在主流設計, 採用分支的方式說明建置方式
```shell
Rice$ git checkout -b electronnet
```
## 加入Electron.NET
```shell
Sun$ dotnet add package ElectronNET.API --version 0.0.11
```
## 安裝Electron.NET的CLI
```shell
$ dotnet tool install --global ElectronNET.CLI --version 0.0.11
```
## 初始環境
```shell
Sun$ electronize init
```
## 修改Sun專案
Program.cs
```cs
public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseElectron(args)
                .UseStartup<Startup>();
```
Startup.cs
```cs
app.Run(async (context) =>
{
    if (!Path.HasExtension(context.Request.Path.Value))
    {
        await context.Response.SendFileAsync(Path.Combine(env.WebRootPath, "index.html"));
    }
});

Task.Run(async () => await Electron.WindowManager.CreateWindowAsync());
```
## 測試
```cs
Moon$ npm run build
Sun$ electronize start
```
## 問題
程式修改後測試會沒有異動, 原因是Cache的關係,
1. 視窗鍵->shell:appdata
2. 將Electron目錄刪除

或是直接刪除  
C:\Users\<user>\AppData\Roaming\Electron  

##建置
```shell
Sun> electronize build /target win
```
程式會在bin/desktop/electron.net.host-win32-x64  
執行electron.net.host.exe

## 版控
```shell
Rice$ git add .
Rice$ git commit -m "electronnet"
Rice$ git tag electronnet
```
## 收尾
```shell
Rice$ git checkout master
```
## 完成品測試
建置專案
```shell
$ dotnet tool install --global ElectronNET.CLI --version 0.0.11
$ git clone https://github.com/hyflamewow/Rice.git
Rice$ git checkout -b lab electronnet
Moon$ npm i
Moon$ npm run build
Sun$ dotnet restore
Sun$ electronize start
```
佈署
```shell
Sun> electronize build /target win
```
程式會在bin/desktop/electron.net.host-win32-x64  
執行electron.net.host.exe
