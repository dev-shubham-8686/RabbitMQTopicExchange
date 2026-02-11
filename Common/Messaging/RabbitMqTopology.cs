using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Messaging
{

    public static class RabbitMqTopology
    {
        public static void Ensure(IRabbitMqConnection conn, RabbitMqOptions opt)
        {
            using var ch = conn.CreateChannel();
            ch.ExchangeDeclare(opt.EventsExchange, ExchangeType.Topic, durable: true, autoDelete: false);
            // Queues are declared by consumers (so services own their queues).
        }
    }

}
