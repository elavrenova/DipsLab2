using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TransferService.Models
{
    public class Transfer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Carrying { get; set; }
        public int Status { get; set; }
        
    }
}
