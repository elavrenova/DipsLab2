using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockService.Models
{
    public class Stock
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public double FreePlace { get; set; }
    }
}
