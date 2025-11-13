using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using AstraLauncher.Models;

namespace AstraLauncher.Services;

public class ProfileService
{
    private readonly JsonDocument _configuration;
    private readonly List<GameProfile> _profiles = new();

    public IReadOnlyList<GameProfile> Profiles => _profiles;

    public ProfileService(JsonDocument configuration)
    {
        _configuration = configuration;
    }

    public void Load()
    {
        try
        {
            var path = _configuration.RootElement.GetProperty("Launcher").GetProperty("ProfilesConfig").GetString();
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            var resolved = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, path));
            if (!File.Exists(resolved))
            {
                LoggingService.LogWarning($"Le fichier des profils est introuvable: {resolved}");
                return;
            }

            using var stream = File.OpenRead(resolved);
            var document = JsonDocument.Parse(stream);
            if (document.RootElement.TryGetProperty("profiles", out var profilesElement))
            {
                _profiles.Clear();
                foreach (var profileElement in profilesElement.EnumerateArray())
                {
                    var profile = profileElement.Deserialize<GameProfile>();
                    if (profile != null)
                    {
                        _profiles.Add(profile);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            LoggingService.LogException(ex);
        }
    }

    public GameProfile? GetDefaultProfile(string? defaultId)
    {
        if (string.IsNullOrWhiteSpace(defaultId))
        {
            return _profiles.FirstOrDefault();
        }

        return _profiles.FirstOrDefault(p => p.Id.Equals(defaultId, StringComparison.OrdinalIgnoreCase));
    }
}
