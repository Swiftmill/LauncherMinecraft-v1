using System;
using System.Text.Json.Serialization;

namespace AstraLauncher.Models;

public class NewsArticle
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;

    [JsonPropertyName("date")]
    public string Date { get; set; } = string.Empty;

    public DateTime? ParsedDate
    {
        get
        {
            if (DateTime.TryParse(Date, out var parsed))
            {
                return parsed;
            }

            return null;
        }
    }
}
