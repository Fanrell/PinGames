using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PinGames.Models
{
    public class UserAccountModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(20)]
        [Display(Name = "Login")]
        public string UserName { get; set; }
        [Required]
        [MaxLength(60)]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [MaxLength(32)]
        public string Password { get; set; }
        public bool AdminPrivilage { get; set; }
        [DisplayFormat(NullDisplayText = "default")]
        [Display(Name = "Profile Picture")]
#nullable enable
        public string? ImageName { get; set; }
    }
}
