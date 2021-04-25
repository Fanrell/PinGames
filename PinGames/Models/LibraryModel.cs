using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace PinGames.Models
{
    public class LibraryModel
    {  
        [Key]
        public int Id { get; set; }
        [Required]
        public UserAccountModel User { get; set; }
        [Required]
        public GameModel Game { get; set; }
    }
}
