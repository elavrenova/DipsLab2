using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StatisticServer.Models
{
    public class OrderAdditionDetailModel
    {
        public string Author { get; set; }
        public string Stock { get; set; }
        public string Value { get; set; }
        public string Time { get; set; }
    }
}
