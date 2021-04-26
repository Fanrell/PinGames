using System;
using System.Collections.Generic;

namespace PinGames.Models
{
public class ProfileModel
    {
        public UserAccountModel User { get; set; }
        public List<LibraryModel> Games {get; set;}
    }
}