using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Models
{
    public class Order
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long StockId { get; set; }
        public double Weight { get; set; }
        public long TransferId { get; set; }
        public int Status { get; set; }
    }
}
