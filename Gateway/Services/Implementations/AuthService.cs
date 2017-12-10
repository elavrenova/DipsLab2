using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Gateway.Models;
using Microsoft.Extensions.Configuration;

namespace Gateway.Services.Implementations
{
    public class AuthService: Service, IAuthService
    {
        public AuthService(IConfiguration configuration) : 
            base("http://localhost:50539/") { }

        public async Task<string> Login(UserModel item)
        {
            var res = await PostJson("auth/customlogin", item);
            try
            {
                return await res.Content.ReadAsStringAsync();
            }
            catch
            {
                return null;
            }
        }
        public async Task<HttpResponseMessage> LogOut(string token)
        {
            return await Get($"auth/customlogout?token={token}");
        }

        public async Task<string> VerifyToken(string token)
        {
            var resp = await Get($"auth/verifytoken?token={token}");
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
