using System.Text.Json;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Common.Messaging;

public abstract class EventConsumerBackgroundService<T> : BackgroundService
{
    private readonly IRabbitMqConnection _conn;
    private readonly RabbitMqOptions _opt;
    private readonly string _queue;
    private readonly string _routingKey;
    private IModel? _ch;
    private static readonly JsonSerializerOptions _json = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    protected EventConsumerBackgroundService(IRabbitMqConnection conn, RabbitMqOptions opt, string queue, string routingKey)
    {
        _conn = conn;
        _opt = opt;
        _queue = queue;
        _routingKey = routingKey;
    }

    protected abstract Task HandleAsync(T message, CancellationToken ct);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _ch = _conn.CreateChannel();
        _ch.ExchangeDeclare(_opt.EventsExchange, ExchangeType.Topic, durable: true);
        _ch.QueueDeclare(queue: _queue, durable: true, exclusive: false, autoDelete: false);
        _ch.QueueBind(_queue, _opt.EventsExchange, _routingKey);
        _ch.BasicQos(prefetchSize: 0, prefetchCount: 10, global: false);

        var consumer = new AsyncEventingBasicConsumer(_ch);
        consumer.Received += async (_, ea) =>
        {
            try
            {
                var message = JsonSerializer.Deserialize<T>(ea.Body.Span, _json)!;
                await HandleAsync(message, stoppingToken);
                _ch.BasicAck(ea.DeliveryTag, multiple: false);
            }
            catch
            {
                _ch.BasicNack(ea.DeliveryTag, multiple: false, requeue: false);
            }
        };

        _ch.BasicConsume(_queue, autoAck: false, consumer: consumer);
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    public override void Dispose()
    {
        _ch?.Dispose();
        base.Dispose();
    }
}