using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StatisticServer.Events
{
    public class DeleteOrderEvent : Event
    {
        public string Author { get; set; }
    }
}
