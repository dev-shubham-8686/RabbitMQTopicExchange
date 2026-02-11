using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace Common.Messaging
{

    public sealed class EventPublisher
    {
        private readonly IRabbitMqConnection _conn;
        private readonly RabbitMqOptions _opt;
        private static readonly JsonSerializerOptions _json = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        public EventPublisher(IRabbitMqConnection conn, RabbitMqOptions opt)
        {
            _conn = conn;
            _opt = opt;
        }

        public void Publish<T>(string routingKey, T evt)
        {
            using var ch = _conn.CreateChannel();
            var props = ch.CreateBasicProperties();
            props.DeliveryMode = 2; // persistent
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(evt, _json));
            ch.BasicPublish(_opt.EventsExchange, routingKey, props, body);
        }
    }

}
