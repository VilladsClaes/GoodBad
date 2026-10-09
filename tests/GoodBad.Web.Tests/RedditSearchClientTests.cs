using System.Net;
using GoodBad.Web.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace GoodBad.Web.Tests;

/// <summary>
/// The Reddit client is verified against the documented JSON shapes: the public
/// search endpoint (data.children[].data) and the app-only OAuth flow
/// (access_token + oauth.reddit.com with a Bearer token).
/// </summary>
public class RedditSearchClientTests
{
    private const string SearchJson = """
    {
      "kind": "Listing",
      "data": {
        "children": [
          {
            "kind": "t3",
            "data": {
              "title": "How do you stop rust on hand tools?",
              "permalink": "/r/Tools/comments/abc/how_do_you_stop_rust/",
              "subreddit_name_prefixed": "r/Tools",
              "score": 128,
              "num_comments": 41
            }
          },
          {
            "kind": "t3",
            "data": {
              "title": "Adult content that must be filtered out",
              "permalink": "/r/nsfw/comments/xyz/adult/",
              "over_18": true
            }
          },
          {
            "kind": "t3",
            "data": {
              "title": "Keep your zipper alive",
              "permalink": "https://www.reddit.com/r/Frugal/comments/def/zipper/",
              "subreddit": "Frugal",
              "score": 12,
              "num_comments": 3
            }
          }
        ]
      }
    }
    """;

    private const string TokenJson = """
    { "access_token": "TOKEN123", "token_type": "bearer", "expires_in": 3600 }
    """;

    private static RedditSearchClient CreateClient(StubHandler handler, FakeSettings settings, IMemoryCache? cache = null)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Integrations:Reddit:UserAgent"] = "GoodBad/1.0 (test)",
                ["Integrations:Reddit:CacheMinutes"] = "60"
            })
            .Build();

        return new RedditSearchClient(
            new StubHttpClientFactory(handler),
            settings,
            configuration,
            cache ?? new MemoryCache(new MemoryCacheOptions()),
            NullLogger<RedditSearchClient>.Instance);
    }

    [Fact]
    public async Task Public_search_parses_threads_and_skips_nsfw()
    {
        var handler = new StubHandler().Enqueue(HttpStatusCode.OK, SearchJson);
        var client = CreateClient(handler, new FakeSettings());

        var results = await client.SearchAsync("rust", 5);

        Assert.Equal(2, results.Count);
        Assert.Equal("How do you stop rust on hand tools?", results[0].Title);
        Assert.Equal("https://www.reddit.com/r/Tools/comments/abc/how_do_you_stop_rust/", results[0].Url);
        Assert.Contains("r/Tools", results[0].Meta);
        Assert.Contains("128", results[0].Meta);
        Assert.Equal("Keep your zipper alive", results[1].Title);
        Assert.Equal("https://www.reddit.com/r/Frugal/comments/def/zipper/", results[1].Url);

        var request = handler.Requests.Single();
        Assert.StartsWith("https://www.reddit.com/search.json", request.RequestUri!.ToString());
        Assert.Equal("GoodBad/1.0 (test)", request.Headers.UserAgent.ToString());
    }

    [Fact]
    public async Task Oauth_search_requests_a_token_and_uses_the_oauth_endpoint()
    {
        var handler = new StubHandler()
            .Enqueue(HttpStatusCode.OK, TokenJson)
            .Enqueue(HttpStatusCode.OK, SearchJson);

        var client = CreateClient(handler, new FakeSettings(new()
        {
            [SettingKeys.RedditClientId] = "CLIENT-ID",
            [SettingKeys.RedditClientSecret] = "CLIENT-SECRET"
        }));

        var results = await client.SearchAsync("rust", 5);

        Assert.Equal(2, results.Count);

        var tokenRequest = handler.Requests[0];
        Assert.Equal("https://www.reddit.com/api/v1/access_token", tokenRequest.RequestUri!.ToString());
        Assert.Equal("Basic", tokenRequest.Headers.Authorization!.Scheme);
        Assert.Equal(
            Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("CLIENT-ID:CLIENT-SECRET")),
            tokenRequest.Headers.Authorization.Parameter);
        Assert.Contains("grant_type=client_credentials", handler.RequestBodies[0]);

        var searchRequest = handler.Requests[1];
        Assert.StartsWith("https://oauth.reddit.com/search", searchRequest.RequestUri!.ToString());
        Assert.Equal("Bearer", searchRequest.Headers.Authorization!.Scheme);
        Assert.Equal("TOKEN123", searchRequest.Headers.Authorization.Parameter);
    }

    [Fact]
    public async Task Search_returns_nothing_when_reddit_blocks_the_request()
    {
        // Reddit answers 403 for anonymous traffic from data centres, which is
        // why the OAuth mode above exists. The app must degrade gracefully.
        var handler = new StubHandler().Enqueue(HttpStatusCode.Forbidden, "<html>blocked</html>");
        var client = CreateClient(handler, new FakeSettings());

        Assert.Empty(await client.SearchAsync("rust", 5));
    }

    [Fact]
    public async Task Search_is_cached_between_calls()
    {
        var handler = new StubHandler().Enqueue(HttpStatusCode.OK, SearchJson);
        var client = CreateClient(handler, new FakeSettings());

        await client.SearchAsync("rust", 5);
        await client.SearchAsync("rust", 5);

        Assert.Single(handler.Requests);
    }
}
