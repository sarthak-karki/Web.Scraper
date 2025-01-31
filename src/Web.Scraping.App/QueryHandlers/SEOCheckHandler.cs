using Web.Scraping.Client;
using Web.Scraping.App.Interfaces;
using System.Text.RegularExpressions;

namespace Web.Scraping.App.QueryHandlers;

public partial class SEOCheckHandler : IQueryHandler<(string keywords, string targetUrl), string>
{
    private readonly IHttpClientService _client;
    private const int ResultsPerPage = 100;
    private const string GoogleSearchBaseUrl = "https://www.google.com.au/search";

    public SEOCheckHandler(IHttpClientService client)
    {
        _client = client;
    }

    public async Task<string> HandleAsync((string keywords, string targetUrl) request)
    {
        string encodedKeywords = System.Web.HttpUtility.UrlEncode(request.keywords);
        string searchUrl = $"{GoogleSearchBaseUrl}?num={ResultsPerPage}&q={encodedKeywords}";
        string? html = await _client.GetHtmlAsync(searchUrl);

        if (string.IsNullOrWhiteSpace(html))
        {
            return "0";
        }

        MatchCollection matches = CiteTagRegex().Matches(html);

        List<int> targetUrlPositions = 
            matches
            .Select((match, index) => new { Index = index, Content = match.Groups[2].Value })
            .Where(item => item.Content.Contains(request.targetUrl))
            .Select(item => item.Index + 1)
            .ToList();

        return
            targetUrlPositions.Count != 0
            ? string.Join(", ", targetUrlPositions)
            : "0";
    }

    private static Regex CiteTagRegex()
    {
        string pattern = @"<div class=""B6fmyf byrV5b Mg1HEd"">.*?<span class=""VuuXrf"">([^<]*)<\/span>.*?<cite\b[^>]*>(.*?)<\/cite>";

        return new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
    }
}
