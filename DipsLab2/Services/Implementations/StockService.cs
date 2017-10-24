using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace DipsLab2.Services.Implementations
{
    public class StockService : Service, IStockService
    {
        public AccountsService(IConfiguration configuration) : 
            base(configuration.GetSection("Addresses")["Accs"]) { }
    }
}
