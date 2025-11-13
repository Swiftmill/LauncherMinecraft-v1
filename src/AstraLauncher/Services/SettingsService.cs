using System;
using System.IO;
using System.Text.Json;
using AstraLauncher.Models;

namespace AstraLauncher.Services;

public class SettingsService
{
    private readonly JsonDocument _configuration;
    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        WriteIndented = true
    };

    public LauncherSettings Settings { get; private set; } = new();
    public string SettingsPath { get; }

    public SettingsService(JsonDocument configuration)
    {
        _configuration = configuration;
        var configPath = configuration.RootElement.GetProperty("Launcher").GetProperty("ConfigPath").GetString() ?? "%APPDATA%/AstraLauncher/settings.json";
        SettingsPath = Environment.ExpandEnvironmentVariables(configPath);

        LoggingService.Configure(configuration.RootElement.GetProperty("Logging").GetProperty("Directory").GetString());
    }

    public void Load()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath)!);
            if (File.Exists(SettingsPath))
            {
                var json = File.ReadAllText(SettingsPath);
                var settings = JsonSerializer.Deserialize<LauncherSettings>(json);
                if (settings != null)
                {
                    Settings = settings;
                }
            }
            else
            {
                Settings = new LauncherSettings();
                Save();
            }
        }
        catch (Exception ex)
        {
            LoggingService.LogException(ex);
            Settings = new LauncherSettings();
        }
    }

    public void Save()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath)!);
            var json = JsonSerializer.Serialize(Settings, _serializerOptions);
            File.WriteAllText(SettingsPath, json);
        }
        catch (Exception ex)
        {
            LoggingService.LogException(ex);
        }
    }
}
