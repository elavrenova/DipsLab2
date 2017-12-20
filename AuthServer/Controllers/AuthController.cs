using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthServer.Entities;
using AuthServer.Models;
using Gateway.Models;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthServer.Controllers
{
    [Route("account")]
    public class AuthController : Controller
    {
        private ApplicationDbContext dbContext;
        private UserContext userDbContext;
        private TokenContext tokenDbContext;

        public AuthController(ApplicationDbContext dbContext, UserContext userDbContext, TokenContext tokenDbContext)
        {
            this.dbContext = dbContext;
            this.userDbContext = userDbContext;
            this.tokenDbContext = tokenDbContext;
        }

        [HttpGet("login")]
        public async Task<IActionResult> Login(string returnUrl)
        {
            //return await Login(new UserLogin { Username = "User1", Password = "pass1", ReturnUrl = returnUrl });
            return View(new UserLogin { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(UserLogin loginModel)
        {
            var user = dbContext.Users.FirstOrDefault(u => u.Username == loginModel.Username);
            if (user != null)
            {
                if (user.Password == loginModel.Password.Sha256())
                {
                    await AuthenticationManagerExtensions.SignInAsync(HttpContext, user.Id.ToString(), user.Username, new Claim("Name", loginModel.Username));
                    return Redirect(loginModel.ReturnUrl);
                }
            }
            return Redirect("~/");
        }

        [HttpPost("customlogin")]
        public async Task<IActionResult> CustomLogin([FromBody]UserModel userModel)
        {
            var user = userDbContext.Users.FirstOrDefault(u => u.Username == userModel.Username);
            if (user != null)
            {
                if (user.Password == userModel.Password.Sha256())
                {
                    var token = new Entities.Token
                    {
                        Id = Guid.NewGuid().ToString(),
                        Expiration = DateTime.Now + TimeSpan.FromMinutes(30),
                        Owner = userModel.Username,
                        Role = user.Role
                    };
                    tokenDbContext.Add(token);
                    tokenDbContext.SaveChanges();
                    return Ok(token.Id);
                }
                else
                {
                    return StatusCode(500, "Wrong password");
                }
            }
            return StatusCode(500, "User not found");
        }

        [HttpGet("customlogout")]
        public async Task<IActionResult> CustomLogout(string token)
        {
            var tkn = tokenDbContext.Tokens.FirstOrDefault(x => x.Id == token);
            tokenDbContext.Remove(tkn);
            tokenDbContext.SaveChanges();
            return Ok();
        }

        [HttpGet("verifytoken")]
        public async Task<IActionResult> VerifyToken(string token)
        {
            var tkn = tokenDbContext.Tokens.FirstOrDefault(x => x.Id == token);
            if (tkn != null)
            {
                if (tkn.Expiration > DateTime.Now)
                    return Ok(tkn.Owner);
                tokenDbContext.Remove(tkn);
                tokenDbContext.SaveChanges();
            }
            return Unauthorized();
        }
        [HttpGet("getrole")]
        public async Task<IActionResult> GetRole(string token)
        {
            var tkn = tokenDbContext.Tokens.FirstOrDefault(x => x.Id == token);
                    return Ok(tkn.Role);
        }

    }
}