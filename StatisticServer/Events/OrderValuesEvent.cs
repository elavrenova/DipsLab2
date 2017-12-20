using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StatisticServer.Events
{
    public class OrderValuesEvent : Event
    {
        public string Author { get; set; }
        public string Value { get; set; }
    }
}
