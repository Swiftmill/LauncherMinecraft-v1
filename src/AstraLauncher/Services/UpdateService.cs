using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;
using AstraLauncher.Models;

namespace AstraLauncher.Services;

public class UpdateService
{
    private readonly JsonDocument _configuration;
    private readonly HttpClient _httpClient = new();

    public UpdateService(JsonDocument configuration)
    {
        _configuration = configuration;
    }

    public async Task<UpdateInfo?> CheckForUpdatesAsync(string currentVersion)
    {
        try
        {
            var endpoint = _configuration.RootElement.GetProperty("Launcher").GetProperty("UpdateEndpoint").GetString();
            if (string.IsNullOrWhiteSpace(endpoint))
            {
                return null;
            }

            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var update = JsonSerializer.Deserialize<UpdateInfo>(json);
            if (update == null)
            {
                return null;
            }

            if (string.Compare(update.Version, currentVersion, StringComparison.OrdinalIgnoreCase) > 0)
            {
                return update;
            }
        }
        catch (Exception ex)
        {
            LoggingService.LogException(ex);
        }

        return null;
    }

    public async Task<bool> DownloadUpdateAsync(UpdateInfo update, string destinationPath)
    {
        try
        {
            var response = await _httpClient.GetAsync(update.DownloadUrl);
            response.EnsureSuccessStatusCode();
            await using var stream = await response.Content.ReadAsStreamAsync();
            await using var file = File.Create(destinationPath);
            await stream.CopyToAsync(file);

            if (!string.IsNullOrWhiteSpace(update.Sha256))
            {
                file.Close();
                var actual = ComputeSha256(destinationPath);
                if (!actual.Equals(update.Sha256, StringComparison.OrdinalIgnoreCase))
                {
                    LoggingService.LogWarning("La somme SHA256 ne correspond pas.");
                    return false;
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            LoggingService.LogException(ex);
            return false;
        }
    }

    private static string ComputeSha256(string file)
    {
        using var stream = File.OpenRead(file);
        using var sha = SHA256.Create();
        var hash = sha.ComputeHash(stream);
        return Convert.ToHexString(hash);
    }
}
