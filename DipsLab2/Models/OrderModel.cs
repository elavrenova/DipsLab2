﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DipsLab2.Models
{
    public class OrderModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int StockId { get; set; }
        public double Value { get; set; }
        public int Status { get; set; }
        public int TransferId { get; set; }
    }
}
