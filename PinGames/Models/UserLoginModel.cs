using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PinGames.Models
{
    public class UserLoginModel
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public bool RememberLogin { get; set; }
    }
}
