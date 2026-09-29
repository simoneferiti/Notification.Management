using Notification.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Notification.Core.Interface;

public interface INotificationDispatcher
{
    event EventHandler<NotificationEvent>? NotificationPublished;

    /// <summary>Pubblica la notifica su tutti i canali abilitati, in parallelo.</summary>
    Task PublishAsync(NotificationEvent notification, CancellationToken cancellationToken = default);

    /// <summary>History in sola lettura di tutte le notifiche pubblicate in questa sessione.</summary>
    IReadOnlyList<NotificationEvent> History { get; }
}


