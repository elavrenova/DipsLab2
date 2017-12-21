using StatisticServer.EventBus;
using StatisticServer.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StatisticServer.EventsHandlers
{
    public class LoginHandler : EventHandler<LoginEvent>
    {

        public LoginHandler(RabbitMQEventBus eventBus, DBProxy proxy): base(eventBus, proxy)
        {
        }

        public async override Task Handle(LoginEvent @event)
        {
            try
            {
                var eventDescription = $"{@event.GetType().Name} { @event}";
                Entities.LoginInfo entity = new Entities.LoginInfo
                {
                    DateTime = @event.OccurenceTime,
                    Username = @event.Name,
                    From = @event.Origin,
                    Id = @event.Id + @event.GetType().Name
                };
                if (dbContext.Logins.FirstOrDefault(r => r.Id == entity.Id) == null)
                {
                    dbContext.Logins.Add(entity);
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
