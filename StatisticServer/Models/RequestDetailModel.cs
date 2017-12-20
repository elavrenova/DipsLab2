using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StatisticServer.Models
{
    public class RequestDetailModel
    {
        public string From { get; set; }
        public string To { get; set; }
        public DateTime Time { get; set; }
    }
}
