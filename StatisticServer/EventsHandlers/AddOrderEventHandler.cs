using StatisticServer.EventBus;
using StatisticServer.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StatisticServer.EventsHandlers
{
    public class AddOrderEventHandler : EventHandler<AddOrderEvent>
    {
        public AddOrderEventHandler(RabbitMQEventBus eventBus, DBProxy proxy): base(eventBus, proxy)
        {
        }

        public async override Task Handle(AddOrderEvent @event)
        {
            try
            {
                Entities.AddOrderInfo entity = new Entities.AddOrderInfo
                {
                    Id = @event.Id + @event.GetType().Name,
                    AddedTime = @event.OccurenceTime,
                    Author = @event.Author,
                    Value = @event.Value,
                    Stock = @event.Stock
                };
                if (dbContext.OrderAdditions.FirstOrDefault(r => r.Id == entity.Id) == null)
                {
                    dbContext.OrderAdditions.Add(entity);
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
