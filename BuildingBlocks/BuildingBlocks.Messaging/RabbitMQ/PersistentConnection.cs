using RabbitMQ.Client;

public class PersistentConnection : IDisposable
{
    private readonly IConnectionFactory _factory;
    private IConnection? _connection;
    private readonly object _syncRoot = new();
    private bool _disposed;

    public PersistentConnection(IConnectionFactory factory)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    public bool IsConnected =>
        _connection is not null
     && _connection.IsOpen
     && !_disposed;

    private bool TryConnect()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(PersistentConnection));
        if (IsConnected) return true;

        lock (_syncRoot)
        {
            if (IsConnected) return true;

            // retry/back-off
            const int maxAttempts = 5;
            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    _connection = _factory.CreateConnection();
                    _connection.ConnectionShutdown += OnConnectionShutdown;
                    return true;
                }
                catch
                {
                    Thread.Sleep(TimeSpan.FromSeconds(2 * attempt));
                }
            }

            return false;
        }
    }

    private void OnConnectionShutdown(object? sender, ShutdownEventArgs e)
    {
        // limpa para que a próxima tentativa reconecte
        _connection?.Dispose();
        _connection = null;
    }

    public IModel CreateModel()
    {
        if (!TryConnect())
            throw new InvalidOperationException("Could not connect to RabbitMQ.");

        return _connection!.CreateModel();
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        try { _connection?.Dispose(); }
        catch { /* swallow */ }
    }
}