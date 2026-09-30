using Notification.Core.Config;
using Notification.Core.Interface;
using NotificationApp.Core.Interfaces;
using System.Text.Json;

namespace NotificationApp.Services.Config;

/// <summary>
/// Carica ChannelConfig da un file JSON (channels.config.json) tramite IFileSystem,
/// così nei test si può iniettare un MockFileSystem con contenuto JSON arbitrario.
/// </summary>
public sealed class JsonChannelConfigLoader : IChannelConfigLoader
{
    private readonly IFileWriter _fileSystem;
    private readonly string _configPath;

    public JsonChannelConfigLoader(IFileWriter fileSystem, string configPath = "C:\\Users\\Utente\\source\\repos\\Notification.Management\\channels.config.json")
    {
        _fileSystem = fileSystem;
        _configPath = configPath;
    }

    public async Task<ChannelConfig> LoadAsync(CancellationToken cancellationToken = default)
    {
        if (!_fileSystem.Exists(_configPath))
        {
            // Fallback prudente: tutti i canali disabilitati se manca il config,
            // invece di far crashare l'app all'avvio.
            return new ChannelConfig();
        }

        var json = await _fileSystem.ReadAllTextAsync(_configPath, cancellationToken);
        var config = JsonSerializer.Deserialize<ChannelConfig>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return config ?? new ChannelConfig();
    }
}
