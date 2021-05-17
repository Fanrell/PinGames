using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PinGames.Models;
using PinGames.Data;
using static PinGames.Static.SessionController;
using static PinGames.Static.UploadFile;
using Microsoft.AspNetCore.Hosting;

namespace PinGames.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ILogger<ProfileController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHost;
        public LibraryModel library = new LibraryModel();
        public ProfileController(ILogger<ProfileController> logger, ApplicationDbContext db, IWebHostEnvironment webHost)
        {
            _logger = logger;
            _db = db;
            _webHost = webHost;
        }
        [HttpGet] // zabezpieczuć bo error bez id
        public async Task<IActionResult> Index(string login)
        {

            var userDb = await
                (
                from user in _db.Users
                where user.UserName == login
                select new ProfileModel
                {
                    UserId = user.Id,
                    UserAbout = user.About,
                    UserName = user.UserName,
                    UserImg = user.ImageName ?? "default.png"
                }
                ).AsNoTracking().FirstOrDefaultAsync();

            var games = await
            (
            from library in _db.Libraries
            where library.UserId == userDb.UserId
            join game in _db.Games on library.GameId equals game.Id
            select new gamesProfile
            {
                gameId = game.Id,
                libId = library.Id,
                gameName = game.Name,
                gameAbout = game.About,
                gameReview = library.Review ?? "",
                GameImg = game.GameImg
            }
            ).ToListAsync();
            userDb.gameProfiles = games;


            ViewData["userProfile"] = userDb;


            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Game(int GameId)
        {
            var rev = await (
                from library in _db.Libraries
                where library.GameId == GameId
                join user in _db.Users on library.UserId equals user.Id
                select new Reviews
                {
                    userName = user.UserName,
                    review = library.Review

                }
                ).ToListAsync();

            Console.WriteLine(GameId);
            var gameView = await (
                from game in _db.Games
                where game.Id == GameId
                join genre in _db.Genres on game.GenreId equals genre.Id
                select new GameViewModel
                {
                    GameId = game.Id,
                    GameName = game.Name,
                    GameAbout = game.About,
                    GameImg = game.GameImg, // po zmianie modelu "GameModel" zmienić na pobranie wartości
                    GenreName = genre.GenreName,
                    Review = rev
                }
                ).AsNoTracking().FirstOrDefaultAsync();

            ViewData["game"] = gameView;
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> ProfileInfo(string login)
        {
            var user = await
                (
                    from usr in _db.Users
                    where usr.UserName == login
                    select new UserAccountModel
                    {
                        UserName = usr.UserName,
                        About = usr.About,
                        Email = usr.Email,
                        ImageName = usr.ImageName ?? "default.png"
                    }
                ).AsNoTracking().FirstOrDefaultAsync();
            ViewData["userInfo"] = user;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ProfileInfo(ProfileInfoModel model)
        {
            var login = HttpContext.User.Identity.Name;
            var dataFromDb = await _db.Users.FirstOrDefaultAsync(user => user.UserName == login);
            if (dataFromDb != null)
            {
                string imgName = await UploadProfileImg(_webHost, model);
                dataFromDb.UserName = model.userName;
                dataFromDb.Email = model.email;
                dataFromDb.About = model.about;
                dataFromDb.ImageName = imgName;
                _db.Users.Update(dataFromDb);
                await _db.SaveChangesAsync();
                AddToSession(HttpContext, "LoginSession", model.userName);
            }
            return RedirectToActionPermanent("index", new { login = dataFromDb.UserName });
        }
    }
}
