using Notification.Core.Interface;
using Notification.Core.Models;

namespace Notification.Services.Channels;

public sealed class LogFileChannel : INotificationChannel
{
    private readonly IFileWriter _fileSystem;
    private readonly string _logFilePath;

    public LogFileChannel(IFileWriter fileSystem, string logFilePath = "notifications.log")
    {
        _fileSystem = fileSystem;
        _logFilePath = logFilePath;
    }

    public string Name => "logFile";

    public bool ShouldHandle(NotificationEvent notification) => true;

    public async Task DeliverAsync(NotificationEvent notification, CancellationToken cancellationToken = default)
    {
        var line = $"{notification.Timestamp:O} [{notification.Priority}] {notification.Title} - {notification.Message}{Environment.NewLine}";
        await _fileSystem.AppendAllTextAsync(_logFilePath, line);
    }
}
