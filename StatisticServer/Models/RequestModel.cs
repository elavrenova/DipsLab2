using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StatisticServer.Models
{
    public class RequestModel
    {
        public string From { get; set; }
        public string Time { get; set; }
        public int Count { get; set; }
    }
}
