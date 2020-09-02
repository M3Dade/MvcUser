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
using Newtonsoft.Json;
using System.Web;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.AspNetCore.Hosting;
using System.Text;
using System.Net.Mail;
using Microsoft.AspNetCore.Http;
using MySqlConnector;
using SQLitePCL;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Principal;
using MvcUser.Controllers;

namespace MvcUser.Controllers
{
    public class UsersController : Controller
    {
        
        private readonly AppDb Db;

        private readonly IWebHostEnvironment _webHostEnvironment;
        
        public UsersController(AppDb db, IWebHostEnvironment webHostEnvironment = null)
        {
            Db = db;
            _webHostEnvironment = webHostEnvironment;
 
        }
        
        public IActionResult Test()
        {
            return View();
        }
        public  IActionResult Users(string sortOrder,
                                    string currentFilter,
                                    string searchString,
                                    int? pageNumber)   //加载登陆
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["LevelSortParm"] = sortOrder == "level" ? "level_desc" : "level";
            //ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;
   
            List<People> list = new List<People>();
            List<Sidebar> listSidebar = new List<Sidebar>();
           
            Db.Connection.Open();
            MySqlCommand cmd = new MySqlCommand("select * from People", Db.Connection);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                //读取数据
                while (reader.Read())
                {
                    list.Add(new People()
                    {
                        ID = reader.GetInt32("ID"),
                        Name = reader.GetString("Name"),
                        Level = reader.GetString("Level"),
                        Phone = reader.GetString("Phone"),
                    });
                }
                if (!String.IsNullOrEmpty(searchString))
                {
                    list = list.Where(s => s.Name.Contains(searchString) || s.ID.ToString().Contains(searchString) || s.Level.Contains(searchString)).ToList();
                }
                switch (sortOrder)
                {
                    case "name_desc":
                        list = list.OrderByDescending(s => s.Name).ToList();
                        break;
                    case "level":
                        list = list.OrderBy(s => s.Level).ToList();
                        break;
                    case "level_desc":
                        list = list.OrderByDescending(s => s.Level).ToList();
                        break;
                    default:
                        list = list.OrderBy(s => s.Name).ToList();
                        break;
                }
            }
            ViewBag.people = list;

            cmd = new MySqlCommand("select * from Sidebar", Db.Connection);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    listSidebar.Add(new Sidebar()
                    {
                        Id = reader.GetInt32("Id"),
                        Name = reader.GetString("Name"),
                        Link = !reader.IsDBNull(2) ? reader.GetString("Link") : string.Empty,
                        Parent = !reader.IsDBNull(3) ? reader.GetString("Parent") : string.Empty,
                        Value = reader.GetString("Value")
                    });
                }
            }
            ViewBag.link = listSidebar.OrderBy(s=>s.Value).ToList();
            Db.Connection.Close(); 
            int pageSize = 10;  //一页条数
            return View( PaginatedList<People>.Create(list, pageNumber ?? 1, pageSize));
            
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmpe(People person)  //新建雇员
        {
            await Db.Connection.OpenAsync();
            person.Db = Db;
            var query = new PeopleQuery(Db);
            var result = await query.FindOneAsync(person.Phone);
            if (result is null)
            {
                await person.InsertAsync();
                return new OkObjectResult(person);
            }
            return new NotFoundResult();
        }
        [HttpPost]
        public async Task<IActionResult> SaveData(User user)    //用户注册
        {
            await Db.Connection.OpenAsync();
            user.Db = Db;
            var query = new UserQuery(Db);
            var result = await query.FindOneAsync(user.Email);
            if (result is null)
            {
                await user.InsertAsync();
                return new OkObjectResult(user);
            }
            return new NotFoundResult();
        }
        [HttpPost]
        public async Task<IActionResult> CreateEmployee(People person)  //新建雇员
        {
            await Db.Connection.OpenAsync();
            person.Db = Db;
            var query = new PeopleQuery(Db);
            var result = await query.FindOneAsync(person.Phone);
            if (result is null)
            {
                await person.InsertAsync();
                return new OkObjectResult(person);
            }
            return new NotFoundResult();
        }
        [HttpPost]
        public async Task<IActionResult> EditEmployee(People person)
        {
            await Db.Connection.OpenAsync();
            var query = new PeopleQuery(Db);
            var result = await query.FindOneAsync(person.ID);
            if (result is null)
                return new NotFoundResult();
            result.Name = person.Name;
            result.Level = person.Level;
            result.Phone = person.Phone;
            await result.UpdateAsync();
            return new OkObjectResult(result);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            await Db.Connection.OpenAsync();
            var query = new PeopleQuery(Db);
            var result = await query.FindOneAsync(id);
            if (result is null)
                return new NotFoundResult();
            await result.DeleteAsync();
            return new OkObjectResult(result);
        }
        /*
        public IActionResult SaveData(User user)
        {
            
            var DataItem = _context.User.GetAllUser().Where(x => x.Email == user.Email && x.Password == user.Password).SingleOrDefault();
            //var DataItem2 = user.GetAllUser().Where(x => x.Email == user.Email && x.Password == user.Password).SingleOrDefault();
            if (DataItem != null) 
            {
                return NotFound(user);
            }
            user.IsValid = false;
            user.Insert();
            //BuildEmailTemplate(user.ID);
            return Ok(DataItem);
        }
        */
        public IActionResult Confirm(int regId)
        {
            ViewBag.regID = regId;
            return View();
        }
        /*
        public IActionResult RegisterConfirm(int regId)
        {
            User Data = User.GetAllUser().Where(x => x.ID == regId).FirstOrDefault();
            Data.IsValid = true;
            //_context.SaveChanges();
            var msg = "Your Email Is Verified!";
            return Ok(msg);
        }
        
        public void BuildEmailTemplate(int regID)
        {
            string webRootPath = _webHostEnvironment.WebRootPath;
            string contentRootPath = _webHostEnvironment.ContentRootPath;

            string path = "";
            path = System.IO.Path.Combine(contentRootPath, "EmailTemplate", "Text.cshtml");
            var regInfo = _context.GetAllUser().Where(x => x.ID == regID).FirstOrDefault();
            var url = "http://localhost:54013/" + "Register/Confirm?regId=" + regID;
            path = path.Replace("@ViewBag.ConfirmationLink", url);
            path = path.ToString();
            BuildEmailTemplate("Your Account Is Successfully Created", path, regInfo.Email);
        }
        */
        public static void BuildEmailTemplate(string subjectText, string bodyText, string sendTo)
        {
            string from, to, bcc, cc, subject, body;
            from = "1095412419@qq.com";
            to = sendTo.Trim();
            bcc = "";
            cc = "";
            subject = subjectText;
            StringBuilder sb = new StringBuilder();
            sb.Append(bodyText);
            body = sb.ToString();
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(from);
            mail.To.Add(new MailAddress(to));
            if (!string.IsNullOrEmpty(bcc))
            {
                mail.Bcc.Add(new MailAddress(bcc));
            }
            if(!string.IsNullOrEmpty(cc))
            {
                mail.CC.Add(new MailAddress(cc));
            }
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;
            SendEmail(mail);
        }

        public static void SendEmail(MailMessage mail)
        {
            SmtpClient client = new SmtpClient();
            client.Host = "smtp.qq.com";
            client.Port = 587;
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Credentials = new System.Net.NetworkCredential("1095412419@qq.com", "felpgojkfptajjdf");
            try 
            {
                client.Send(mail);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public async Task<IActionResult> CheckValidUser(User user)
        {
            await Db.Connection.OpenAsync();
            user.Db = Db;
            var query = new UserQuery(Db);
            var result = await query.FindOneAsync(user.Email, user.Password);
            if (result is  null) 
                return new NotFoundResult();
            TempData["UserID"] = result.ID.ToString();
            TempData["UserName"] = result.Username.ToString();
            return new OkObjectResult(user);

        }
        /*
        public IActionResult CheckValidUser(User user)
        {
            string result = "Fail";
            var DataItem = _context.GetAllUser().Where(x => x.Email == user.Email && x.Password == user.Password).SingleOrDefault();
            if(DataItem != null)
            {
                TempData["UserID"] = DataItem.ID.ToString();
                TempData["UserName"] = DataItem.Username.ToString();
                result = "Success";
            }
            return Ok(result);
        }
        */

        public IActionResult AfterLogin()   //加载登陆
        {
            
            if (TempData["UserID"] != null)
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
                            Link = !reader.IsDBNull(2) ? reader.GetString("Link"): string.Empty,
                            Parent = !reader.IsDBNull(3) ? reader.GetString("Parent") : string.Empty,
                            Value = reader.GetString("Value")
                        });
                    }
                }
                ViewBag.link = listSidebar;
                Db.Connection.Close();
                return View();
            }
            else 
            {
                return RedirectToAction("Index");
            }
        }

        public IActionResult Logout()
        {
            TempData.Clear();
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        public IActionResult Index()
        {
            AppDb context = HttpContext.RequestServices.GetService(typeof(MvcUser.Data.AppDb)) as AppDb;
            return View(); //_context.GetAllUser()
        }
        /*
        // GET: Users
        public async Task<IActionResult> Index()
        {
            return View(await _context.User.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.ID == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Username,Email,Password,IsValid")] User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Username,Email,Password,IsValid")] User user)
        {
            if (id != user.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.ID == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.User.FindAsync(id);
            _context.User.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.ID == id);
        }
    */
    }
        
}
