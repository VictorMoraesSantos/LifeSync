using RabbitMQ.Client;

public class PersistentConnection : IDisposable
{
    private readonly IConnectionFactory _connectionFactory;
    private IConnection? _connection;
    private bool _disposed;

    public PersistentConnection(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        TryConnect();
    }

    public bool IsConnected => _connection != null && _connection.IsOpen && !_disposed;

    public void TryConnect()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(PersistentConnection));

        _connection?.Dispose();
        _connection = _connectionFactory.CreateConnection();
    }

    public void ExecuteOnChannel(Action<IModel> action)
    {
        if (!IsConnected)
            TryConnect();

        using var channel = _connection!.CreateModel();
        action(channel);
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _connection?.Dispose();
    }
}