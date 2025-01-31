using Microsoft.Extensions.DependencyInjection;
using Web.Scraping.App.Interfaces;
using Web.Scraping.App.QueryHandlers;
using Web.Scraping.Client;

namespace Web.Scraping.App;

class Program
{
    static async Task Main(string[] args)
    {
        try
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            string keywords = <<KEYWORDs_HERE>>;
            string targetUrl = <<TARGET_URL_HERE>>;

            var seoCheckHandler = serviceProvider.GetService<IQueryHandler<(string keywords, string targetUrl), string>>();

            if (seoCheckHandler == null)
            {
                Console.WriteLine("Error: Unable to retrieve the SEO check handler.");
                return;
            }

            string result = await seoCheckHandler.HandleAsync((keywords, targetUrl));
            Console.WriteLine(result);
        }
        catch (ClientException ex)
        {
            Console.WriteLine($"Client error: Unable to fetch data: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error occurred: {ex.Message}");
        }
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddHttpClient<IHttpClientService, HttpClientService>(client =>
        {
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
        });

        services.AddSingleton<IQueryHandler<(string keywords, string targetUrl), string>, SEOCheckHandler>();
    }

}
