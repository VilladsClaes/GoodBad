using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;

namespace GoodBad.Web.Services;

/// <summary>
/// Finds real Reddit threads about a problem. Two modes:
///   1. If a Reddit Client ID + secret is configured (Admin → Indstillinger),
///      the app authenticates with an app-only OAuth token and searches
///      oauth.reddit.com. This is the reliable mode – Reddit blocks anonymous
///      JSON requests from many hosting providers.
///   2. Otherwise it falls back to the public JSON endpoint, which works from
///      most connections but can be rate limited or blocked.
/// No API key is needed for the public mode, but a descriptive User-Agent is.
/// </summary>
public class RedditSearchClient
{
    private const string PublicEndpoint = "https://www.reddit.com/search.json";
    private const string OAuthEndpoint = "https://oauth.reddit.com/search";
    private const string TokenCacheKey = "reddit:access-token";

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ISiteSettings _settings;
    private readonly IConfiguration _configuration;
    private readonly IMemoryCache _cache;
    private readonly ILogger<RedditSearchClient> _logger;

    public RedditSearchClient(
        IHttpClientFactory httpClientFactory,
        ISiteSettings settings,
        IConfiguration configuration,
        IMemoryCache cache,
        ILogger<RedditSearchClient> logger)
    {
        _httpClientFactory = httpClientFactory;
        _settings = settings;
        _configuration = configuration;
        _cache = cache;
        _logger = logger;
    }

    public async Task<string> GetUserAgentAsync()
    {
        var fromSettings = await _settings.GetAsync(SettingKeys.RedditUserAgent);
        if (!string.IsNullOrWhiteSpace(fromSettings))
        {
            return fromSettings;
        }

        return _configuration["Integrations:Reddit:UserAgent"]
               ?? "GoodBad/1.0 (product-review platform; by /u/goodbad)";
    }

    /// <summary>True when a Reddit Client ID + secret is configured (the reliable OAuth mode).</summary>
    public async Task<bool> IsConfiguredAsync()
    {
        var (id, secret) = await GetCredentialsAsync();
        return id is not null && secret is not null;
    }

    private async Task<(string? Id, string? Secret)> GetCredentialsAsync()
    {
        var id = await _settings.GetAsync(SettingKeys.RedditClientId) ?? _configuration["Integrations:Reddit:ClientId"];
        var secret = await _settings.GetAsync(SettingKeys.RedditClientSecret) ?? _configuration["Integrations:Reddit:ClientSecret"];

        return string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(secret) ? (null, null) : (id, secret);
    }

    private async Task<string?> GetAccessTokenAsync(CancellationToken cancellationToken)
    {
        var (clientId, clientSecret) = await GetCredentialsAsync();
        if (clientId is null || clientSecret is null)
        {
            return null;
        }

        if (_cache.TryGetValue(TokenCacheKey, out string? cachedToken) && !string.IsNullOrWhiteSpace(cachedToken))
        {
            return cachedToken;
        }

        try
        {
            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(10);
            client.DefaultRequestHeaders.UserAgent.ParseAdd(await GetUserAgentAsync());

            using var request = new HttpRequestMessage(HttpMethod.Post, "https://www.reddit.com/api/v1/access_token");
            request.Headers.Authorization = new AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}")));
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials"
            });

            using var response = await client.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Kunne ikke hente Reddit-token (status {Status}). Bruger anonym søgning.", (int)response.StatusCode);
                return null;
            }

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

            if (!document.RootElement.TryGetProperty("access_token", out var tokenElement))
            {
                return null;
            }

            var token = tokenElement.GetString();
            var expiresIn = document.RootElement.TryGetProperty("expires_in", out var exp) && exp.TryGetInt32(out var seconds)
                ? seconds
                : 3600;

            if (!string.IsNullOrWhiteSpace(token))
            {
                _cache.Set(TokenCacheKey, token, TimeSpan.FromSeconds(Math.Max(60, expiresIn - 120)));
            }

            return token;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Fejl ved hentning af Reddit-token.");
            return null;
        }
    }

    public async Task<List<ExternalSearchResult>> SearchAsync(string query, int max, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return new List<ExternalSearchResult>();
        }

        max = Math.Clamp(max, 1, 10);
        var cacheKey = $"reddit:{query.Trim().ToLowerInvariant()}:{max}";
        if (_cache.TryGetValue(cacheKey, out List<ExternalSearchResult>? cached) && cached is not null)
        {
            return cached;
        }

        var token = await GetAccessTokenAsync(cancellationToken);
        var url = (token is null ? PublicEndpoint : OAuthEndpoint)
                  + $"?q={Uri.EscapeDataString(query)}&limit={max}&sort=relevance&type=link";

        try
        {
            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(10);
            client.DefaultRequestHeaders.UserAgent.ParseAdd(await GetUserAgentAsync());

            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            if (token is not null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            using var response = await client.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Reddit-søgning fejlede med status {Status} ({Mode}).",
                    (int)response.StatusCode,
                    token is null ? "anonym" : "oauth");
                return new List<ExternalSearchResult>();
            }

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

            var results = new List<ExternalSearchResult>();
            if (document.RootElement.TryGetProperty("data", out var data)
                && data.TryGetProperty("children", out var children)
                && children.ValueKind == JsonValueKind.Array)
            {
                foreach (var child in children.EnumerateArray())
                {
                    if (!child.TryGetProperty("data", out var post))
                    {
                        continue;
                    }

                    if (post.TryGetProperty("over_18", out var over18) && over18.ValueKind == JsonValueKind.True)
                    {
                        continue;
                    }

                    var title = post.TryGetProperty("title", out var t) ? t.GetString() : null;
                    var permalink = post.TryGetProperty("permalink", out var p) ? p.GetString() : null;
                    if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(permalink))
                    {
                        continue;
                    }

                    var subreddit = post.TryGetProperty("subreddit_name_prefixed", out var sr)
                        ? sr.GetString()
                        : post.TryGetProperty("subreddit", out var sr2) ? "r/" + sr2.GetString() : null;

                    var score = post.TryGetProperty("score", out var sc) && sc.TryGetInt32(out var scoreValue)
                        ? $"\u25b2 {scoreValue}"
                        : null;

                    var comments = post.TryGetProperty("num_comments", out var nc) && nc.TryGetInt32(out var commentCount)
                        ? $"{commentCount} kommentarer"
                        : null;

                    results.Add(new ExternalSearchResult(
                        title!,
                        permalink!.StartsWith("http", StringComparison.OrdinalIgnoreCase) ? permalink : "https://www.reddit.com" + permalink,
                        null,
                        "Reddit",
                        string.Join(" \u00b7 ", new[] { subreddit, score, comments }.Where(x => !string.IsNullOrWhiteSpace(x)))));
                }
            }

            var minutes = _configuration.GetValue("Integrations:Reddit:CacheMinutes", 360);
            _cache.Set(cacheKey, results, TimeSpan.FromMinutes(Math.Max(1, minutes)));
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Kunne ikke søge på Reddit.");
            return new List<ExternalSearchResult>();
        }
    }
}
