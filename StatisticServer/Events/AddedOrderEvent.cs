using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StatisticServer.Events
{
    public class AddedOrderEvent : Event
    {
        public string User { get; set; }
    }
}
