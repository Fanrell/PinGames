﻿using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> Index(string userName)
        {
            Console.WriteLine(userName);
            var user = await _db.Users.FirstOrDefaultAsync(user => user.UserName == userName);
            return View(user);
        }

        public async Task<IActionResult> AddGamesToDatabase()
        {

            return View();
        }
    }
}
