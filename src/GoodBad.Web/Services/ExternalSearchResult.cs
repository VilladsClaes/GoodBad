namespace GoodBad.Web.Services;

/// <summary>A real YouTube video or Reddit thread found by the fix engine.</summary>
public record ExternalSearchResult(
    string Title,
    string Url,
    string? Snippet,
    string Source,
    string? Meta);
