using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PinGames.Models
{
    public class ProfileInfoModel
    {
        public string userName { get; set; }
        public string email { get; set; }
        public string about { get; set; }
        public IFormFile profilePicture { get; set; }
    }
}
