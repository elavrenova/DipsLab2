using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gateway.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Libuv.Internal.Networking;

namespace Gateway.Authorisation
{
    public class GatewayAuthorizationMiddleWare : AuthorizationMiddleWare
    {
        private IAuthService authService;

        public GatewayAuthorizationMiddleWare(RequestDelegate next, IAuthService authService) : base(next)
        {
            this.authService = authService;
        }

        public override async Task Invoke(HttpContext context)
        {
            if (context.Request.Headers.Keys.Contains(AuthorizationWord))
            {
                await this._next(context);
            }
            else
                await base.Invoke(context);
        }

        public override List<string> GetAnonymousPaths() => new[] { "api", "auth", "login", "register", "users/auth" }.ToList();

        public override async Task ReturnForbidden(HttpContext context, string message)
        {
            string redirect = "/users/auth";
            if (context.Request.Path != redirect)
            {
                redirect += $"?Redirect={context.Request.Path}";
            }
            if (context.Request.Cookies.ContainsKey(AuthorizationWord))
            {
                context.Response.Cookies.Delete(AuthorizationWord);
                context.Response.Cookies.Append(AuthorizationWord, "-", new CookieOptions{Expires = DateTimeOffset.Now + TimeSpan.FromDays(-1)});
            }
            context.Response.Redirect(redirect);
        }

        public override string GetUserByToken(string token)
        {
            return authService.VerifyToken(token)?.Result;
        }
    }
}
