namespace NewsFeed.Api.Data.Repositories;

public interface INewsRepository
{
    IAsyncEnumerable<News> ListAllAsync(NewsFilter filter, CancellationToken cancellationToken = default);

    Task<News?> GetByIdAsync(string newsId, CancellationToken cancellationToken = default);
}
