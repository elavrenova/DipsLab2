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
            Initialize();
        }
        public StockContext(DbContextOptions<StockContext> options)
            : base(options)
        {
            Initialize();
        }

        private void Initialize()
        {
            if (!Stocks.Any())
            {
                Stocks.Add(new Stock { FreePlace = 500000, Name = "Stock 1" });
                Stocks.Add(new Stock { FreePlace = 700000, Name = "Stock 2" });
                Stocks.Add(new Stock { FreePlace = 900000, Name = "Stock 3" });
                SaveChanges();
            }
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
