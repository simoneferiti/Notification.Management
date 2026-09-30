using Notification.Core.Interface;

namespace Notification.Service;

public sealed class FileWriter : IFileWriter
{
    public bool Exists(string path) => File.Exists(path);

    public Task<string> ReadAllTextAsync(string path, CancellationToken ct = default) =>
        File.ReadAllTextAsync(path, ct);

    public Task AppendAllTextAsync(string path, string content, CancellationToken ct = default) =>
        File.AppendAllTextAsync(path, content, ct);
}