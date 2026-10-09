using GoodBad.Web.Data;
using GoodBad.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace GoodBad.Web.Services;

public record FixSuggestion(string Title, string? Url, FixSourceKind Kind, string MatchedKeyword, bool IsRuleBased);

/// <summary>
/// Looks for keywords in what the user wrote about a product and suggests
/// YouTube tutorials / Reddit threads that may solve the problem. Uses a
/// curated rule table plus a keyword-extraction fallback that builds search
/// links, so the feature works without any external API keys.
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
        "skulle", "denne", "end", "dette", "jeg", "så", "bare", "meget", "kan", "den", "har",
        "the", "and", "but", "with", "this", "that", "very", "was", "are", "for", "not"
    };

    private readonly AppDbContext _db;

    public FixSuggestionService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<FixSuggestion>> SuggestAsync(string text, int max = 4)
    {
        var results = new List<FixSuggestion>();
        if (string.IsNullOrWhiteSpace(text))
        {
            return results;
        }

        var lower = text.ToLowerInvariant();
        var rules = await _db.FixSuggestionRules.AsNoTracking().ToListAsync();

        foreach (var rule in rules)
        {
            var keywords = rule.Keywords
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var match = keywords.FirstOrDefault(k => lower.Contains(k.ToLowerInvariant()));
            if (match is not null)
            {
                results.Add(new FixSuggestion(rule.Title, rule.Url, rule.Kind, match, true));
            }
        }

        if (results.Count >= max)
        {
            return results.Take(max).ToList();
        }

        // Fallback: build search links from the most meaningful words the user wrote.
        var keywordsFound = ExtractKeywords(text, 5);
        foreach (var keyword in keywordsFound)
        {
            if (results.Count >= max)
            {
                break;
            }

            if (results.Any(r => r.MatchedKeyword.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }

            var query = Uri.EscapeDataString(keyword + " reparation");
            results.Add(new FixSuggestion(
                $"YouTube-søgning: hvordan løser man \"{keyword}\"?",
                $"https://www.youtube.com/results?search_query={query}",
                FixSourceKind.YouTube,
                keyword,
                false));

            if (results.Count >= max)
            {
                break;
            }

            results.Add(new FixSuggestion(
                $"Reddit-søgning: erfaringer med \"{keyword}\"",
                $"https://www.reddit.com/search/?q={Uri.EscapeDataString(keyword)}",
                FixSourceKind.Reddit,
                keyword,
                false));
        }

        return results.Take(max).ToList();
    }

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
