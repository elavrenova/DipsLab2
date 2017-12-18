using StatisticServer.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StatisticServer
{
    public class EventsStorage
    {
        Dictionary<string, Event> events = new Dictionary<string, Event>();
        object lck = new object();
        public bool AddEvent(Event @event)
        {
            lock (lck)
            {
                if (events.ContainsKey(@event.Id))
                    return false;
                events.Add(@event.Id, @event);
                return true;
            }
        }

        public Event GetEvent(string id)
        {
            lock (lck)
            {
                return events.ContainsKey(id) ? events[id] : null;
            }
        }

        public bool RemoveEvent(string id)
        {
            lock (lck)
            {
                if (events.ContainsKey(id))
                {
                    events.Remove(id);
                    return true;
                }
                return false;
            }
        }
    }
}
