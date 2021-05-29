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
using static PinGames.Static.Base64;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using X.PagedList;

namespace PinGames.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ILogger<ProfileController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHost;
        public LibraryModel library = new LibraryModel();
        public ProfileController(ILoggerFactory logger, ApplicationDbContext db, IWebHostEnvironment webHost)
        {
            _logger = logger.CreateLogger<ProfileController>();
            _db = db;
            _webHost = webHost;
        }
        [HttpGet] // zabezpieczuć bo error bez id
        public async Task<IActionResult> Index(string login, int? page)
        {
            int pageNumber = page ?? 1;
            int pageSize = 3;
            ProfileModel userDb = new ProfileModel();
            List<gamesProfile> games = new List<gamesProfile>(); 

            try
            {
                userDb = await
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

                games = await
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
            }
            catch(Exception e)
            {
                _logger.LogCritical(e, $"Someone use {login} as login to show profile");
                return NotFound();
            }

            ViewData["userProfile"] = userDb;


            return View(games.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        public async Task<IActionResult> Game(int? GameId)
        {
            GameViewModel gameView = new GameViewModel();
            try
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
                gameView = await (
                    from game in _db.Games
                    where game.Id == GameId
                    join genre in _db.Genres on game.GenreId equals genre.Id
                    select new GameViewModel
                    {
                        GameId = game.Id,
                        GameName = game.Name,
                        GameAbout = game.About,
                        GameImg = game.GameImg ?? "default.jpg", 
                        GenreName = genre.GenreName,
                        Review = rev
                    }
                    ).AsNoTracking().FirstOrDefaultAsync();

                    if(GameId == null || gameView == null)
                        throw new NullReferenceException();
                }
            catch(Exception e)
            {
                _logger.LogCritical(e, $"Someone use {GameId} as gameId to show Profile of game");
                return NotFound();
            }
            ViewData["game"] = gameView;
            return View();
        }
        [HttpGet]
        [Authorize]
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
        [Authorize]
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
        [HttpGet]
        public async Task<IActionResult> Review(int gameId)
        {
            var game = await (
                from lib in _db.Libraries
                where lib.GameId == gameId
                join games in _db.Games on lib.GameId equals games.Id
                select new LibraryModel
                {
                    Id = lib.Id,
                    UserId = lib.UserId,
                    GameId = lib.GameId,
                    Game = games,
                    Review = lib.Review
                }
                ).AsNoTracking().FirstOrDefaultAsync();

            ViewData["game"] = game;

            return View();
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Review(int userId, int gameId, LibraryModel model)
        {
            var user = await _db.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();
            var rev = await (
                from lib in _db.Libraries
                where lib.UserId == user.Id && lib.GameId == gameId
                select new LibraryModel
                {
                    Id = lib.Id,
                    UserId = lib.UserId,
                    GameId = lib.GameId,
                    Review = model.Review
                }
                ).AsNoTracking().FirstOrDefaultAsync();
            _db.Update(rev);
            await _db.SaveChangesAsync();
            return RedirectToAction("index", new { login = user.UserName});
        }
        
        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(PasswordChangeModel model)
        {
            var user = await _db.Users.Where(u => u.UserName == HttpContext.User.Identity.Name).AsNoTracking().FirstOrDefaultAsync();
            if (user != null)
            {
                if (model.NewPassword == model.RePassword)
                {
                    user.Password = Encode(model.NewPassword);
                    _db.Update(user);
                    await _db.SaveChangesAsync();
                    return RedirectToAction(actionName: "index", new { login = user.UserName });
                }
                return View();
            }
            return RedirectToAction(controllerName: "home", actionName: "index");
        }

            [Authorize]
        public async Task<IActionResult> DeleteFromLibrary(int gameId)
        {
            try
            {
                var game = await _db.Libraries.Where(game => game.GameId == gameId).AsNoTracking().FirstOrDefaultAsync();
                _db.Libraries.Remove(game);
                await _db.SaveChangesAsync();
            }
            catch(Exception e)
            {
                _logger.LogCritical(e, "");
            }

            return RedirectToAction("Index", new{login = HttpContext.User.Identity.Name});
        }
    }


}
