using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace PinGames.Static
{
    public static class SessionController
    {
        internal static void AddToSession(HttpContext httpContext, string sessionName, string serializeDataToAdd)
        {
            httpContext.Session.SetString(sessionName, JsonSerializer.Serialize(serializeDataToAdd));
        }

        internal static bool SessionExists(HttpContext httpContext, string sessionName)
        {
            return !string.IsNullOrEmpty(
                httpContext.Session.GetString(sessionName)
                );
        }

        internal static string ReadUserNameFromSession(HttpContext httpContext, string sessionName)
        {
            var user = httpContext.Session.GetString(sessionName);
            return JsonSerializer.Deserialize<string>(user);
        }

        internal static int ReadUserIdFromSession(HttpContext httpContext, string sessionName)
        {
            var user = httpContext.Session.GetString(sessionName);
            return JsonSerializer.Deserialize<int>(user);
        }
    }
}
