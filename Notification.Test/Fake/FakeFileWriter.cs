using Notification.Core.Interface;

namespace Notification.Test.Fake;

public sealed class FakeFileWriter : IFileWriter
{
    public Dictionary<string, string> Files { get; } = new();
    public bool Exists(string path) => Files.ContainsKey(path);
    public Task<string> ReadAllTextAsync(string path, CancellationToken ct = default) =>
        Task.FromResult(Files.TryGetValue(path, out var c) ? c : string.Empty);
    public Task AppendAllTextAsync(string path, string content, CancellationToken ct = default)
    {
        Files[path] = Files.TryGetValue(path, out var existing) ? existing + content : content;
        return Task.CompletedTask;
    }
}


