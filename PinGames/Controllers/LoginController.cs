using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PinGames.Data;
using PinGames.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using static PinGames.Static.SessionController;
using static PinGames.Static.Base64;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace PinGames.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;
        private readonly ApplicationDbContext _db;
        public LoginController(ILogger<LoginController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserLoginModel model)
        {
            var existingUser = await _db.Users.FirstOrDefaultAsync(user => user.UserName == model.Login || user.Email == model.Login);
            if (existingUser != null && Decode(existingUser.Password) == model.Password)
            {
                var userClaim = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, existingUser.UserName),
                    new Claim(ClaimTypes.Sid, existingUser.Id.ToString())
                };
                if (existingUser.AdminPrivilage)
                    userClaim.Add(new Claim(ClaimTypes.Role, "Admin"));
                var userIdentity = new ClaimsIdentity(userClaim, "UserIdentity");
                var userPrincipal = new ClaimsPrincipal(userIdentity);
                await HttpContext.SignInAsync(userPrincipal);
                HttpContext.User = userPrincipal;
                return RedirectToAction("index", "profile", new { login = existingUser.UserName });
                
            }
            else
                return RedirectToAction(actionName: "Index", controllerName:"Login");
        }

        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction(controllerName: "Home", actionName: "Index");
        }

        public IActionResult Register()
        {
            return View("Register");
        }
        [HttpPost]
        /*
         * To Do:
         *  -Password without banned charactes
         */
        public async Task<IActionResult> Register(RegisterUserModel model)
        {
            var anotherUser = await _db.Users.FirstOrDefaultAsync(x => x.UserName == model.Login || x.Email == model.Email);
            if (anotherUser == null)
            {
                UserAccountModel newUser = new UserAccountModel
                {
                    Id = Math.Abs(Guid.NewGuid().GetHashCode()),
                    UserName = model.Login,
                    Password = Convert.ToBase64String(
                        System.Text.Encoding.UTF8.GetBytes(model.Password)
                        ),
                    Email = model.Email,
                    AdminPrivilage = model.AdminPrivilage ?? false
                };

                await _db.AddAsync(newUser);
                await _db.SaveChangesAsync();
                return View("index");
            }
            return View("Register");

        }
    }
}
