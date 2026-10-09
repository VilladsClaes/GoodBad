using System.Net;
using GoodBad.Web.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace GoodBad.Web.Tests;

/// <summary>
/// The YouTube client is verified against the documented Data API v3 response
/// shape (search.list -> items[].id.videoId / snippet.title).
/// </summary>
public class YouTubeSearchClientTests
{
    private const string TwoVideos = """
    {
      "kind": "youtube#searchListResponse",
      "items": [
        {
          "kind": "youtube#searchResult",
          "id": { "kind": "youtube#video", "videoId": "abc123" },
          "snippet": { "title": "Sådan reparerer du en lynlås", "channelTitle": "FixIt" }
        },
        {
          "kind": "youtube#searchResult",
          "id": { "kind": "youtube#video", "videoId": "xyz789" },
          "snippet": { "title": "Ny lynlås i jakken" }
        }
      ]
    }
    """;

    private static YouTubeSearchClient CreateClient(StubHandler handler, FakeSettings settings, IMemoryCache? cache = null)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Integrations:YouTube:ApiKey"] = string.Empty,
                ["Integrations:YouTube:CacheMinutes"] = "60"
            })
            .Build();

        return new YouTubeSearchClient(
            new StubHttpClientFactory(handler),
            settings,
            configuration,
            cache ?? new MemoryCache(new MemoryCacheOptions()),
            NullLogger<YouTubeSearchClient>.Instance);
    }

    [Fact]
    public async Task Search_parses_real_video_results_and_uses_the_api_key()
    {
        var handler = new StubHandler().Enqueue(HttpStatusCode.OK, TwoVideos);
        var client = CreateClient(handler, new FakeSettings(new()
        {
            [SettingKeys.YouTubeApiKey] = "MIN-NOEGLE"
        }));

        var results = await client.SearchAsync("lynlås reparation", 5);

        Assert.Equal(2, results.Count);
        Assert.Equal("https://www.youtube.com/watch?v=abc123", results[0].Url);
        Assert.Equal("Sådan reparerer du en lynlås", results[0].Title);
        Assert.Equal("FixIt", results[0].Meta);
        Assert.Equal("YouTube", results[0].Source);
        Assert.Equal("Ny lynlås i jakken", results[1].Title);

        var query = handler.Requests.Single().RequestUri!.Query;
        Assert.Contains("part=snippet", query);
        Assert.Contains("type=video", query);
        Assert.Contains("key=MIN-NOEGLE", query);
        Assert.Contains("q=lynl%C3%A5s%20reparation", query);
    }

    [Fact]
    public async Task Search_returns_nothing_without_an_api_key()
    {
        var handler = new StubHandler();
        var client = CreateClient(handler, new FakeSettings());

        Assert.False(await client.IsConfiguredAsync());
        Assert.Empty(await client.SearchAsync("lynlås", 5));
        Assert.Empty(handler.Requests); // no API call is made at all
    }

    [Fact]
    public async Task Search_returns_nothing_when_youtube_answers_with_an_error()
    {
        var handler = new StubHandler().Enqueue(HttpStatusCode.BadRequest,
            """{ "error": { "code": 400, "message": "API key not valid" } }""");
        var client = CreateClient(handler, new FakeSettings(new()
        {
            [SettingKeys.YouTubeApiKey] = "UGYLDIG"
        }));

        Assert.Empty(await client.SearchAsync("lynlås", 5));
    }

    [Fact]
    public async Task Search_is_cached_between_calls()
    {
        var handler = new StubHandler().Enqueue(HttpStatusCode.OK, TwoVideos);
        var client = CreateClient(handler, new FakeSettings(new()
        {
            [SettingKeys.YouTubeApiKey] = "MIN-NOEGLE"
        }));

        var first = await client.SearchAsync("lynlås", 5);
        var second = await client.SearchAsync("lynlås", 5);

        Assert.Equal(2, first.Count);
        Assert.Equal(2, second.Count);
        Assert.Single(handler.Requests); // the second call came from the cache
    }
}
