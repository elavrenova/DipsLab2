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
            Initialize();
        }

        public TransferContext()
            : base()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (!Transfers.Any())
            {
                Transfers.Add(new Transfer { Carrying = 1000, Name = "Transfer 1", Status = 0 });
                Transfers.Add(new Transfer { Carrying = 2000, Name = "Transfer 2", Status = 0 });
                Transfers.Add(new Transfer { Carrying = 3000, Name = "Transfer 3", Status = 0 });
                Transfers.Add(new Transfer { Carrying = 4000, Name = "Transfer 4", Status = 0 });
                Transfers.Add(new Transfer { Carrying = 5000, Name = "Transfer 5", Status = 0 });
                Transfers.Add(new Transfer { Carrying = 6000, Name = "Transfer 6", Status = 0 });
                Transfers.Add(new Transfer { Carrying = 7000, Name = "Transfer 7", Status = 0 });
                Transfers.Add(new Transfer { Carrying = 8000, Name = "Transfer 81", Status = 0 });
                Transfers.Add(new Transfer { Carrying = 8000, Name = "Transfer 82", Status = 0 });
                Transfers.Add(new Transfer { Carrying = 8000, Name = "Transfer 83", Status = 0 });
                Transfers.Add(new Transfer { Carrying = 8000, Name = "Transfer 84", Status = 0 });
                SaveChanges();
            }
        }

        public DbSet<Transfer> Transfers { get; set; }
    }
}
