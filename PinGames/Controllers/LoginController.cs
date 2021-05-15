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
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Login(UserLoginModel model)
        {
            var existingUser = await _db.Users.FirstOrDefaultAsync(user => user.UserName == model.Login || user.Email == model.Login);
            if (existingUser != null && Decode(existingUser.Password) == model.Password)
            {
                var userClaim = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, existingUser.UserName),
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
    }
}
