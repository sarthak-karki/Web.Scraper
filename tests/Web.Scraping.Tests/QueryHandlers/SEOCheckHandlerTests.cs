using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Web.Scraping.App.QueryHandlers;
using Web.Scraping.Client;
using Xunit;

namespace Web.Scraping.Tests;

public static class TestData
{
    public static string GetTestHtmlContent()
    {
        return
            @"
                <div class=""B6fmyf byrV5b Mg1HEd"">
                    <span class=""VuuXrf"">Some Text</span>
                    <cite class=""qLRx3b tjvcx GvPZzd cHaqb"" role=""text"">https://www.smokeball.com.au<span class=""ylgVCe ob9lvb"" role=""text""> › subpage</span></cite>
                </div>
                <div class=""B6fmyf byrV5b Mg1HEd"">
                    <span class=""VuuXrf"">Another Text</span>
                    <cite class=""qLRx3b tjvcx GvPZzd cHaqb"" role=""text"">https://www.anotherexample.com<span class=""ylgVCe ob9lvb"" role=""text""> › other-page</span></cite>
                </div>
                <div class=""B6fmyf byrV5b Mg1HEd"">
                    <span class=""VuuXrf"">Another Text</span>
                    <cite class=""qLRx3b tjvcx GvPZzd cHaqb"" role=""text"">https://www.smokeball.com.au<span class=""ylgVCe ob9lvb"" role=""text""> › other-page</span></cite>
                </div>
                <div class=""B6fmyf byrV5b Mg1HEd"">
                    <span class=""VuuXrf"">Another Text</span>
                    <cite class=""qLRx3b tjvcx GvPZzd cHaqb"" role=""text"">https://www.smokeball.com.au<span class=""ylgVCe ob9lvb"" role=""text""> › other-page</span></cite>
                </div>
                <div class=""B6fmyf byrV5b Mg1HEd"">
                    <span class=""VuuXrf"">Another Text</span>
                    <cite class=""qLRx3b tjvcx GvPZzd cHaqb"" role=""text"">https://www.someotherexample.com.au<span class=""ylgVCe ob9lvb"" role=""text""> › other-page</span></cite>
                </div>
            ";
    }
}

public class SEOCheckHandlerTests
{
    private readonly IHttpClientService _httpClient;

    public SEOCheckHandlerTests()
    {
        _httpClient = Substitute.For<IHttpClientService>();
    }

    [Fact]
    public async Task HandleAsync_Should_ReturnPositions_When_TargetUrlIsFound()
    {
        var handler = new SEOCheckHandler(_httpClient);
        string keywords = "conveyancing software";
        string targetUrl = "https://www.smokeball.com.au";

        string htmlContent = TestData.GetTestHtmlContent();

        _httpClient.GetHtmlAsync(Arg.Any<string>())!.Returns(Task.FromResult(htmlContent));

        var result = await handler.HandleAsync((keywords, targetUrl));

        result.Should().Be("1, 3, 4");
    }

    [Fact]
    public async Task HandleAsync_Should_ReturnZero_When_TargetUrlIsNotFound()
    {
        var handler = new SEOCheckHandler(_httpClient);
        string keywords = "conveyancing software";
        string targetUrl = "https://www.smokeball.com.au";
        string htmlContent =
            @"
                    <html>
                    <body>
                        <cite class='qLRx3b tjvcx GvPZzd cHaqb' role='text'>https://www.anotherwebsite.com<span class='ylgVCe ob9lvb' role='text'> › another-page</span></cite>
                    </body>
                    </html>";

        _httpClient.GetHtmlAsync(Arg.Any<string>())!.Returns(Task.FromResult(htmlContent));

        var result = await handler.HandleAsync((keywords, targetUrl));

        result.Should().Be("0");
    }

    [Fact]
    public async Task HandleAsync_Should_HandleEmptyHtmlResponse()
    {
        var handler = new SEOCheckHandler(_httpClient);
        string keywords = "conveyancing software";
        string targetUrl = "https://www.smokeball.com.au";

        _httpClient.GetHtmlAsync(Arg.Any<string>()).Returns(Task.FromResult<string?>(null));

        var result = await handler.HandleAsync((keywords, targetUrl));

        result.Should().Be("0");
    }

    [Fact]
    public async Task HandleAsync_Should_HandleEmptyResults()
    {
        var handler = new SEOCheckHandler(_httpClient);
        string keywords = "conveyancing software";
        string targetUrl = "https://www.smokeball.com.au";

        string emptyHtml = "<html><body></body></html>";
        _httpClient.GetHtmlAsync(Arg.Any<string>())!.Returns(Task.FromResult(emptyHtml));

        var result = await handler.HandleAsync((keywords, targetUrl));

        result.Should().Be("0");
    }
}
