using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PinGames.Models
{
    public class EmailConfiguration
    {
        public string Email{ get; set; }
        public string Password { get; set; }
        public string SmtpServer { get; set; }
        public string PageForRestorePassword { get; set; }
        public int Port { get; set; }
    }
}
