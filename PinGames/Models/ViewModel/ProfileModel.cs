using System;
using System.Collections.Generic;

namespace PinGames.Models
{
public class ProfileModel
    {
        private string _userImg;
        private string _gameImg;

        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserAbout { get; set; }
        public string UserImg { 
            get => _userImg; 
            set => _userImg = value ?? "default.jpg"; 
        }
        public int GameId { get; set; }
        public string GameName { get; set; }
        public string GameImg { 
            get => _gameImg;
            set => _gameImg = value ?? "default.jpg"; 
        }
        public string GameAbout { get; set; }
    }
}