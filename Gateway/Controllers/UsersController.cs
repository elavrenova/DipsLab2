using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gateway.Authorisation;
using Gateway.Models;
using Gateway.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Gateway.Controllers
{
    [Route("users")]
    public class UsersController : Controller
    {
        private IAuthService authService;
        public UsersController(IAuthService authService)
        {
            this.authService = authService;
        }

        [HttpGet("auth")]
        public async Task<IActionResult> Authenticate(AuthenticationModel authenticationModel)
        {
            return View();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(AuthenticationModel authenticationModel)
        {
            var token = await authService.Login(new Models.UserModel { Username = authenticationModel.Username, Password = authenticationModel.Password });
            if (String.IsNullOrWhiteSpace(token))
            {
                var resp = StatusCode(500, "Authentication failed");
                return View("MyError", new ErrorModel(resp));
            }    
            Response.Cookies.Append(AuthorizationMiddleWare.AuthorizationWord, $"Bearer {token}");
            if (authenticationModel.Redirect != null)
                return Redirect(authenticationModel.Redirect);
            return RedirectToAction("Index",nameof(HomeController),null);
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            if (Request.Cookies.Keys.Contains(AuthorizationMiddleWare.AuthorizationWord))
            {
                var cookie = Request.Cookies[AuthorizationMiddleWare.AuthorizationWord];
                Response.Cookies.Append(AuthorizationMiddleWare.AuthorizationWord, cookie, new Microsoft.AspNetCore.Http.CookieOptions { Expires = DateTime.Now.AddDays(-1) });
            }
            return RedirectToAction("Index", nameof(HomeController),null);
        }
    }
}