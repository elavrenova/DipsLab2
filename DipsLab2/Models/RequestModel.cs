using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DipsLab2.Models
{
    public class RequestModel
    {
        public int UserId { get; set; }
        public int StockId { get; set; }
        public double Value { get; set; }
    }
}
