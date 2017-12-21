using StatisticServer.EventBus;
using StatisticServer.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StatisticServer.EventsHandlers
{
    public class RequestEventHandler : EventHandler<RequestEvent>
    {
        public RequestEventHandler(RabbitMQEventBus eventBus,
            DBProxy proxy)
            : base(eventBus, proxy)
        {
        }

        public async override Task Handle(RequestEvent @event)
        {
            try
            {
                var eventDescription = $"{@event.GetType().Name} { @event}";
                Entities.RequestInfo entity = new Entities.RequestInfo
                {
                    From = $"{@event.Origin}",
                    To = $"{@event.Host}{@event.Route}",
                    Time = @event.OccurenceTime,
                    RequestType = @event.RequestType,
                    Id = @event.Id + @event.GetType().Name
                };
                if (dbContext.Requests.FirstOrDefault(r => r.Id == entity.Id) == null)
                {
                    dbContext.Requests.Add(entity);
                    dbContext.SaveChanges();
                }
                eventBus.PublishEvent(new AckEvent { AdjEventId = @event.Id, Status = AckStatus.Success });
            }
            catch (Exception e)
            {
                eventBus.PublishEvent(new AckEvent { AdjEventId = @event.Id, Description = e.ToString(), Status = AckStatus.Failed });
            }
        }
    }
}
