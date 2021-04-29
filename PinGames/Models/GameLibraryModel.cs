using System.Collections.Generic;

namespace PinGames.Models
{
    public class GameLibraryModel
        {  
        public List<string> Genres { get; set; }
        public List<GameModel> Games { get; set; }
        public List<GenreModel> Game_Genre { get; set; }
        }
}
