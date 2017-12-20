using StatisticServer.EventBus;
using StatisticServer.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StatisticServer.EventsHandlers
{
    public class AckHandler : EventHandler<AckEvent>
    {
        private EventsStorage eventsStorage;

        public AckHandler(RabbitMQEventBus eventBus, EventsStorage eventStorage) : base(eventBus, null)
        {
            this.eventsStorage = eventsStorage;
        }

        public override async Task Handle(AckEvent @event)
        {
            switch (@event.Status)
            {
                case AckStatus.Success:
                    eventsStorage.RemoveEvent(@event.AdjEventId);
                    break;
                case AckStatus.Failed:
                    eventsStorage.RemoveEvent(@event.AdjEventId);
                    break;
            }
        }
    }
}
