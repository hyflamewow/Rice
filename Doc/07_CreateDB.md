# Rice
2019/01/24
## 目標
建立資料庫

## 專案初始
建置專案
```shell
$ git clone https://github.com/hyflamewow/Rice.git
Rice$ git checkout -b lab sln
Moon$ npm i
Sun$ dotnet restore
```
## 除錯模式
```shell
Sun$ dotnet watch run
Moon$ npm start
```
瀏覽 http://localhost:5000/api/auth

## 安裝套件
```shell
Sun.DBHelper$ dotnet add package Dapper --version 1.50.5
Sun.DBHelper$ dotnet add package NLog --version 4.5.11
Sun$ dotnet add package NLog --version 4.5.11
```
## 修改Sun.DBHelper
### 新增DBHelperConfig.cs
```cs
namespace Sun.DB
{
    public class DBHelperConfig
    {
        public string Driver { get; set; }
        public string ConnStr { get; set; }
    }
}
```
### 修改DBHelper.cs
```cs
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using CLN.DB.Table;
using Dapper;

namespace Sun.DB
{
    public class DBHelper
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        private Dictionary<string, DBHelperConfig> _config;
        public DBHelper(Dictionary<string, DBHelperConfig> config)
        {
            this._config = config;
        }
        private DbConnection CreateDBConnection(string key)
        {
            DBHelperConfig dbConfig = _config[key];

            DbConnection conn = null;

            switch (dbConfig.Driver.ToUpper())
            {
                case "MSSQL":
                    conn = new SqlConnection(dbConfig.ConnStr);
                    break;
                case "MYSQL":
                    // conn = new MySqlConnection(dbConfig.ConnStr);
                    break;
                case "SQLITE":
                    // conn = new SQLiteConnection(dbConfig.ConnStr);
                    break;
            }

            return conn;
        }
        public List<string> Auth_ListAll()
        {
            return new List<string> { "Peter", "Mandy" };
        }
        public void CreateDB()
        {
            string profile = Environment.ExpandEnvironmentVariables("%userprofile%");
            DbConnection conn = CreateDBConnection("CreateDB");
            try
            {
                string strSQL = $@"
CREATE DATABASE [Sun]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'Sun', FILENAME = N'{profile}\Sun.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'Sun_log', FILENAME = N'{profile}\Sun_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
";
                conn.Execute(strSQL);
                strSQL = $@"
CREATE TABLE [Sun].[dbo].[Auth_User](
	[Account] [varchar](50) NOT NULL,
CONSTRAINT [PK_Auth_User] PRIMARY KEY CLUSTERED 
(
	[Account] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
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
}
```
## 修改Sun專案
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
```cs
using Newtonsoft.Json;
using NLog.Config;
using Sun.DB;
...
public void ConfigureServices(IServiceCollection services)
{
  ...
    services.Configure<MvcOptions>(options =>
    {
        options.Filters.Add(new CorsAuthorizationFilterFactory("CorsPolicy"));
    });
    ConfigFiles configFiles = Configuration.GetSection("ConfigFiles").Get<ConfigFiles>();
    // #目前不考慮整合ASP.NET的Log機制, 因為看不出優點, 單純用NLog就夠了。
    NLog.LogManager.Configuration = new XmlLoggingConfiguration(configFiles.NLogConfig);
    var dbConfig = JsonConvert.DeserializeObject<Dictionary<string, DBHelperConfig>>(File.ReadAllText(configFiles.DBConfig));
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
    "CreateDB": {
        "Driver": "MSSQL",
        "ConnStr": "Server=(localdb)\\mssqllocaldb;Database=master;Trusted_Connection=True;MultipleActiveResultSets=true"
    },
    "MainDB": {
        "Driver": "MSSQL",
        "ConnStr": "Server=(localdb)\\mssqllocaldb;Database=MSSQLLocalDB;Trusted_Connection=True;MultipleActiveResultSets=true"
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
## 測試
```shell
Sun$ dotnet run
```
http://localhost:5000/api/auth/createdb
應該會在%userprofile%下產生Sun.mdf及Sun_log.ldf
## 版控
```shell
Rice$ git add .
Rice$ git commit -m "createdb"
Rice$ git tag createdb
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
Rice$ git checkout -b lab createdb
Moon$ npm i
Sun$ dotnet restore
Sun$ dotnet run
```
瀏覽  
http://localhost:5000/api/auth/createdb
應該會在%userprofile%下產生Sun.mdf及Sun_log.ldf

收尾
```shell
Rice$ git checkout master
Rice$ branch -d lab
```