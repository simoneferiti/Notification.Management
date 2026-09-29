using System.IO.Abstractions;
using Notification.Core.Interface;
using Notification.Core.Models;

namespace NotificationApp.Services.Channels;

/// <summary>
/// Canale LogFile: scrive ogni notifica su un file .log con timestamp e priorità.
/// Usa IFileSystem (System.IO.Abstractions) invece di File.* statico, così nei
/// test si può iniettare un MockFileSystem senza toccare il disco reale.
/// </summary>
public sealed class LogFileChannel : INotificationChannel
{
    private readonly IFileSystem _fileSystem;
    private readonly string _logFilePath;

    public LogFileChannel(IFileSystem fileSystem, string logFilePath = "notifications.log")
    {
        _fileSystem = fileSystem;
        _logFilePath = logFilePath;
    }

    public string Name => "logFile";

    public bool ShouldHandle(NotificationEvent notification) => true;

    public async Task DeliverAsync(NotificationEvent notification, CancellationToken cancellationToken = default)
    {
        var line = $"{notification.Timestamp:O} [{notification.Priority}] {notification.Title} - {notification.Message}{Environment.NewLine}";
        await using var stream = _fileSystem.File.Open(_logFilePath, FileMode.Append, FileAccess.Write);
        await using var writer = new StreamWriter(stream);
        await writer.WriteAsync(line);
    }
}
