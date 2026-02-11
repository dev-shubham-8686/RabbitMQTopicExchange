using RabbitMQ.Client;

namespace Common.Messaging
{

    public interface IRabbitMqConnection : IDisposable
    {
        IConnection GetConnection();
        IModel CreateChannel();
    }

    public sealed class RabbitMqConnection : IRabbitMqConnection
    {
        private readonly ConnectionFactory _factory;
        private IConnection? _conn;

        public RabbitMqConnection(RabbitMqOptions opt)
        {
            _factory = new ConnectionFactory
            {
                HostName = opt.HostName,
                Port = opt.Port,
                UserName = opt.UserName,
                Password = opt.Password,
                VirtualHost = opt.VirtualHost,
                DispatchConsumersAsync = true
            };
        }

        public IConnection GetConnection()
        {
            if (_conn is { IsOpen: true }) return _conn;
            _conn?.Dispose();
            _conn = _factory.CreateConnection("svc-connection");
            return _conn;
        }

        public IModel CreateChannel() => GetConnection().CreateModel();

        public void Dispose() => _conn?.Dispose();
    }

}
