using Notification.Core.Interface;
using Notification.Core.Models;

namespace Notification.Services.Channels;


public sealed class DisplayChannel : INotificationChannel
{
    public string Name => "display";

    public bool ShouldHandle(NotificationEvent notification) => true;

    public Task DeliverAsync(NotificationEvent notification, CancellationToken cancellationToken = default) =>
        // La consegna vera e propria avviene tramite l'evento del dispatcher;
        // qui non c'è I/O, quindi non serve nulla da testare con mock.
        Task.CompletedTask;
}
