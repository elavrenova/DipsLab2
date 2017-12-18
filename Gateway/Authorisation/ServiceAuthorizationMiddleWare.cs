using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Gateway.Services;
using Microsoft.AspNetCore.Http;

namespace Gateway.Authorisation
{
    public class ServiceAuthorizationMiddleWare : AuthorizationMiddleWare
    {
        private const string serviceWord = "Service";
        private List<(string, string)> allowedApps = new List<(string, string)> { ("AppId", "AppSecret") };
        private TokensStore tokensStore;

        public ServiceAuthorizationMiddleWare(RequestDelegate next, TokensStore tokensStore) : base(next)
        {
            this.tokensStore = tokensStore;
        }

        public override async Task Invoke(HttpContext context)
        {
            if (IsBasicAuthorizationSuccess(context))
            {
                context.Response.StatusCode = 200;
                await context.Response.WriteAsync(tokensStore.GetToken(serviceWord, TimeSpan.FromMinutes(15)));
                return;
            }
            else if (IsBearerAuthorization(context))
            {
                var auth = context.Request.Headers[AuthorizationWord];
                await CheckBearerAuthorization(context, auth);
            }
            else
                await base.Invoke(context);
        }

        private static bool IsBearerAuthorization(HttpContext context)
        {
            return context.Request.Headers.Keys.Contains(AuthorizationWord);
        }

        public override List<string> GetAnonymousPaths()
        {
            return new List<string>();
        }

        public override async Task ReturnForbidden(HttpContext context, string message)
        {
            using (var writer = new StreamWriter(context.Response.Body))
            {
                context.Response.StatusCode = 401;
                await writer.WriteAsync(message);
            }
        }

        private bool IsBasicAuthorizationSuccess(HttpContext context)
        {
            if (context.Request.Headers.Keys.Contains(AuthorizationWord))
            {
                string auth = string.Join(string.Empty, context.Request.Headers[AuthorizationWord]);
                var match = Regex.Match(auth, @"Basic (\S+)");
                if (match.Groups.Count > 1)
                {
                    byte[] appIdAndSecretBytes = Convert.FromBase64String(match.Groups[1].Value);
                    var appIdAndSecret = Encoding.UTF8.GetString(appIdAndSecretBytes).Split(':');
                    if (allowedApps.Contains((appIdAndSecret[0], appIdAndSecret[1])))
                        return true;
                }
            }
            return false;
        }

        public override string GetUserByToken(string token)
        {
            return tokensStore.GetNameByToken(token);
        }
    }
}
