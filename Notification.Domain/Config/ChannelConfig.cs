using System;
using System.Collections.Generic;
using System.Text;

namespace Notification.Core.Config;

public sealed class ChannelConfig
{
    public Dictionary<string, bool> Channels { get; init; } = new();

    public bool IsEnabled(string channelName) =>
        Channels.TryGetValue(channelName, out var enabled) && enabled;
}

