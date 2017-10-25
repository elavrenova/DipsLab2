using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockService.Models
{
    public class StockContext : DbContext
    {
        public StockContext() : base()
        {
            
        }
        public StockContext(DbContextOptions<StockContext> options)
            : base(options)
        {
        }

        public DbSet<Stock> Stocks { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Stock>(stock =>
            {
                stock.HasKey(x => x.Id);
            });
        }
    }
}
