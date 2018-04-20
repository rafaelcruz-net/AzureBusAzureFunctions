using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BugTrackerWeb.Models;
using System.Data.SqlClient;
using Dapper;
using BugTrackerWeb.Publish;

namespace BugTrackerWeb.Controllers
{
    public class BugController : Controller
    {
        const string ConnectionString = "Server=tcp:thedevconfserver.database.windows.net,1433;Initial Catalog=thedevconfdb;Persist Security Info=False;User ID=samples;Password=123456#A;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        public IActionResult List()
        {
            using (var sqlConnection = new SqlConnection(ConnectionString))
            {
                var result = sqlConnection.Query<BugViewModel>("Select * from Bug");
                return View(result);
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(BugViewModel model)
        {
            ViewBag.Message = "Bug cadastrado com sucesso";

            var publish = new Publisher();
            publish.SendMessage(model).Wait();

            return View();
        }
    }
}