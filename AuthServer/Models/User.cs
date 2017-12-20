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
            this.Role = userModel.Role;
        }
        public User()
        {

        }

        public User(string userName, string password, string role)
        {
            this.Username = userName;
            this.Password = password;
            this.Role = role;
        }
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}
