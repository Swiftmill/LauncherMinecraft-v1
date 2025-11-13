using System.Text.Json.Serialization;

namespace AstraLauncher.Models;

public class GameProfile
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("version")]
    public string Version { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = "release";

    [JsonPropertyName("loader")]
    public string Loader { get; set; } = "Vanilla";

    [JsonPropertyName("gameDirectory")]
    public string GameDirectory { get; set; } = string.Empty;
}
