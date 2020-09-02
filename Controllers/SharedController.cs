using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcUser.Data;
using MvcUser.Models;
using Microsoft.AspNetCore.Hosting;
using System.Text;
using System.Net.Mail;
using Microsoft.AspNetCore.Http;
using MySqlConnector;
namespace MvcUser.Controllers
{   [Route("Shared")]
    public abstract class SharedController : Controller
    {
        private readonly AppDb Db;

        private readonly IWebHostEnvironment _webHostEnvironment;

        public SharedController(AppDb db = null, IWebHostEnvironment webHostEnvironment = null)
        {
            Db = db;
            _webHostEnvironment = webHostEnvironment;
        }
        [Route("Layout")]
        public IActionResult _Layout()
        {
            List<Sidebar> listSidebar = new List<Sidebar>();

            Db.Connection.Open();
            MySqlCommand cmd = new MySqlCommand("select * from Sidebar", Db.Connection);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    listSidebar.Add(new Sidebar()
                    {
                        Id = reader.GetInt32("Id"),
                        Name = reader.GetString("Name"),
                        Link = reader.GetString("Link"),
                        Parent = reader.GetString("Parent"),
                        Value = reader.GetString("Value")
                    });
                }
            }
            ViewBag.link = listSidebar.OrderBy(s => s.Value).ToList();
            return View();
        }
    }
}
