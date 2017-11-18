using Gateway.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TransferService.Models
{
    public class Transfer
    {
        public Transfer(TransferModel transferModel)
        {
            this.Name = transferModel.Name;
            this.Carrying = transferModel.Carrying;
            this.Status = transferModel.Status;
        }

        public Transfer()
        {
            
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public double Carrying { get; set; }
        public int Status { get; set; }
        
    }
}
