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
        public LibraryModel library = new LibraryModel();
        public ProfileController(ILogger<ProfileController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }
        [HttpGet]
        public async Task<IActionResult> Index(string login)
        {
            var userDB = await _db.Users.FirstOrDefaultAsync(user => user.UserName == login);

            var lib = (
                from library in _db.Libraries
                where library.UserId == userDB.Id
                join game in _db.Games on library.GameId equals game.Id
                join user in _db.Users on library.UserId equals user.Id
                select new ProfileModel
                {
                    Id = library.Id,
                    UserId = user.Id,
                    UserName = user.UserName,
                    UserAbout = user.About,
                    UserImg = user.ImageName,
                    GameId = game.Id,
                    GameName = game.Name,
                    GameAbout = game.About
                }
                ).FirstOrDefault();

            ViewData["userProfile"] = lib;


            return View();
        }

    }
}
