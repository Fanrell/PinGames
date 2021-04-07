﻿using System;
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
            throw new NotImplementedException();
        }

        internal static bool SessionExists(HttpContext httpContext, string sessionName)
        {
            return !string.IsNullOrEmpty(
                httpContext.Session.GetString(sessionName)
                );
        }
    }
}