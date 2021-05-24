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

namespace PinGames.Controllers
{
    public class RestoreController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<RestoreController> _logger;
        public RestoreController(ApplicationDbContext db, ILogger<RestoreController> logger)
        {
            _db = db;
            _logger = logger;
        }
        public IActionResult Index(string restorePassword)
        {
            ViewData["rest"] = restorePassword;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(string restorePassword, PasswordChangeModel model)
        {
            var user = await _db.Users.Where(u => u.Id == Convert.ToInt32(Decode(restorePassword))).AsNoTracking().FirstOrDefaultAsync();
            if(user != null)
            {
                user.Password = Encode(model.NewPassword);
                _db.Update(user);
                await _db.SaveChangesAsync();
                return RedirectToAction(controllerName: "login", actionName: "index");
            }
            return RedirectToAction(actionName: "index", new { restorePassword = restorePassword });
            
        }
    }
}
