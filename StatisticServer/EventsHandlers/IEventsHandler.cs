using StatisticServer.EventBus;
using StatisticServer.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StatisticServer.EventsHandlers
{
    public interface IEventsHandler
    {
    }

    public abstract class EventHandler<T> : IEventsHandler, IDisposable
        where T : Event
    {
        protected ApplicationDbContext dbContext;
        protected RabbitMQEventBus eventBus;

        public EventHandler(RabbitMQEventBus eventBus, DBProxy dbProxy)
        {
            this.eventBus = eventBus;
            eventBus.Subscribe(this);
            this.dbContext = dbProxy?.DbContext;
        }

        public void Dispose()
        {
            eventBus.Unsubscribe(this);
        }

        public abstract Task Handle(T @event);
    }
}
