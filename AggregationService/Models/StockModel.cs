﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AggregationService.Models
{
    public class StockModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double FreePlace { get; set; }
    }
}