using System;
using System.Collections.Generic;
using System.Text;

namespace Notification.Core.Models;

public sealed record NotificationEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string Title { get; init; }
    public required string Message { get; init; }
    public required NotificationPriority Priority { get; init; }
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.Now;

    public static NotificationEvent Create(string title, string message, NotificationPriority priority) =>
        new()
        {
            Title = title,
            Message = message,
            Priority = priority
        };
}
