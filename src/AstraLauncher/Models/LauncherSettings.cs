using System;
using System.Text.Json.Serialization;

namespace AstraLauncher.Models;

public class LauncherSettings
{
    [JsonPropertyName("launcherVersion")]
    public string LauncherVersion { get; set; } = "1.0.0";

    [JsonPropertyName("theme")]
    public string Theme { get; set; } = "auto";

    [JsonPropertyName("language")]
    public string Language { get; set; } = "en-US";

    [JsonPropertyName("gameDataRoot")]
    public string GameDataRoot { get; set; } = "%APPDATA%/AstraLauncher";

    [JsonPropertyName("logsDirectory")]
    public string LogsDirectory { get; set; } = "%APPDATA%/AstraLauncher/logs";

    [JsonPropertyName("defaultProfileId")]
    public string DefaultProfileId { get; set; } = string.Empty;

    [JsonPropertyName("network")]
    public NetworkSettings Network { get; set; } = new();

    [JsonPropertyName("game")]
    public GameSettings Game { get; set; } = new();
}

public class NetworkSettings
{
    [JsonPropertyName("cdn")]
    public string Cdn { get; set; } = string.Empty;

    [JsonPropertyName("timeoutSeconds")]
    public int TimeoutSeconds { get; set; } = 30;

    [JsonPropertyName("retryCount")]
    public int RetryCount { get; set; } = 3;
}

public class GameSettings
{
    [JsonPropertyName("minRamMb")]
    public int MinRamMb { get; set; } = 2048;

    [JsonPropertyName("maxRamMb")]
    public int MaxRamMb { get; set; } = 4096;

    [JsonPropertyName("jvmArguments")]
    public string JvmArguments { get; set; } = string.Empty;

    [JsonPropertyName("resolution")]
    public ResolutionSettings Resolution { get; set; } = new();
}

public class ResolutionSettings
{
    [JsonPropertyName("width")]
    public int Width { get; set; } = 1600;

    [JsonPropertyName("height")]
    public int Height { get; set; } = 900;

    [JsonPropertyName("fullscreen")]
    public bool Fullscreen { get; set; }
}
