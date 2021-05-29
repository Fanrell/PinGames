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
using X.PagedList;

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
        [HttpGet]
        public async Task<IActionResult>Index(string name, int? genre, int? page)
        {
            int pageNumber = page ?? 1;
            int pageSize = 5;
            List<GameModel> games = new List<GameModel>();
            var genres = await _db.Genres.ToListAsync();
            ViewData["Genres"] = genres;

            if (name == null && genre == null)
                games = await _IndexNoFilter();

            else if (name != null && genre == null)
                games = await _IndexNameFilter(name);
            else if (name == null && genre != null)
                games = await _IndexGenreFilter(genre);
            else
                games = await _IndexFullFilter(name, genre);

            return View(games.ToPagedList(pageNumber, pageSize));
        }
        [Authorize]
        public async Task<IActionResult> AddGame(int? gameId)
        {
            GameToUpload game = new GameToUpload();

            if(gameId != null)
            {
                game = await
                (
                    from games in _db.Games
                    where games.Id == gameId
                    select new GameToUpload
                    {
                        Id = games.Id,
                        Name = games.Name,
                        About = games.About,
                        GenreId = games.GenreId
                    }
                ).AsNoTracking().FirstOrDefaultAsync();
            }

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
            return View(game);
        }
        [HttpPost]
        public async Task<IActionResult> AddGame(GameToUpload model)
        {
            string imgName = await UploadGameCover(_webHost, model);
            var gameInDb = await _db.Games.Where(g => g.Id == model.Id).AsNoTracking().FirstOrDefaultAsync();
            var gameToDb = new GameModel
            {
                
                Name = model.Name,
                GameImg = imgName,
                GenreId = model.GenreId,
                About = model.About

            };
            
            if(gameInDb == null)
                await _db.Games.AddAsync(gameToDb);
            else
            {
                gameToDb.Id = gameInDb.Id;
                _db.Update(gameToDb);
            }
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> DeleteGame(int? gameId)
        {
            var gameInDb = await _db.Games.Where(g => g.Id == gameId).AsNoTracking().FirstOrDefaultAsync();
            _db.Remove(gameInDb);
            await _db.SaveChangesAsync();
            return RedirectToAction("index");
        }

        [HttpGet]
        public async Task<IActionResult> GameToLibrary(int gameId)
        {
            var userName = HttpContext.User.Identity.Name;
            var userId = await _db.Users.Where(u => u.UserName == userName).Select(u => u.Id).FirstOrDefaultAsync();

            if (userName != null)
            {
                var library = await (
                    from lib in _db.Libraries
                    where lib.GameId == gameId
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

        private async Task<List<GameModel>> _IndexNoFilter()
        {
            var gameLibraries = await (
                from game in _db.Games
                join genre in _db.Genres on game.GenreId equals genre.Id
                select new GameModel
                {
                    Id = game.Id,
                    Name = game.Name,
                    GameImg = game.GameImg ?? "default.jpg",
                    About = game.About,
                    Genre = genre
                }
                ).AsNoTracking().ToListAsync();

            return gameLibraries;
        }

        private async Task<List<GameModel>> _IndexNameFilter(string name)
        {
            var gameLibraries = await
            (
                from game in _db.Games
                where game.Name.ToLower().Contains(name.ToLower())
                join genr in _db.Genres on game.GenreId equals genr.Id
                select new GameModel
                {
                    Id = game.Id,
                    Name = game.Name,
                    GameImg = game.GameImg ?? "default.jpg",
                    About = game.About,
                    Genre = genr
                }
            ).AsNoTracking().ToListAsync();

            return gameLibraries;
        }

        private async Task<List<GameModel>> _IndexGenreFilter(int? genre)
        {
            var gameLibraries = await
            (
                from game in _db.Games
                where game.GenreId == genre
                join genr in _db.Genres on game.GenreId equals genr.Id
                select new GameModel
                {
                    Id = game.Id,
                    Name = game.Name,
                    GameImg = game.GameImg ?? "default.jpg",
                    About = game.About,
                    Genre = genr
                }
            ).AsNoTracking().ToListAsync();

            return gameLibraries;
        }

        private async Task<List<GameModel>> _IndexFullFilter(string name, int? genre)
        {
            var gameLibraries = await
            (
                from game in _db.Games
                where game.GenreId == genre
                where game.Name.ToLower().Contains(name.ToLower())
                join genr in _db.Genres on game.GenreId equals genr.Id
                select new GameModel
                {
                    Id = game.Id,
                    Name = game.Name,
                    GameImg = game.GameImg ?? "default.jpg",
                    About = game.About,
                    Genre = genr
                }
            ).AsNoTracking().ToListAsync();

            return gameLibraries;
        }
    }
}
