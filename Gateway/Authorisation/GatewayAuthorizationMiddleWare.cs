using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gateway.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Libuv.Internal.Networking;
using StatisticServer.EventBus;
using StatisticServer.Events;

namespace Gateway.Authorisation
{
    public class GatewayAuthorizationMiddleWare : AuthorizationMiddleWare
    {
        private IAuthService authService;
        private RabbitMQEventBus eventBus;

        public GatewayAuthorizationMiddleWare(RequestDelegate next, IAuthService authService, RabbitMQEventBus eventBus) : base(next)
        {
            this.eventBus = eventBus;
            this.authService = authService;
        }

        public override async Task Invoke(HttpContext context)
        {
            eventBus.PublishEvent(new RequestEvent
            {
                Host = context.Connection.LocalIpAddress.ToString() + ":" + context.Connection.LocalPort.ToString(),
                Origin = context.Connection.RemoteIpAddress.ToString() + ":" + context.Connection.RemotePort.ToString(),
                Route = context.Request.Path.ToString(),
                RequestType = RequestType.Gateway,
                OccurenceTime = DateTime.Now
            }, true);
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
        public override string GetRoleByToken(string token)
        {
            return authService.GetRole(token)?.Result;
        }
    }
}
