using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PinGames.Static
{
    public static class Base64
    {
        internal static string Encode(string toEncode)
        {
            var data = Convert.ToBase64String(
                System.Text.Encoding.UTF8.GetBytes(toEncode)
                );
            return data;
        }
        internal static string Decode(string toDecode)
        {
            var data = Convert.FromBase64String(toDecode);
            return System.Text.Encoding.UTF8.GetString(data);
        }
    }
}
