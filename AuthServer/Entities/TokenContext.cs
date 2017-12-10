using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AuthServer.Entities
{
    public class TokenContext : DbContext
    {
        public TokenContext(DbContextOptions<TokenContext> options)
            : base(options)
        {
            
        }

        public TokenContext()
            : base()
        {
        }
        public DbSet<Token> Tokens { get; set; }
    }
}
