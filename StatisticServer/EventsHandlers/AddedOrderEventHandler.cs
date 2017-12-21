using StatisticServer.EventBus;
using StatisticServer.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StatisticServer.EventsHandlers
{
    public class AddedOrderEventHandler : EventHandler<AddedOrderEvent>
    {
        public AddedOrderEventHandler(RabbitMQEventBus eventBus, DBProxy proxy) : base(eventBus, proxy)
        {
        }

        public async override Task Handle(AddedOrderEvent @event)
        {
            try
            {
                var eventDescription = $"{@event.GetType().Name} { @event}";
                Entities.OperationInfo entity = new Entities.OperationInfo
                {
                    Operation = Entities.Operation.AddedOrder,
                    Username = @event.User,
                    Time = @event.OccurenceTime,
                    Id = @event.Id + @event.GetType().Name
                };
                if (dbContext.UserOperations.FirstOrDefault(r => r.Id == entity.Id) == null)
                {
                    dbContext.UserOperations.Add(entity);
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
