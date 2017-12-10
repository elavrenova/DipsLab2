using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Models
{
    public class UserLogin
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string ReturnUrl { get; set; }
    }
}
