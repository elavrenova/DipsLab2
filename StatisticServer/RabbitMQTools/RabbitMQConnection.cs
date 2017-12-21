using Microsoft.Extensions.Configuration;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;


namespace StatisticServer.RabbitMQTools
{
    public class RabbitMQConnection
    {
        private readonly IConnectionFactory connectionFactory;
        IConnection connection;
        bool disposed;
        int retry = 2;
        object sync_root = new object();

        public RabbitMQConnection(IConfiguration configuration)
        {
            connectionFactory = new ConnectionFactory { Uri = new Uri(configuration.GetConnectionString("MQ")) };
        }

        public bool IsConnected
        {
            get
            {
                return connection != null && connection.IsOpen && !disposed;
            }
        }

        public IModel CreateModel()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("There are no connected channels");
            }

            return connection.CreateModel();
        }

        public void Dispose()
        {
            if (disposed) return;
            disposed = true;
            try
            {
                connection.Dispose();
            }
            catch
            {

            }
        }

        public bool TryConnect()
        {
            lock (sync_root)
            {
                var policy = Policy.Handle<SocketException>()
                   .Or<BrokerUnreachableException>()
                   .WaitAndRetry(retry, retryAttempt => TimeSpan.FromSeconds(5), (ex, time) =>{}
                );
                policy.Execute(() => connection = connectionFactory.CreateConnection());
                if (IsConnected)
                {
                    connection.ConnectionShutdown += (sender, ea) =>
                    {
                        if (disposed) return;
                        TryConnect();
                    };
                    connection.CallbackException += (sender, ea) =>
                    {
                        if (disposed) return;
                        TryConnect();
                    };
                    connection.ConnectionBlocked += (sender, ea) =>
                    {
                        if (disposed) return;
                        TryConnect();
                    };
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
