using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PinGames.Models
{
    public struct Reviews
    {
        public string userName;
        public string review;
    }

    public class GameViewModel : Controller
    {
        private string _gameImg;

        public int GameId { get; set; }
        public string GameName { get; set; }
        public string GameAbout { get; set; }
        public string GameImg
        {
            get => _gameImg;
            set => _gameImg = value ?? "default.jpg";
        }
        public string GenreName { get; set; }
        public List<Reviews> Review { get; set; }

    }
}
