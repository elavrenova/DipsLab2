using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Gateway.Models
{
    public class StockTransferOrderModel
    {
        public int Id { get; set; }
        [Display(Name="Choose stock name")]
        public int StockId { get; set; }
        public int Status { get; set; }
        [Display(Name = "Choose transfer name")]
        public int TransferId { get; set; }
        [Range(0,1000000)]
        public double Value { get; set; }
        public int UserId { get; set; }
    }
}
