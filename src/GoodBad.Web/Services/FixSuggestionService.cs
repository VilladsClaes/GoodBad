using GoodBad.Web.Data;
using GoodBad.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace GoodBad.Web.Services;

public record FixSuggestion(
    string Title,
    string? Url,
    FixSourceKind Kind,
    string MatchedKeyword,
    bool IsRuleBased,
    string? Body = null);

/// <summary>
/// Looks for keywords in what the user wrote about a product and suggests real
/// YouTube videos and Reddit threads that may solve the problem.
///
/// The engine works in three steps:
///   1. Curated rules from the database (editable in /Admin/FixRegler).
///   2. Live searches via the YouTube Data API and Reddit's JSON search.
///   3. A plain search link as the last resort, so the feature always works –
///      also before an API key has been configured.
/// </summary>
public class FixSuggestionService
{
    private static readonly HashSet<string> StopWords = new(StringComparer.OrdinalIgnoreCase)
    {
        "og", "i", "jeg", "det", "at", "en", "den", "til", "er", "som", "på", "de", "med", "han",
        "af", "for", "ikke", "der", "var", "mig", "sig", "men", "et", "har", "om", "vi", "min",
        "havde", "ham", "hun", "nu", "over", "da", "fra", "du", "ud", "sin", "dem", "os", "op",
        "man", "hans", "hvor", "eller", "hvad", "skal", "selv", "her", "alle", "vil", "blev",
        "kunne", "ind", "når", "være", "dog", "noget", "ville", "jo", "deres", "efter", "ved",
        "skulle", "denne", "end", "dette", "så", "bare", "meget", "kan", "har", "godt", "dårligt",
        "dårlig", "god", "produkt", "produktet", "købte", "købt", "brugt", "bruge", "gang", "gange",
        "the", "and", "but", "with", "this", "that", "very", "was", "are", "for", "not"
    };

    private readonly AppDbContext _db;
    private readonly YouTubeSearchClient _youTube;
    private readonly RedditSearchClient _reddit;
    private readonly ILogger<FixSuggestionService> _logger;

    public FixSuggestionService(
        AppDbContext db,
        YouTubeSearchClient youTube,
        RedditSearchClient reddit,
        ILogger<FixSuggestionService> logger)
    {
        _db = db;
        _youTube = youTube;
        _reddit = reddit;
        _logger = logger;
    }

    public async Task<List<FixSuggestion>> SuggestAsync(string text, int max = 6, string? context = null, CancellationToken cancellationToken = default)
    {
        var results = new List<FixSuggestion>();
        if (string.IsNullOrWhiteSpace(text))
        {
            return results;
        }

        var seenUrls = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var lower = text.ToLowerInvariant();
        var matchedKeywords = new List<string>();

        // ---- 1. Curated keyword rules from the database --------------------
        var rules = await _db.FixSuggestionRules.AsNoTracking().ToListAsync(cancellationToken);
        foreach (var rule in rules)
        {
            var keywords = rule.Keywords.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var match = keywords.FirstOrDefault(k => lower.Contains(k.ToLowerInvariant()));
            if (match is null)
            {
                continue;
            }

            matchedKeywords.Add(match);
            if (results.Count >= max)
            {
                continue;
            }

            if (rule.Url is not null && !seenUrls.Add(rule.Url))
            {
                continue;
            }

            results.Add(new FixSuggestion(
                rule.Title,
                rule.Url,
                rule.Kind,
                match,
                true,
                $"Anbefalet løsning fra GoodBad-reglerne (nøgleord: \u201c{match}\u201d)."));
        }

        // ---- 2. Live searches (real YouTube videos and Reddit threads) -----
        var searchTerms = matchedKeywords
            .Concat(ExtractKeywords(text, 6))
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Take(2)
            .ToList();

        var youTubeConfigured = await _youTube.IsConfiguredAsync();
        var youTubeResults = 0;
        var redditResults = 0;

        // A category hint ("rust værktøj" instead of just "rust") makes the
        // keyword searches noticeably more relevant.
        var hint = string.IsNullOrWhiteSpace(context) ? string.Empty : " " + context.Trim();

        foreach (var term in searchTerms)
        {
            if (results.Count >= max)
            {
                break;
            }

            if (youTubeConfigured)
            {
                var videos = await _youTube.SearchAsync($"{term}{hint} reparation", 3, cancellationToken);
                foreach (var video in videos)
                {
                    if (results.Count >= max)
                    {
                        break;
                    }

                    if (!seenUrls.Add(video.Url))
                    {
                        continue;
                    }

                    results.Add(new FixSuggestion(
                        video.Title,
                        video.Url,
                        FixSourceKind.YouTube,
                        term,
                        false,
                        Describe("YouTube-video fundet på søgningen", term, video.Meta)));
                    youTubeResults++;
                }
            }

            if (results.Count >= max)
            {
                break;
            }

            var threads = await _reddit.SearchAsync($"{term}{hint}", 3, cancellationToken);
            foreach (var thread in threads)
            {
                if (results.Count >= max)
                {
                    break;
                }

                if (!seenUrls.Add(thread.Url))
                {
                    continue;
                }

                results.Add(new FixSuggestion(
                    thread.Title,
                    thread.Url,
                    FixSourceKind.Reddit,
                    term,
                    false,
                    Describe("Reddit-tråd fundet på søgningen", term, thread.Meta)));
                redditResults++;
            }
        }

        // ---- 3. Fallback: hand the user a search link ----------------------
        // Reached when no API key is configured, or when the live searches came
        // back empty (bad key, rate limit, network hiccup). A plain search link
        // always works, so the feature never looks broken.
        foreach (var keyword in ExtractKeywords(text, 5))
        {
            if (results.Count >= max)
            {
                break;
            }

            var youTubeUrl = $"https://www.youtube.com/results?search_query={Uri.EscapeDataString(keyword + " reparation")}";
            if (youTubeResults == 0 && seenUrls.Add(youTubeUrl))
            {
                results.Add(new FixSuggestion(
                    $"YouTube-søgning: hvordan løser man \u201c{keyword}\u201d?",
                    youTubeUrl,
                    FixSourceKind.YouTube,
                    keyword,
                    false,
                    youTubeConfigured
                        ? "YouTube-søgningen gav ingen resultater – prøv selv søgningen."
                        : "Tip: tilføj din YouTube Data API-nøgle under Admin → Indstillinger for at få rigtige videoer her."));
            }

            if (results.Count >= max)
            {
                break;
            }

            var redditUrl = $"https://www.reddit.com/search/?q={Uri.EscapeDataString(keyword)}";
            if (redditResults == 0 && seenUrls.Add(redditUrl))
            {
                results.Add(new FixSuggestion(
                    $"Reddit-søgning: erfaringer med \u201c{keyword}\u201d",
                    redditUrl,
                    FixSourceKind.Reddit,
                    keyword,
                    false,
                    $"Reddit-tråde hvor andre har haft samme problem med \u201c{keyword}\u201d."));
            }
        }

        return results.Take(max).ToList();
    }

    private static string Describe(string prefix, string term, string? meta)
        => string.IsNullOrWhiteSpace(meta)
            ? $"{prefix} \u201c{term}\u201d."
            : $"{prefix} \u201c{term}\u201d ({meta}).";

    /// <summary>Pulls the most relevant nouns/terms out of a free-text review.</summary>
    public static List<string> ExtractKeywords(string text, int max = 5)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return new List<string>();
        }

        var words = text
            .ToLowerInvariant()
            .Split(new[] { ' ', '.', ',', ';', ':', '!', '?', '\n', '\r', '\t', '(', ')', '"', '/', '-' },
                StringSplitOptions.RemoveEmptyEntries)
            .Select(w => w.Trim())
            .Where(w => w.Length >= 4 && !StopWords.Contains(w) && w.All(c => char.IsLetter(c) || char.IsDigit(c)))
            .ToList();

        return words
            .GroupBy(w => w)
            .OrderByDescending(g => g.Count())
            .ThenByDescending(g => g.Key.Length)
            .Select(g => g.Key)
            .Take(max)
            .ToList();
    }
}
