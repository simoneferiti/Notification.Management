using Notification.Core.Interface;
using Notification.Core.Models;
using Notification.Services.Abstractions;


namespace Notification.Services.Channels;

/// <summary>
/// Canale Email: si attiva SOLO per priorità High o Critical (requisito esplicito).
/// Dipende da ISmtpClient (non da SmtpClient concreto) per poter fare mock nei test.
/// </summary>
public sealed class EmailChannel : INotificationChannel
{
    private readonly ISmtpClient _smtpClient;

    public EmailChannel(ISmtpClient smtpClient)
    {
        _smtpClient = smtpClient;
    }

    public string Name => "email";

    public bool ShouldHandle(NotificationEvent notification) =>
        notification.Priority is NotificationPriority.High or NotificationPriority.Critical;

    public async Task DeliverAsync(NotificationEvent notification, CancellationToken cancellationToken = default)
    {
        var subject = $"[{notification.Priority}] {notification.Title}";
        await _smtpClient.SendAsync(subject, notification.Message, cancellationToken);
    }
}
