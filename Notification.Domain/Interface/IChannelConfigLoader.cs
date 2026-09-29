
using Notification.Core.Config;

namespace NotificationApp.Core.Interfaces;

/// <summary>Carica la configurazione dei canali (es. da un file JSON) all'avvio dell'app.</summary>
public interface IChannelConfigLoader
{
    Task<ChannelConfig> LoadAsync(CancellationToken cancellationToken = default);
}
