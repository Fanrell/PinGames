using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PinGames.Models
{
    public class GameModel
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        [MaxLength(60)]
        public string Name { get; set; }
        [Required]
        [MaxLength(20)]
        public string Genre { get; set; }
        public List<ReviewModel> Reviews { get; set; }

    }
}
