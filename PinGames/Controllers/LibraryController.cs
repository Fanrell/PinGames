using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PinGames.Data;
using PinGames.Models;
using Microsoft.EntityFrameworkCore;
using static PinGames.Static.UploadFile;
using Microsoft.AspNetCore.Hosting;
using static PinGames.Static.SessionController;
using Microsoft.AspNetCore.Authorization;

namespace PinGames.Controllers
{
    public class LibraryController : Controller
    {
        private readonly ILogger<LibraryController> _logger;
        private readonly IWebHostEnvironment _webHost;
        private readonly ApplicationDbContext _db;

        public LibraryController(IWebHostEnvironment webHost,ILogger<LibraryController> logger, ApplicationDbContext db)
        {
            _db = db;
            _logger = logger;
            _webHost = webHost;
        }
        public async Task<IActionResult> Index()
        {
            var gameLibraries = await (
                from game in _db.Games
                join genre in _db.Genres on game.GenreId equals genre.Id
                select new GameLibraryModel
                {
                    GameId = game.Id,
                    GameName = game.Name,
                    GameImg = game.GameImg,
                    GameAbout = game.About,
                    GenreName = genre.GenreName
                }
                ).AsNoTracking().ToListAsync();

            ViewData["gameLibrary"] = gameLibraries;

            return View();
        }
        [Authorize]
        public async Task<IActionResult> AddGame()
        {
            var genres = await
                (
                from genre in _db.Genres
                select new GenreModel
                {
                    Id = genre.Id,
                    GenreName = genre.GenreName
                }
                ).AsNoTracking().ToListAsync();
            ViewData["genres"] = genres;
            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> AddGame(GameToUpload model)
        {
            string imgName = await UploadGameCover(_webHost, model);
            var gameToDb = new GameModel
            {
                Name = model.Name,
                GameImg = imgName,
                GenreId = model.GenreId,
                About = model.About

            };
            await _db.Games.AddAsync(gameToDb);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> GameToLibrary(int gameId)
        {
            int userId = ReadUserIdFromSession(HttpContext, "LoginSessionId");

            if(userId >= 0)
            {
                var library = await (
                    from lib in _db.Libraries
                    where lib.UserId == userId
                    join usr in _db.Users on lib.UserId equals usr.Id
                    select new LibraryModel
                    {
                        Id = lib.Id,
                        UserId = lib.UserId,
                        GameId = lib.GameId
                    }
                    ).AsNoTracking().FirstOrDefaultAsync();

                if (library == null)
                {
                    var gameToAdd = new LibraryModel
                    {
                        GameId = gameId,
                        UserId = userId
                    };
                    await _db.Libraries.AddAsync(gameToAdd);
                    await _db.SaveChangesAsync();
                }

            }
            return RedirectToAction("Index");
        }
    }
}
