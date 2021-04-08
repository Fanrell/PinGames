using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PinGames.Models;
using PinGames.Data;


namespace PinGames.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ILogger<ProfileController> _logger;
        private readonly ApplicationDbContext _db;
        public ProfileController(ILogger<ProfileController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }
        public async Task<IActionResult> Index(string userName)
        {
            var profile = await _db.Users.FirstOrDefaultAsync(user => user.UserName == userName);
            return View(profile);
        }
    }
}
