# Dawn
2018/12/05
## 目標
CRUD-新增

## 專案初始
建置專案
```shell
$ git clone https://github.com/hyflamewow/Dawn.git
Dawn$ git checkout -b lab webapi2
Moon$ npm i
Sun$ dotnet restore
```
## 除錯模式
```shell
Sun$ dotnet watch run
Moon$ npm start
```
瀏覽 http://localhost:4200/
## 修改Sun專案
### 安裝套件
```shell
Sun$ dotnet add package System.Data.SQLite.Core --version 1.0.109.2
Sun$ dotnet add package Dapper --version 1.50.5
Sun$ dotnet add package NLog --version 4.5.11
```
### 新增DB/DBConfig.cs
```cs
namespace Sun.DB
{
    public class DBConfig
    {
        public string Driver { get; set; }
        public string ConnStr { get; set; }
    }
}
```
### 新增DB/Entity/TicketRow.cs
```cs
using System;

namespace Sun.DB.Entity
{
    public class TicketRow
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Seat { get; set; }
        public double Amount { get; set; }
        public DateTime DateTime { get; set; }
    }
}
```
### 新增DB/DBHelper.cs
```cs
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using Dapper;
using Sun.DB.Entity;

namespace Sun.DB
{
    public class DBHelper
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        private Dictionary<string, DBConfig> _config;
        public DBHelper(Dictionary<string, DBConfig> config)
        {
            _config = config;
        }
        private DbConnection CreateConn(string key)
        {
            DBConfig dbConfig = _config[key];
            DbConnection conn = null;
            if (dbConfig.Driver.ToUpper() == "SQLITE")
            {
                conn = new SQLiteConnection($"data source={dbConfig.ConnStr}");
            }
            return conn;
        }
        public void CreateDB()
        {
            DBConfig dbConfig = _config["MainDB"];
            if (!File.Exists(dbConfig.ConnStr))
            {
                DbConnection conn = CreateConn("MainDB");
                try
                {
                    string strSQL = $@"
CREATE TABLE Ticket (
    ID INTEGER NOT NULL,
    Name TEXT NOT NULL,
    Seat TEXT NOT NULL,
    Amount REAL NOT NULL,
    DateTime DATETIME NOT NULL)
";
                    conn.Execute(strSQL);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "CreateDB");
                    throw ex;
                }
                finally
                {
                    conn?.Close();
                }
            }
        }
        #region Ticket
        public List<TicketRow> GetTicketList()
        {
            DbConnection conn = CreateConn("MainDB");
            try
            {
                string strSQL = $@"
SELECT * FROM Ticket
";
                var list = conn.Query<TicketRow>(strSQL).ToList();
                return list;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "GetTicketList");
                throw ex;
            }
            finally
            {
                conn?.Close();
            }
        }
        public void InsertTicket(TicketRow ticket)
        {
            DbConnection conn = CreateConn("MainDB");
            try
            {
                string strSQL = $@"
INSERT INTO Ticket
       ( ID,  Name,  Seat,  Amount,  DateTime)
VALUES (@ID, @Name, @Seat, @Amount, @DateTime)
";
                conn.Execute(strSQL, ticket);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "InsertTicket");
                throw ex;
            }
            finally
            {
                conn?.Close();
            }
        }
        #endregion Ticket
    }
}
```
### 新增Controllers/TicketController.cs
```cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Sun.DB;
using Sun.DB.Entity;

namespace Sun.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        private DBHelper _dbHelper;
        public TicketController(DBHelper dbHelper)
        {
            _dbHelper = dbHelper;
            _dbHelper.CreateDB();
        }
        [HttpGet]
        public ActionResult<IEnumerable<TicketRow>> Get()
        {
            var list = _dbHelper.GetTicketList();
            return Ok(list);
        }

        [HttpPost()]
        public void Post([FromBody] TicketRow ticket)
        {
            // #日期要換成本地時間
            ticket.DateTime = ticket.DateTime.ToLocalTime();
            _dbHelper.InsertTicket(ticket);
        }
    }
}
```
### 新增ConfigFiles.cs
```cs
namespace Sun
{
    public class ConfigFiles
    {
        public string NLogConfig { get; set; }
        public string DBConfig { get; set; }
    }
}
```
### 修改Startup.cs
這是為了WebAPI產Json時不要將屬性自動轉為小寫開頭的camel格式, 改寫後Json就會保持原名。
```cs
public void ConfigureServices(IServiceCollection services)
{
    services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
        .AddJsonOptions(options =>
        {   // #維持屬性名稱大小寫
            options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
        });
    ConfigFiles configFiles = Configuration.GetSection("ConfigFiles").Get<ConfigFiles>();
    // #目前不考慮整合ASP.NET的Log機制, 因為看不出優點, 單純用NLog就夠了。
    NLog.LogManager.Configuration = new XmlLoggingConfiguration(configFiles.NLogConfig);
    var dbConfig = JsonConvert.DeserializeObject<Dictionary<string,DBConfig>>(File.ReadAllText(configFiles.DBConfig));
    // #將DBHelper放入DI
    services.AddSingleton<DBHelper>(new DBHelper(dbConfig));
}
```
### 新增Config/nlog.config
```xml
<?xml version="1.0" encoding="utf-8" ?>
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
      xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <targets>
    <target name="logfile" xsi:type="File" fileName="Logs/${date:format=yyyyMMdd}-nlog.log"
            maxArchiveFiles="10"
            archiveAboveSize="1000000"
            archiveEvery="Day">
      <layout xsi:type="JsonLayout">
        <attribute name="TimeStamp" layout="${longdate}" />
        <attribute name="Level" layout="${level:uppercase=true}" />
        <attribute name="LoggerName" layout="${logger}" />
        <attribute name="Message" layout="${message}" escapeUnicode="false" />
        <attribute name="Exception" layout="${exception:format=ToString}" escapeUnicode="false" />
      </layout>
    </target>
    <target xsi:type="Null" name="blackhole" />
  </targets>
  <rules>
    <logger name="Microsoft.*" minlevel="Debug" writeTo="blackhole" final="true" />
    <logger name="*" minlevel="Debug" writeTo="logfile" />
  </rules>
</nlog>
```
### 新增Config/dbConfig.json
```json
{
    "MainDB": {
        "Driver": "SQLITE",
        "ConnStr": "Movies.sqlite"
    }
}
```
### 修改appsettings.json
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConfigFiles": {
    "NLogConfig": "Config/nlog.config",
    "DBConfig": "Config/dbConfig.json"
  }
}
```
### 修改Sun.csproj, 加上
```xml
  <ItemGroup>
    <Content Update="Config\nlog.config">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </Content>
  </ItemGroup>
  <ItemGroup>
    <Content Update="Config\dbConfig.json">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </Content>
  </ItemGroup>
```
### .gitignore, 加上
```sh
# DB
*.sqlite
```
## 修改Moon專案
### 建立component、Service及Elf
```shell
Moon$ ng g c ticket
src\app\ticket$ ng g s ticket
src\app\ticket$ ng g class ticketElf
```
### 修改app.module.ts
```ts
import { ReactiveFormsModule } from '@angular/forms';
...
imports: [
    ...
    ReactiveFormsModule
  ],
```
### src\app\ticket\ticket-elf.ts
```ts
export class TicketElf {
    ID: number;
    Name: string;
    Seat: string;
    Amount: number;
    DateTime: Date;
}
```
### src\app\ticket\ticket.service.ts
```ts
import { Injectable } from '@angular/core';
import { HttpHeaders, HttpClient } from '@angular/common/http';
import { MessageService } from '../message.service';
import { TicketElf } from './ticket-elf';
import { catchError } from 'rxjs/operators';
import { Observable, of } from 'rxjs';
import { FormBuilder, Validators } from '@angular/forms';

const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};

@Injectable({
  providedIn: 'root'
})
export class TicketService {

  constructor(private http: HttpClient, private messageService: MessageService) { }

  save(row: TicketElf) {
    return this.http.post('api/ticket', row, httpOptions)
      .pipe(
        catchError(this.handleError('save', {}))
      );
  }
  getList(): Observable<TicketElf[]> {
    return this.http.get<TicketElf[]>('api/ticket')
      .pipe(
        catchError(this.handleError('getList', []))
      );
  }
  private handleError<T>(operation = 'operation', result?: T) {
    return (error: any): Observable<T> => {
      switch (error.status) {
        case 200: // #URL錯了
          this.log('200服務不存在');
          break;
        // case 204: // #NoContent
        //   break;
        case 400: // #BadRequest
          this.log('400服務不存在');
          break;
        case 404: // #NotFound
          this.log('404服務不存在');
          break;
        case 500: // #Excpetion
          this.log('500服務發生未知錯誤');
          break;
        case 504: // #Server沒啟動
          this.log('504伺服器無回應');
          break;
        default: // #其他
          this.log(`${operation} failed: ${error.message}`);
          break;
      }
      return of(result as T);
    };
  }
  private log(message: string) {
    this.messageService.add(`ValuesService: ${message}`);
  }
}
```
### src\app\ticket\ticket.component.ts
```ts
import { Component, OnInit } from '@angular/core';
import { TicketService } from './ticket.service';
import { TicketElf } from './ticket-elf';
import { FormBuilder, Validators } from '@angular/forms';

@Component({
  selector: 'app-ticket',
  templateUrl: './ticket.component.html',
  styleUrls: ['./ticket.component.scss']
})
export class TicketComponent implements OnInit {

  mainForm = this.fb.group({
    ID: [0, Validators.required],
    Name: ['', Validators.required],
    Seat: ['', Validators.required],
    Amount: [0, Validators.required],
    DateTime: ['', Validators.required]
  });

  constructor(private fb: FormBuilder, private ticketService: TicketService) { }
  list: TicketElf[] = [];
  ngOnInit() {
    this.resetForm();
    this.getList();
  }
  getList() {
    this.ticketService.getList()
      .subscribe(list => {
        list.forEach(p => {
          // #日期要轉型
          p.DateTime = new Date(p.DateTime);
        });
        this.list = list;
      });
  }
  save() {
    this.ticketService.save(this.mainForm.value)
      .subscribe(_ => this.getList());
  }
  resetForm() {
    this.mainForm.patchValue({
      ID: 0,
      Name: '',
      Seat: '',
      Amount: 0,
      DateTime: new Date()
    });
  }
}
```
### src\app\ticket\ticket.component.html
```html
<ul>
  <li *ngFor="let item of list">
    {{item.ID}}, {{item.Name}}, {{item.Seat}}, {{item.Amount | currency:'TWD':'symbol-narrow':'1.0-0'}},
    {{item.DateTime|date:'yyyy-MM-dd HH:mm:ss'}}
  </li>
</ul>
<form [formGroup]="mainForm">
  <label>ID:
    <input type="number" formControlName="ID">
  </label><br>
  <label>Name:
    <input type="text" formControlName="Name">
  </label><br>
  <label>Seat:
    <input type="text" formControlName="Seat">
  </label><br>
  <label>Amount:
    <input type="number" formControlName="Amount">
  </label><br>
  <label>DateTime:
    <input type="datetime" formControlName="DateTime">
  </label>
</form>
<button (click)="save()">Save</button>
```
### src\app\app-routing.module.ts
```ts
import { TicketComponent } from './ticket/ticket.component';

const routes: Routes = [
  { path: '', redirectTo: '/ticket', pathMatch: 'full' },
  { path: 'values', component: ValuesComponent },
  { path: 'ticket', component: TicketComponent }
];
```
## 測試
新增及列表
## 版控
```shell
Dawn$ git add .
Dawn$ git commit -m "crud1"
Dawn$ git tag crud1
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
Dawn$ git checkout -b lab crud1
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
Dawn$ git checkout master
Dawn$ branch -d lab
```