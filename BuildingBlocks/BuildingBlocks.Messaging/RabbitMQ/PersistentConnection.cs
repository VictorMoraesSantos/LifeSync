using BuildingBlocks.Messaging.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System.Net.Sockets;

public class PersistentConnection : IDisposable
{
    private readonly ConnectionFactory _factory;
    private IConnection? _connection;
    private readonly object _syncRoot = new();
    private bool _disposed;
    private readonly ILogger<PersistentConnection> _logger;
    private readonly string _clientProvidedName;

    public PersistentConnection(ConnectionFactory factory, IOptions<RabbitMqSettings> options, ILogger<PersistentConnection> logger)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _clientProvidedName = options?.Value?.ClientProvidedName ?? "LifeSync";

        // apply client provided name
        _factory.ClientProvidedName = _clientProvidedName;
    }

    public bool IsConnected => _connection is not null && _connection.IsOpen && !_disposed;

    private bool TryConnect()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(PersistentConnection));
        if (IsConnected) return true;

        lock (_syncRoot)
        {
            if (IsConnected) return true;

            const int maxAttempts = 5;
            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    _logger.LogInformation("Attempting to connect to RabbitMQ ({Attempt}/{MaxAttempts})", attempt, maxAttempts);

                    var connection = _factory.CreateConnectionAsync().GetAwaiter().GetResult();
                    _connection = connection;

                    _logger.LogInformation("Successfully connected to RabbitMQ");
                    return true;
                }
                catch (BrokerUnreachableException ex)
                {
                    _logger.LogWarning(ex, "RabbitMQ broker unreachable on attempt {Attempt}/{MaxAttempts}", attempt, maxAttempts);
                }
                catch (ConnectFailureException ex)
                {
                    _logger.LogWarning(ex, "RabbitMQ connect failure on attempt {Attempt}/{MaxAttempts}", attempt, maxAttempts);
                }
                catch (SocketException ex)
                {
                    _logger.LogWarning(ex, "RabbitMQ socket exception on attempt {Attempt}/{MaxAttempts}", attempt, maxAttempts);
                }
                catch (TimeoutException ex)
                {
                    _logger.LogWarning(ex, "RabbitMQ connection timeout on attempt {Attempt}/{MaxAttempts}", attempt, maxAttempts);
                }

                var delaySeconds = Math.Min(30, (int)Math.Pow(2, attempt));
                _logger.LogInformation("Waiting {Delay}s before next RabbitMQ connection attempt", delaySeconds);
                Thread.Sleep(TimeSpan.FromSeconds(delaySeconds));
            }

            _logger.LogError("Could not connect to RabbitMQ after {MaxAttempts} attempts", maxAttempts);
            return false;
        }
    }

    public IChannel CreateChannel()
    {
        if (!TryConnect())
            throw new InvalidOperationException("Could not connect to RabbitMQ.");

        return _connection!.CreateChannelAsync().GetAwaiter().GetResult();
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        try
        {
            _connection?.Dispose();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error disposing RabbitMQ connection");
        }
        finally
        {
            _connection = null;
        }
    }
}