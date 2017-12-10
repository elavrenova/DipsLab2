using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gateway.Models;

namespace AuthServer.Models
{
    public class User
    {
        public User(UserModel userModel)
        {
            this.Username = userModel.Username;
            this.Password = userModel.Password;
        }
        public User()
        {

        }

        public User(string userName, string password)
        {
            this.Username = userName;
            this.Password = password;
        }
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
