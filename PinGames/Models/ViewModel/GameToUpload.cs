using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PinGames.Models
{
    public class GameToUpload
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        [MaxLength(60)]
        public string Name { get; set; }
        [Required]
        public int GenreId { get; set; }
        [Required]
        public string About { get; set; }
        public IFormFile GameImg { get; set; } 
    }
}
