using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthServer.Entities;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthServer
{
    public class ApplicationDbContext : DbContext
    {
        public List<User> Users { get; set; } = new List<User>();

        public ApplicationDbContext() : base()
        {
            Initialize();
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> ops) : base(ops)
        {
            Initialize();
        }

        private void Initialize()
        {
            if (!Users.Any())
            {
                Users.Add(new User { Id = 1, Username = "User1", Password = "pass1".Sha256() });
                Users.Add(new User { Id = 2, Username = "User2", Password = "pass2".Sha256() });
                Users.Add(new User { Id = 3, Username = "User3", Password = "pass3".Sha256() });
                Users.Add(new User { Id = 4, Username = "Admin", Password = "admin".Sha256() });
                SaveChanges();
            }
        }
    }
}
