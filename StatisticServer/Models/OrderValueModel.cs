using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StatisticServer.Models
{
    public class OrderValueModel
    {
        public string Value { get; set; }
        public string Author { get; set; }
        public int Count { get; set; }
    }
}
