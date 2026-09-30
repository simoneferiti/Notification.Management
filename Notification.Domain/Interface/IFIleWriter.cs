namespace Notification.Core.Interface;

public interface IFileWriter
{
    bool Exists(string path);
    Task<string> ReadAllTextAsync(string path, CancellationToken ct = default);
    Task AppendAllTextAsync(string path, string content, CancellationToken ct = default);
}
