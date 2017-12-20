using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StatisticServer.Entities
{
    public class OrderValuesInfo
    {
        public string Id { get; set; }
        public string Author { get; set; }
        public string Value { get; set; }
        public DateTime AddedTime { get; set; }
    }
}
