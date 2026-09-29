using System;
using System.Collections.Generic;
using System.Text;
using Notification.Core.Models;

namespace Notification.Core.Interface;

public interface INotificationChannel
{
    /// <summary>Nome univoco del canale, usato anche come chiave nel file di config JSON.</summary>
    string Name { get; }

    /// <summary>
    /// Decide se questo canale deve gestire la notifica data (es. Email gestisce solo High/Critical).
    /// Viene valutato dal dispatcher DOPO aver verificato che il canale sia abilitato da config.
    /// </summary>
    bool ShouldHandle(NotificationEvent notification);

    /// <summary>Consegna la notifica. Deve essere asincrona e non bloccare il thread UI.</summary>
    Task DeliverAsync(NotificationEvent notification, CancellationToken cancellationToken = default);
}
