using System;
using System.Collections.Generic;

namespace PinGames.Models
{
    public struct gamesProfile
    {
        public int gameId;
        public int libId;
        public string gameName;
        public string gameReview;
        public string gameAbout;
        private string _gameImg;

        public string GameImg
        {
            get => _gameImg;
            set => _gameImg = value ?? "default.jpg";
        }
    }
    public class ProfileModel
    {
        private string _userImg;
        

/*        public int Id { get; set; }*/
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserAbout { get; set; }
        public string UserImg { 
            get => _userImg; 
            set => _userImg = value ?? "default.png"; 
        }
        public List<gamesProfile> gameProfiles;
    }
}