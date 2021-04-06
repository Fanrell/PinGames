using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using PinGames.Models;
using PinGames.Data;
using static PinGames.Static.PasswordValid;
using Microsoft.EntityFrameworkCore;

namespace PinGames.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        public IEnumerable<UserAccountModel> Users { get; set; }


        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpGet]
        public IActionResult LoginAction()
        {
            return View("Privacy");
        }
        public IActionResult RegisterAction()
        {
            return View("Register");
        }
        [HttpPost]
        /*
         * To Do:
         *  -Password without banned charactes
         */
        public async Task<IActionResult> RegisterAction(RegisterUserModel model)
        {
            var anotherUser = await _db.Users.FirstOrDefaultAsync(x => x.UserName == model.Login || x.Email == model.Email);
            if (anotherUser == null)
            {
                UserAccountModel newUser = new UserAccountModel {
                    Id = Math.Abs(Guid.NewGuid().GetHashCode()),
                    UserName = model.Login,
                    Password = Convert.ToBase64String(
                        System.Text.Encoding.UTF8.GetBytes(model.Password)
                        ),
                    Email = model.Email,
                    AdminPrivilage = model.AdminPrivilage ?? false
                };

                await _db.AddAsync<UserAccountModel>(newUser);
                await _db.SaveChangesAsync();
                return View("Index");
            }
            return View("Privacy");
            
        }

    }
}
