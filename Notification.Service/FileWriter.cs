using Notification.Core.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notification.Service;

// Services/FileWriter.cs — implementazione reale, solo BCL, nessun pacchetto esterno
public sealed class FileWriter : IFileWriter
{
    public bool Exists(string path) => File.Exists(path);

    public Task<string> ReadAllTextAsync(string path, CancellationToken ct = default) =>
        File.ReadAllTextAsync(path, ct);

    public Task AppendAllTextAsync(string path, string content, CancellationToken ct = default) =>
        File.AppendAllTextAsync(path, content, ct);
}