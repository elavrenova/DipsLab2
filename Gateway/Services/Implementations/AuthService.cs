using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Gateway.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Gateway.Services.Implementations
{
    public class AuthService: Service, IAuthService
    {
        public AuthService(IConfiguration configuration) : 
            base("http://localhost:50539") { }

        public async Task<HttpResponseMessage> Login(UserModel item)
        {
            try
            {
                return await PostJson("account/customlogin", item);
            }
            catch
            {
                return null;
            }
        }
        public async Task<HttpResponseMessage> LogOut(string token)
        {
            return await Get($"account/customlogout?token={token}");
        }

        public async Task<string> VerifyToken(string token)
        {
            var resp = await Get($"account/verifytoken?token={token}");
            try
            {
                return await resp.Content.ReadAsStringAsync();
            }
            catch
            {
                return null;
            }
             
        }


    }
}
