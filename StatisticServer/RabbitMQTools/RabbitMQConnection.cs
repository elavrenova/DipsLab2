using Microsoft.Extensions.Configuration;
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

        //public bool TryConnect()
        //{
        //    lock (sync_root)
        //    {
        //        var policy = Policy.Handle<SocketException>()
        //           .Or<BrokerUnreachableException>()
        //           .WaitAndRetry(retryCount, retryAttempt => TimeSpan.FromSeconds(5), (ex, time) =>
        //           {
        //               logger.LogWarning($"{ex.Message}");
        //           }
        //        );
        //        policy.Execute(() => connection = connectionFactory.CreateConnection());
        //        if (IsConnected)
        //        {
        //            connection.ConnectionShutdown += OnConnectionShutdown;
        //            connection.CallbackException += OnCallbackException;
        //            connection.ConnectionBlocked += OnConnectionBlocked;
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //}

        //private void OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
        //{
        //    if (disposed) return;
        //    TryConnect();
        //}

        //void OnCallbackException(object sender, CallbackExceptionEventArgs e)
        //{
        //    if (disposed) return;
        //    TryConnect();
        //}

        //void OnConnectionShutdown(object sender, ShutdownEventArgs reason)
        //{
        //    if (disposed) return;
        //    TryConnect();
        //}
    }
}
