using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PinGames.Models
{
    public class ReviewModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public UserAccountModel User { get; set; }
        [Required]
        public GameModel Game { get; set; }
        [Required]
        [MaxLength(400)]
        public string Review { get; set; }
    }
}
