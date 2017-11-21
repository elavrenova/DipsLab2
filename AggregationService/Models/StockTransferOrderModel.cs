using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AggregationService.Models
{
    public class StockTransferOrderModel
    {
        public int OrderId { get; set; }
        [Display(Name="Stock")]
        public int StockId { get; set; }
        public int OrderStatus { get; set; }
        public int TransferId { get; set; }
        public double Value { get; set; }
        public int UserId { get; set; }
    }
}
