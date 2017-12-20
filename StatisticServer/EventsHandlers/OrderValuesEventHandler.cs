using StatisticServer.EventBus;
using StatisticServer.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StatisticServer.EventsHandlers
{
    public class OrderValuesEventHandler : EventHandler<OrderValuesEvent>
    {
        public OrderValuesEventHandler(RabbitMQEventBus eventBus, DBProxy proxy): base(eventBus, proxy)
        {
        }

        public async override Task Handle(OrderValuesEvent @event)
        {
            try
            {
                Entities.OrderValuesInfo entity = new Entities.OrderValuesInfo
                {
                    Id = @event.Id + @event.GetType().Name,
                    AddedTime = @event.OccurenceTime,
                    Author = @event.Author,
                    Value = @event.Value
                };
                if (dbContext.OrderValues.FirstOrDefault(r => r.Id == entity.Id) == null)
                {
                    dbContext.OrderValues.Add(entity);
                    dbContext.SaveChanges();
                }
                eventBus.Publish(new AckEvent { AdjEventId = @event.Id, Status = AckStatus.Success });
            }
            catch (Exception e)
            {
                eventBus.Publish(new AckEvent { AdjEventId = @event.Id, Description = e.ToString(), Status = AckStatus.Failed });
            }
        }

        public void Handle(string message)
        {
            var obj = "string";
        }
    }
}
