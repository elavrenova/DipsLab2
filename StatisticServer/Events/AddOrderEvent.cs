using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StatisticServer.Events
{
    public class AddOrderEvent : Event
    {
        public string Author { get; set; }
        public string Value { get; set; }
        public string Stock { get; set; }
    }
}
