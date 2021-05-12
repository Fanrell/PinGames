using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Text.Json;
using PinGames.Models;
using PinGames.Data;
using static PinGames.Static.PasswordValid;
using static PinGames.Static.SessionController;
using static PinGames.Static.Base64;



namespace PinGames.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;


        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }
        public IActionResult Login()
        {
            if (SessionExists(HttpContext, "LoginSession"))
            {
                return RedirectToRoute("Profile", new { userName = ReadUserNameFromSession(HttpContext, "LoginSession") });
            }
            return View();
        }

        //[HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult LogoutAction()
        {
            if(SessionExists(HttpContext, "LoginSession"))
            {
                HttpContext.Session.Remove("LoginSession");
            }
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Login(UserLoginModel model)
        {
            var existingUser = await _db.Users.FirstOrDefaultAsync(user => user.UserName == model.Login || user.Email == model.Login);
            if (existingUser != null && Decode(existingUser.Password) == model.Password)
            {
                HttpContext.Session.SetString("LoginSession", JsonSerializer.Serialize(existingUser.UserName));
                HttpContext.Session.SetString("LoginSessionId", JsonSerializer.Serialize(existingUser.Id));

                return RedirectToAction("index", "profile", new { login = ReadUserNameFromSession(HttpContext, "LoginSession") });
;
            }
            else
                return View();
    }
        public IActionResult RegisterAction()
        {
            return View("Register");
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
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
                return View("Login");
            }
            return View("Privacy");
            
        }

    }
}
