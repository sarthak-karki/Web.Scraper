namespace Web.Scraping.App.Interfaces;

public interface IQueryHandler<TRequest, TResponse>
{
    Task<string> HandleAsync(TRequest request);
}
