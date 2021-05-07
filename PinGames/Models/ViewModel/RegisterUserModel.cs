using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PinGames.Models
{
    public class RegisterUserModel
    {
        public string Login { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string RepeatedPassword { get; set; }
        public bool? AdminPrivilage { get; set; }

    }
}
