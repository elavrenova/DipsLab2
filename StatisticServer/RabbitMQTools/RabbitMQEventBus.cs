using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using StatisticServer.Events;
using StatisticServer.EventsHandlers;
using StatisticServer.RabbitMQTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StatisticServer.EventBus
{
    public class RabbitMQEventBus
    {
        private RabbitMQConnection connection;
        private RetryPolicy policy;
        private IModel myChannel;
        private EventsStorage eventsStorage;
        int retry = 2;
        private Dictionary<string, List<IEventsHandler>> handlers = new Dictionary<string, List<IEventsHandler>>();
        private List<Type> eventTypes = new List<Type>();
        string queue = "myQueue";

        public RabbitMQEventBus(IConfiguration configuration, EventsStorage eventsStorage)
        {
            this.connection = new RabbitMQConnection(configuration);
            policy = Policy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .Or<InvalidOperationException>()
                .WaitAndRetry(retry, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>{});
            this.myChannel = CreateConsumerChannel();
            this.eventsStorage = eventsStorage;
        }

        private IModel CreateConsumerChannel()
        {
            if (!connection.IsConnected)
                connection.TryConnect();

            return policy.Execute(() =>
            {
                var channel = connection.CreateModel();
                channel.ExchangeDeclare(exchange: "myExchange", type: "direct");
                queue = channel.QueueDeclare().QueueName;

                var consumer = new EventingBasicConsumer(channel);
                consumer.ConsumerCancelled += (sender, args) =>
                {
                    myChannel.Dispose();
                    myChannel = CreateConsumerChannel();
                };
                consumer.Received += async (model, ea) =>
                {
                    var eventName = ea.RoutingKey;
                    var message = Encoding.UTF8.GetString(ea.Body);
                    await ProcessEvent(eventName, message);
                    channel.BasicAck(ea.DeliveryTag, false);
                };
                channel.CallbackException += (sender, ea) =>
                {
                    myChannel.Dispose();
                    myChannel = CreateConsumerChannel();
                };

                channel.BasicConsume(queue: queue, autoAck: false, consumer: consumer);
                return channel;
            });
        }

        public void Publish(Event @event, bool ack = false)
        {
            new Thread(o =>
            {
                try
                {
                    @event = o as Event;
                    if (ack)
                    {
                        int @try = 0;
                        eventsStorage.AddEvent(@event);
                        while (@try < retry)
                        {
                            if ((@event = eventsStorage.GetEvent(@event.Id)) != null)
                                PublishInner(@event);
                            else
                                return;
                            Thread.Sleep(5000);
                        }
                    }
                    else
                    {
                        PublishInner(@event);
                    }
                }
                catch
                {
                }
            }).Start(@event);
        }

        private void PublishInner(Event @event)
        {
            if (!connection.IsConnected)
                connection.TryConnect();

            @event.OccurenceTime = DateTime.Now;
            var name = @event.GetType().FullName;

            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "myExchange", type: "direct");
                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event));
                channel.BasicPublish(exchange: "myExchange",
                    routingKey: name,
                    basicProperties: null,
                    body: body);
            }
        }

        public void Subscribe<T>(EventsHandlers.EventHandler<T> eventHandler) where T : Event
        {
            if (!connection.IsConnected)
                connection.TryConnect();

            var name = typeof(T).FullName;

            if (handlers.ContainsKey(name))
                handlers[name].Add(eventHandler);
            else
            {
                policy.Execute(() =>
                {
                    using (var channel = connection.CreateModel())
                    {
                        channel.QueueBind(queue: queue, exchange: "myExchange", routingKey: name);
                        handlers.Add(name, new List<IEventsHandler>());
                        handlers[name].Add(eventHandler);
                        eventTypes.Add(typeof(T));
                    }
                });
            }
        }

        public void Unsubscribe<T>(EventsHandlers.EventHandler<T> eventHandler) where T : Event
        {
            if (!connection.IsConnected)
                connection.TryConnect();

            var name = typeof(T).FullName;

            if (handlers.ContainsKey(name) && handlers[name].Contains(eventHandler))
                handlers[name].Remove(eventHandler);
        }

        private async Task ProcessEvent(string eventName, string message)
        {
            if (handlers.ContainsKey(eventName))
            {
                var type = Type.GetType(eventName);
                var template = typeof(EventsHandlers.EventHandler<>);
                var @event = JsonConvert.DeserializeObject(message, type);
                foreach (var handler in handlers[eventName])
                {
                    try
                    {
                        string id = (@event as Event).Id;
                        {
                            var genericType = template.MakeGenericType(type);
                            await (Task)genericType.GetMethod("Handle").Invoke(handler, new object[] { @event });
                        }
                    }
                    catch
                    {}
                }
            }
        }


    }
}
