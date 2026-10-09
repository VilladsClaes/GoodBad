using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;

namespace GoodBad.Web.Services;

/// <summary>
/// Searches YouTube with the official Data API v3 (true video results, not
/// just a link to a search page). The API key is read from
/// /Admin/Settings first and falls back to appsettings ("Integrations:YouTube:ApiKey").
/// Results are cached aggressively because search.list only allows ~100 free
/// calls per day.
/// </summary>
public class YouTubeSearchClient
{
    private const string Endpoint = "https://www.googleapis.com/youtube/v3/search";

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ISiteSettings _settings;
    private readonly IConfiguration _configuration;
    private readonly IMemoryCache _cache;
    private readonly ILogger<YouTubeSearchClient> _logger;

    public YouTubeSearchClient(
        IHttpClientFactory httpClientFactory,
        ISiteSettings settings,
        IConfiguration configuration,
        IMemoryCache cache,
        ILogger<YouTubeSearchClient> logger)
    {
        _httpClientFactory = httpClientFactory;
        _settings = settings;
        _configuration = configuration;
        _cache = cache;
        _logger = logger;
    }

    public async Task<string?> GetApiKeyAsync()
    {
        var fromSettings = await _settings.GetAsync(SettingKeys.YouTubeApiKey);
        if (!string.IsNullOrWhiteSpace(fromSettings))
        {
            return fromSettings;
        }

        var fromConfig = _configuration["Integrations:YouTube:ApiKey"];
        return string.IsNullOrWhiteSpace(fromConfig) ? null : fromConfig;
    }

    public async Task<bool> IsConfiguredAsync() => await GetApiKeyAsync() is not null;

    public async Task<List<ExternalSearchResult>> SearchAsync(string query, int max, CancellationToken cancellationToken = default)
    {
        var apiKey = await GetApiKeyAsync();
        if (string.IsNullOrWhiteSpace(query) || apiKey is null)
        {
            return new List<ExternalSearchResult>();
        }

        max = Math.Clamp(max, 1, 10);
        var cacheKey = $"yt:{query.Trim().ToLowerInvariant()}:{max}";
        if (_cache.TryGetValue(cacheKey, out List<ExternalSearchResult>? cached) && cached is not null)
        {
            return cached;
        }

        var url = $"{Endpoint}?part=snippet&type=video&maxResults={max}&q={Uri.EscapeDataString(query)}&key={Uri.EscapeDataString(apiKey)}";

        try
        {
            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(10);

            using var response = await client.GetAsync(url, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("YouTube-søgning fejlede med status {Status}.", (int)response.StatusCode);
                return new List<ExternalSearchResult>();
            }

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

            var results = new List<ExternalSearchResult>();
            if (document.RootElement.TryGetProperty("items", out var items) && items.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in items.EnumerateArray())
                {
                    if (!item.TryGetProperty("id", out var id) || !id.TryGetProperty("videoId", out var videoId))
                    {
                        continue;
                    }

                    var snippet = item.TryGetProperty("snippet", out var s) ? s : default;
                    var title = snippet.ValueKind == JsonValueKind.Object && snippet.TryGetProperty("title", out var t)
                        ? t.GetString() ?? "YouTube-video"
                        : "YouTube-video";
                    var channel = snippet.ValueKind == JsonValueKind.Object && snippet.TryGetProperty("channelTitle", out var c)
                        ? c.GetString()
                        : null;

                    results.Add(new ExternalSearchResult(
                        title,
                        $"https://www.youtube.com/watch?v={videoId.GetString()}",
                        null,
                        "YouTube",
                        channel));
                }
            }

            var minutes = _configuration.GetValue("Integrations:YouTube:CacheMinutes", 1440);
            _cache.Set(cacheKey, results, TimeSpan.FromMinutes(Math.Max(1, minutes)));
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Kunne ikke søge på YouTube.");
            return new List<ExternalSearchResult>();
        }
    }
}
