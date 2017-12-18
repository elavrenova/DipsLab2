using Microsoft.Extensions.Configuration;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using StatisticServer.RabbitMQTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace StatisticServer.EventBus
{
    public class RabbitMQEventBus
    {
        private RabbitMQConnection connection;
        private RetryPolicy policy;
        private IModel channel;
        private EventsStorage eventsStorage;
        public RabbitMQEventBus(IConfiguration configuration, EventsStorage eventsStorage)
        {
            //this.connection = new RabbitMQConnection(configuration);
            //policy = Policy.Handle<BrokerUnreachableException>()
            //    .Or<SocketException>()
            //    .Or<InvalidOperationException>()
            //    .WaitAndRetry(retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
            //    {});
            //this.channel = CreateConsumerChannel();
            //this.eventsStorage = eventsStorage;
        }

        //private IModel CreateConsumerChannel()
        //{
        //    if (!connection.IsConnected)
        //        connection.TryConnect();

        //    return policy.Execute(() =>
        //    {
        //        var newChannel = connection.CreateModel();
        //        newChannel.ExchangeDeclare(exchange: exchangeName, type: "direct");
        //        queueName = newChannel.QueueDeclare().QueueName;

        //        var consumer = new EventingBasicConsumer(newChannel);
        //        consumer.ConsumerCancelled += (sender, args) =>
        //        {
        //            channel.Dispose();
        //            channel = CreateConsumerChannel();
        //        };
        //        consumer.Received += async (model, ea) =>
        //        {
        //            var eventName = ea.RoutingKey;
        //            var message = Encoding.UTF8.GetString(ea.Body);
        //            await ProcessEvent(eventName, message);
        //            channel.BasicAck(ea.DeliveryTag, false);
        //        };
        //        newChannel.CallbackException += (sender, ea) =>
        //        {
        //            channel.Dispose();
        //            сhannel = CreateConsumerChannel();
        //        };

        //        channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
        //        return channel;
        //    });
        //}



    }
}
