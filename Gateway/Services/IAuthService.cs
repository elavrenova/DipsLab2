using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Gateway.Models;
using Microsoft.AspNetCore.Http;

namespace Gateway.Services
{
    public interface IAuthService
    {
        Task<HttpResponseMessage> Login(UserModel item);
        Task<HttpResponseMessage> LogOut(string token);
        Task<string> VerifyToken(string token);
        Task<string> GetRole(string token);
    }
}
