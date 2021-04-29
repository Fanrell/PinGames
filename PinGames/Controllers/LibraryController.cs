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

        public LibraryController(ILogger<LibraryController> logger, ApplicationDbContext db)
        {
            _db = db;
            _logger = logger;
        }
        public IActionResult Index()
        {
          GameLibraryModel gameLibrary = new GameLibraryModel();
            gameLibrary.Genres = _db.Genres.Select(x => x.GenreName).ToList();
            gameLibrary.Games = _db.Games.Select(x => x).ToList();
            foreach(var game in gameLibrary.Games)
            {
                game.Genre = _db.Genres.FirstOrDefault(x => x.Id == game.GenreId);
            }
            return View(gameLibrary);
        }
    }
}
