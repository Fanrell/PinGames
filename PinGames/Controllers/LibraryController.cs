using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PinGames.Data;
using PinGames.Models;

namespace PinGames.Controllers
{
    public class LibraryController : Controller
    {
        private readonly ILogger<LibraryController> _logger;
        private readonly ApplicationDbContext _db;
        public IEnumerable<GameLibraryModel> gameLibraries;

        public LibraryController(ILogger<LibraryController> logger, ApplicationDbContext db)
        {
            _db = db;
            _logger = logger;
        }
        public IActionResult Index()
        {
            gameLibraries = (
                from game in _db.Games
                join genre in _db.Genres on game.GenreId equals genre.Id
                select new GameLibraryModel
                {
                    GameId = game.Id,
                    GameName = game.Name,
                    GameAbout = game.About,
                    GenreName = genre.GenreName
                }
                ).ToList();

            return View(gameLibraries);
        }
    }
}
