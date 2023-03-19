namespace NewsFeed.Api.Data.DataLoaders;

internal interface IDataLoader<TModel>
{
    IAsyncEnumerable<TModel> LoadAsync(
        DataLoaderOptions options,
        CancellationToken cancellationToken = default);
}
