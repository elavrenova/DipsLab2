using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StatisticServer.Events
{
    public class DeletedOrderEvent : Event
    {
        public string User { get; set; }
    }
}
