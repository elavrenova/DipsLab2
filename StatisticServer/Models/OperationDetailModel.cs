using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StatisticServer.Models
{
    public class OperationDetailModel
    {
        public string Username { get; set; }
        public string Operation { get; set; }
        public DateTime Time { get; set; }
    }
}
