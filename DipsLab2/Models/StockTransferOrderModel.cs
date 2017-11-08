using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DipsLab2.Models
{
    public class StockTransferOrderModel
    {
        public int Id { get; set; }
        public int StockId { get; set; }
        public int OrderStatus { get; set; }
        public int TransferId { get; set; }
        public double Value { get; set; }
    }
}
