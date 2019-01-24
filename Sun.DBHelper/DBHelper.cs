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
