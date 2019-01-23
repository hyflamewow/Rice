# Dawn
2018/12/06
## 目標
1. JWT
## 專案初始
建置專案
```shell
$ git clone https://github.com/hyflamewow/Dawn.git
Dawn$ git checkout -b lab cors
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

## 修改 Sun

## 測試
```shell
Sun$ dotnet watch run
Moon$ ng serve -o
```
瀏覽 http://localhost:4200/
## 版控
```shell
Dawn$ git add .
Dawn$ git commit -m jwt
Dawn$ git tag jwt
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
Dawn$ git checkout -b lab jwt
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