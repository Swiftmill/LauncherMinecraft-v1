using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using AstraLauncher.Models;

namespace AstraLauncher.Services;

public class NewsService
{
    private readonly JsonDocument _configuration;

    public NewsService(JsonDocument configuration)
    {
        _configuration = configuration;
    }

    public (IEnumerable<NewsArticle> Minecraft, IEnumerable<NewsArticle> Launcher) Load()
    {
        try
        {
            var path = _configuration.RootElement.GetProperty("Launcher").GetProperty("NewsFeed").GetString();
            if (string.IsNullOrWhiteSpace(path))
            {
                return (Array.Empty<NewsArticle>(), Array.Empty<NewsArticle>());
            }

            var resolved = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, path));
            if (!File.Exists(resolved))
            {
                LoggingService.LogWarning($"Le fichier des actualités est introuvable: {resolved}");
                return (Array.Empty<NewsArticle>(), Array.Empty<NewsArticle>());
            }

            using var stream = File.OpenRead(resolved);
            var document = JsonDocument.Parse(stream);

            var minecraft = new List<NewsArticle>();
            var launcher = new List<NewsArticle>();

            if (document.RootElement.TryGetProperty("minecraft", out var minecraftElement))
            {
                foreach (var article in minecraftElement.Deserialize<List<NewsArticle>>() ?? new List<NewsArticle>())
                {
                    minecraft.Add(article);
                }
            }

            if (document.RootElement.TryGetProperty("launcher", out var launcherElement))
            {
                foreach (var article in launcherElement.Deserialize<List<NewsArticle>>() ?? new List<NewsArticle>())
                {
                    launcher.Add(article);
                }
            }

            return (minecraft, launcher);
        }
        catch (Exception ex)
        {
            LoggingService.LogException(ex);
            return (Array.Empty<NewsArticle>(), Array.Empty<NewsArticle>());
        }
    }
}
