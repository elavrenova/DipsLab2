using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TransferService.Models
{
    public class TransferContext : DbContext
    {
        public TransferContext(DbContextOptions<TransferContext> options)
            : base(options)
        {
        }

        public TransferContext()
            : base()
        {
        }
        public DbSet<Transfer> Transfers { get; set; }
    }
}
