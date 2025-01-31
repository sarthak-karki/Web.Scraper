using Microsoft.Extensions.Logging;

namespace Web.Scraping.Client
{
    public interface IHttpClientService
    {
        Task<string?> GetHtmlAsync(string url);
    }

    public class HttpClientService : IHttpClientService
    {
        private readonly HttpClient _client;
        private readonly ILogger _logger;

        public HttpClientService(HttpClient client, ILogger<HttpClientService> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<string?> GetHtmlAsync(string url)
        {
            try
            {
                var response = await _client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    _logger.LogWarning("Unable to fetch the HTML content.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unknown error: {ex.Message}");
                throw new ClientException("An error occurred while fetching the HTML content.", ex);
            }
        }
    }
}
