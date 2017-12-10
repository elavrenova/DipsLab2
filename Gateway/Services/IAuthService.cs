using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Gateway.Models;

namespace Gateway.Services
{
    public interface IAuthService
    {
        Task<string> Login(UserModel item);
        Task<HttpResponseMessage> LogOut(string token);
        Task<string> VerifyToken(string token);
    }
}
