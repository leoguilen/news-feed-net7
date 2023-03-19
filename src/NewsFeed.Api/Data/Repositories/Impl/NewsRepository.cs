namespace NewsFeed.Api.Data.Repositories.Impl;

internal sealed class NewsRepository : INewsRepository
{
    private static readonly DataLoaderOptions s_loaderOptions = new(4096);

    private readonly IDataLoader<News> _dataLoader;

    public NewsRepository(IDataLoader<News> dataLoader)
        => _dataLoader = dataLoader;

    public async Task<News?> GetByIdAsync(string newsId, CancellationToken cancellationToken = default)
    {
        var internalCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var internalCtsToken = internalCts.Token;

        await foreach (var newsItem in _dataLoader.LoadAsync(s_loaderOptions, internalCtsToken))
        {
            if (newsItem.Id.Equals(newsId))
            {
                internalCts.Cancel();
                return newsItem;
            }
        }

        internalCts.Cancel();
        return null;
    }

    public IAsyncEnumerable<News> ListAllAsync(NewsFilter filter, CancellationToken cancellationToken = default)
    {
        var internalCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var internalCtsToken = internalCts.Token;

        var (predicate, pageNumber, pageSize) = filter;

        return _dataLoader
            .LoadAsync(s_loaderOptions, internalCtsToken)
            .Where(predicate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .OrderByDescending(n => n.PublishDate);
    }
}
