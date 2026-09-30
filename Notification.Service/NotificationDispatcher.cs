using Notification.Core.Config;
using Notification.Core.Interface;
using Notification.Core.Models;
using System.Collections.Concurrent;

namespace NotificationApp.Services;

/// <summary>
/// Implementazione di riferimento del dispatcher: riceve i canali via DI
/// (IEnumerable&lt;INotificationChannel&gt;), li filtra in base a config + ShouldHandle,
/// e li invoca in parallelo così un canale lento non blocca gli altri né la UI.
/// Un'eccezione in un canale è isolata: non deve impedire la consegna sugli altri.
/// </summary>
public sealed class NotificationDispatcher : INotificationDispatcher
{
    private readonly IReadOnlyList<INotificationChannel> _channels;
    private readonly ChannelConfig _channelConfig;
    private readonly ConcurrentQueue<NotificationEvent> _history = new();

    public event EventHandler<NotificationEvent>? NotificationPublished;

    public NotificationDispatcher(IEnumerable<INotificationChannel> channels, ChannelConfig channelConfig)
    {
        _channels = channels.ToList();
        _channelConfig = channelConfig;
    }

    public IReadOnlyList<NotificationEvent> History => _history.ToList();

    public async Task PublishAsync(NotificationEvent notification, CancellationToken cancellationToken = default)
    {
        _history.Enqueue(notification);

        var activeChannels = _channels
            .Where(c => _channelConfig.IsEnabled(c.Name) && c.ShouldHandle(notification))
            .Select(c => DeliverSafelyAsync(c, notification, cancellationToken));

        await Task.WhenAll(activeChannels);

        NotificationPublished?.Invoke(this, notification);
    }

    /// <summary>
    /// Avvolge la consegna per isolare le eccezioni di un singolo canale:
    /// un canale che fallisce (es. email non raggiungibile) non deve far
    /// fallire Task.WhenAll per tutti gli altri.
    /// </summary>
    private static async Task DeliverSafelyAsync(INotificationChannel channel, NotificationEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            await channel.DeliverAsync(notification, cancellationToken);
        }
        catch (Exception ex)
        {
            // In un progetto reale: log strutturato (es. ILogger<NotificationDispatcher>).
            Console.Error.WriteLine($"Canale '{channel.Name}' ha fallito la consegna: {ex.Message}");
        }
    }
}
