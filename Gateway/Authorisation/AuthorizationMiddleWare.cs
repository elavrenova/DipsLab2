using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Gateway.Services;
using Microsoft.AspNetCore.Http;

namespace Gateway.Authorisation
{
    public abstract class AuthorizationMiddleWare
    {
        public static string AuthorizationWord = "Authorization";
        public static string UserWord = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";
        public static string RoleWord = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
        protected readonly RequestDelegate _next;
        //protected readonly TokensStore tokensStore;

        public AuthorizationMiddleWare(RequestDelegate next)
        {
            _next = next;
        }

        public virtual async Task Invoke(HttpContext context)
        {
            if (context.Request.Cookies.Keys.Contains(AuthorizationWord))
            {
                var auth = context.Request.Cookies[AuthorizationWord];
                await CheckBearerAuthorization(context, auth);
            }
            else if (context.Request.Path.Value.Split('/').Intersect(GetAnonymousPaths()).Any())
            {
                await this._next(context);
            }
            else
            {
                var message = "No authorization header provided";
                await ReturnForbidden(context, message);
            }
        }


        protected async Task CheckBearerAuthorization(HttpContext context, string auth)
        {
            var match = Regex.Match(auth, @"Bearer (\S+)");
            if (match.Groups.Count == 1)
            {
                await ReturnForbidden(context, "Invalid token format");
            }
            else
            {
                var token = match.Groups[1].Value;
                var result = GetUserByToken(token);
                if (!string.IsNullOrWhiteSpace(result))
                {
                    context.Request.Headers.Add(UserWord, result);
                    await this._next(context);
                }
                else
                { 
                    await ReturnForbidden(context, "Token not valid");
                }
            }
        }

        public abstract Task ReturnForbidden(HttpContext context, string message);

        public abstract List<string> GetAnonymousPaths();

        public abstract string GetUserByToken(string token);
    }
}
