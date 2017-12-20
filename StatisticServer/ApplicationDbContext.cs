using Microsoft.EntityFrameworkCore;
using StatisticServer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StatisticServer
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<LoginInfo> Logins { get; set; }
        public DbSet<RequestInfo> Requests { get; set; }
        public DbSet<OperationInfo> UserOperations { get; set; }
        public DbSet<AddOrderInfo> OrderAdditions { get; set; }
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected ApplicationDbContext()
        {
        }
    }
}
