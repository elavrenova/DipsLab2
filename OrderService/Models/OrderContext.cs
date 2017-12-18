using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace OrderService.Models
{
    public class OrderContext : DbContext
    {
        public OrderContext(DbContextOptions<OrderContext> options)
            : base(options)
        {
            Initialize();
        }

        private void Initialize()
        {
            if (!Orders.Any())
            {
                Orders.Add(new Order
                {
                    StockId = 0,
                    Status = 11,
                    TransferId = 0,
                    UserId = 1,
                    Value = 3000
                });
                Orders.Add(new Order
                {
                    StockId = 1,
                    Status = 99,
                    TransferId = 1,
                    UserId = 1,
                    Value = 5000
                });
                SaveChanges();
            }
        }

        public DbSet<Order> Orders { get; set; }

    }
}
