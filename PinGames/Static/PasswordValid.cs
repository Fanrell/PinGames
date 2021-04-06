using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PinGames.Static
{
    public static class PasswordValid
    {
        public static bool IsValidPassword(string password)
        {
            return
                password.Any(c => char.IsLetter(c)) &&
                password.Any(c => char.IsDigit(c)) &&
                password.Any(c => char.IsSymbol(c));
        }
    }
}
