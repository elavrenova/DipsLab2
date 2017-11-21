using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AggregationService.Models
{
    public class TransferModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Carrying { get; set; }
        public int Status { get; set; }
    }
}
