using System.Net;
using System.Text;
using GoodBad.Web.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;

namespace GoodBad.Web.Tests;

/// <summary>Captures outgoing requests and replays canned responses.</summary>
public sealed class StubHandler : HttpMessageHandler
{
    private readonly Queue<Func<HttpRequestMessage, HttpResponseMessage>> _responses = new();

    public List<HttpRequestMessage> Requests { get; } = new();
    public List<string> RequestBodies { get; } = new();

    public StubHandler Enqueue(HttpStatusCode status, string json)
        => Enqueue(_ => new HttpResponseMessage(status)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        });

    public StubHandler Enqueue(Func<HttpRequestMessage, HttpResponseMessage> factory)
    {
        _responses.Enqueue(factory);
        return this;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Requests.Add(request);
        RequestBodies.Add(request.Content is null
            ? string.Empty
            : await request.Content.ReadAsStringAsync(cancellationToken));

        if (_responses.Count == 0)
        {
            return new HttpResponseMessage(HttpStatusCode.NotFound);
        }

        return _responses.Dequeue()(request);
    }
}

public sealed class StubHttpClientFactory : IHttpClientFactory
{
    private readonly StubHandler _handler;

    public StubHttpClientFactory(StubHandler handler) => _handler = handler;

    public HttpClient CreateClient(string name) => new(_handler, disposeHandler: false);
}

/// <summary>In-memory settings, standing in for /Admin/Settings (the database).</summary>
public sealed class FakeSettings : ISiteSettings
{
    private readonly Dictionary<string, string?> _values;

    public FakeSettings(Dictionary<string, string?>? values = null)
        => _values = values ?? new Dictionary<string, string?>();

    public Task<Dictionary<string, string?>> AllAsync() => Task.FromResult(_values);

    public Task<string?> GetAsync(string key)
        => Task.FromResult(_values.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value) ? value : null);
}

public sealed class FakeWebHostEnvironment : IWebHostEnvironment
{
    public FakeWebHostEnvironment(string webRoot, string? contentRoot = null)
    {
        WebRootPath = webRoot;
        ContentRootPath = contentRoot ?? webRoot;
    }

    public string ApplicationName { get; set; } = "GoodBad.Web.Tests";
    public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
    public string WebRootPath { get; set; }
    public string EnvironmentName { get; set; } = "Development";
    public string ContentRootPath { get; set; }
    public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
}
