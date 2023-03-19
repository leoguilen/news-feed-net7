namespace NewsFeed.Api.Data.DataLoaders;

internal readonly record struct DataLoaderOptions
{
    public DataLoaderOptions(int bufferSize)
        => BufferSize = bufferSize;

    public int BufferSize { get; init; } = 4096;
}