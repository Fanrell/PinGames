using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        public string About { get; set; }
        public bool AdminPrivilage { get; set; }
        public List<LibraryModel> Libraries { get; set; }


#nullable enable
        [DisplayFormat(NullDisplayText = "default")]
        [Display(Name = "Profile Picture")]
        [MaxLength(100)]
        public string? ImageName { get; set; }
    }
}
