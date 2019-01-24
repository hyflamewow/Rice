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
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly DBHelper _dbHelper;
        public AuthController(DBHelper dbHelper)
        {
            this._dbHelper = dbHelper;
        }
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return this._dbHelper.Auth_ListAll();
        }
        [HttpGet("CreateDB")]
        public void CreateDB()
        {
            this._dbHelper.CreateDB();
        }
    }
}
