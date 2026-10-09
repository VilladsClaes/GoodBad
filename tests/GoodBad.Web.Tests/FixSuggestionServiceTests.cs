using GoodBad.Web.Services;
using Xunit;

namespace GoodBad.Web.Tests;

public class FixSuggestionServiceTests
{
    [Fact]
    public void ExtractKeywords_drops_stopwords_short_words_and_duplicates()
    {
        var keywords = FixSuggestionService.ExtractKeywords(
            "Plastikken i spændet flækkede. Plastikken er af lav kvalitet, og lynlåsen ryger.");

        Assert.Contains("plastikken", keywords);
        Assert.Contains("lynlåsen", keywords);
        Assert.DoesNotContain("i", keywords);
        Assert.DoesNotContain("og", keywords);
        Assert.DoesNotContain("af", keywords);
        Assert.Equal(keywords.Distinct().Count(), keywords.Count);
    }

    [Fact]
    public void ExtractKeywords_handles_empty_text()
    {
        Assert.Empty(FixSuggestionService.ExtractKeywords("   "));
    }
}
