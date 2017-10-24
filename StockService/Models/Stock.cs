using DipsLab2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockService.Models
{
    public class Stock
    {
        public Stock(StockModel stockModel)
        {
            this.Name = stockModel.Name;
            this.FreePlace = stockModel.FreePlace;
        }

        public Stock()
        {
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public double FreePlace { get; set; }
    }
}
