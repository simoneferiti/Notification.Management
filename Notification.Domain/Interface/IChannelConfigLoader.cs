
using Notification.Core.Config;

namespace Notification.Core.Interfaces;

public interface IChannelConfigLoader
{
    Task<ChannelConfig> LoadAsync(CancellationToken cancellationToken = default);
}
