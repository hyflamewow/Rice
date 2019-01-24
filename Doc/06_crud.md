# Rice
2019/01/24
## 目標
建立sln, 並建立dll專案參考

## 專案初始
建置專案
```shell
$ git clone https://github.com/hyflamewow/Rice.git
Rice$ git checkout -b lab cors
Moon$ npm i
Sun$ dotnet restore
```
## 除錯模式
```shell
Sun$ dotnet watch run
Moon$ npm start
```
瀏覽 http://localhost:4200/
## 建立Solution及Dll專案參考
```shell
Rice$ dotnet new sln
Rice$ dotnet new classlib -o Sun.DBHelper
Rice$ dotnet sln add **/*.csproj
Rich$ dotnet add Sun reference Sun.DB
```
## git
將Sun/.gitignore移到Rice目錄下
## 修改Sun.DBHelper專案
刪除Class1.cs
新增DBHelper.cs
```cs
using System;
using System.Collections.Generic;

namespace Sun.DB
{
    public class DBHelper
    {
        public List<string> Auth_ListAll()
        {
            return new List<string> {"Peter","Mandy"};
        }
    }
}
```
## 修改Sun專案
### 新增Controllers/AuthController.cs
```cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sun.DB;

namespace Sun.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            DBHelper p = new DBHelper();
            return p.Auth_ListAll();
        }
    }
}
```
## 測試
```shell
Sun$ dotnet run
```
http://localhost:5000/api/auth
## 版控
```shell
Rice$ git add .
Rice$ git commit -m "sln"
Rice$ git tag sln
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
Rice$ git checkout -b lab sln
Sun$ dotnet restore
Sun$ dotnet run
Moon$ npm start
```
瀏覽  
http://localhost:5000/api/auth

收尾
```shell
Rice$ git checkout master
Rice$ branch -d lab
```