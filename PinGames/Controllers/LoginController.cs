using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PinGames.Data;
using PinGames.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static PinGames.Static.Base64;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using System.Net;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace PinGames.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly EmailConfiguration _email;
        public LoginController(ILogger<LoginController> logger, ApplicationDbContext db, EmailConfiguration email)
        {
            _logger = logger;
            _db = db;
            _email = email;
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
                    new Claim("FullName", existingUser.UserName),
                    new Claim(ClaimTypes.Sid, existingUser.Id.ToString())
                };
                if (existingUser.AdminPrivilage)
                    userClaim.Add(new Claim(ClaimTypes.Role, "Admin"));
                var userIdentity = new ClaimsIdentity(userClaim, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = model.RememberLogin
                };
                var userPrincipal = new ClaimsPrincipal(userIdentity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal, authProperties);
                HttpContext.User = userPrincipal;
                _logger.LogInformation("Succseed login");
                return RedirectToAction("index", "profile", new { login = existingUser.UserName });

            }
            else
                return RedirectToAction(actionName: "Index", controllerName: "Login");
        }

        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme
                );
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

        public IActionResult LoginProblem()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> LoginProblem(UserAccountModel model)
        {
            var user = await _db.Users.Where(u => u.Email == model.Email).AsNoTracking().FirstOrDefaultAsync();

            if (user != null)
            {
                if (!SendMail(model.Email, user.Id))
                    return RedirectToAction(controllerName: "home", actionName: "index");
            }

            return RedirectToAction(controllerName: "home", actionName: "index");
        }

        private bool SendMail(string userEmail, int userId)
        {
            bool succseed = false;
            using(var mail = new System.Net.Mail.MailMessage())
            {
                mail.From = new System.Net.Mail.MailAddress(_email.Email, "PinGames");
                mail.Subject = "Forgotten Password";
                mail.Body =
                    (
                        "This is messege from PinGames service. You Probably forgot your password.\n"
                        +$@"Please go to this page: {_email.PageForRestorePassword}?restorePassword={Encode(userId.ToString())}"
                    );
                mail.To.Add(userEmail);

                using (var smtp = new System.Net.Mail.SmtpClient(_email.SmtpServer, _email.Port))
                {
                    smtp.EnableSsl = true;
                    smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(_email.Email, _email.Password);
                    ServicePointManager.ServerCertificateValidationCallback =
                        (sender, certificate, chain, sslPolicyError) => true;
                    smtp.Send(mail);
                    succseed = true;
                }
            }
            return succseed;
        }
    }
}
