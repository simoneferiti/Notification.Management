using Notification.Core.Interface;

namespace Notification.Service;

public sealed class FileWriter : IFileWriter
{
    public bool Exists(string path) => File.Exists(GetFullFileName(path));

    public Task<string> ReadAllTextAsync(string path, CancellationToken ct = default) =>
        File.ReadAllTextAsync(GetFullFileName(path), ct);

    public Task AppendAllTextAsync(string path, string content, CancellationToken ct = default) =>
        File.AppendAllTextAsync(GetFullFileName(path), content, ct);

    private string GetFullFileName(string fileName)
    {
        var baseFolder = AppContext.BaseDirectory; 
        return Path.Combine(baseFolder, fileName);
    }
        
}