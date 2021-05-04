using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PinGames.Models
{
    public class LibraryModel
    {  
        [Key]
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        [ForeignKey("UserId")]

        public UserAccountModel User { get; set; }
        [Required]
        public int GameId { get; set; }
        [ForeignKey("GameId")]
        public GameModel Game { get; set; }
        public string Review { get; set; }
    }
}
