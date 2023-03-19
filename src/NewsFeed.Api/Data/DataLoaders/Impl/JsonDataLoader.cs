namespace NewsFeed.Api.Data.DataLoaders.Impl;

internal sealed class JsonDataLoader : IDataLoader<News>
{
    private const string ResourcePath = "NewsFeed.Api.Resources.NewsDataset.json";
    private const byte StartArrayTokenByte = 91;

    private static readonly Assembly s_currentAssembly = Assembly.GetExecutingAssembly();
    private static readonly JsonSerializerOptions s_serializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private byte[] _remainingBytes = Array.Empty<byte>();

    public async IAsyncEnumerable<News> LoadAsync(
        DataLoaderOptions options,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var resourceStream = s_currentAssembly.GetManifestResourceStream(ResourcePath);
        while (true)
        {
            var buffer = ArrayPool<byte>.Shared.Rent(options.BufferSize);

            Array.Copy(_remainingBytes, 0, buffer, 0, _remainingBytes.Length);

            var numberOfReadedBytes = await resourceStream!
                .ReadAsync(buffer.AsMemory(_remainingBytes.Length, buffer.Length - _remainingBytes.Length), cancellationToken);
            if (numberOfReadedBytes <= 0)
            {
                break;
            }

            var isFinalBlock = (numberOfReadedBytes + _remainingBytes.Length) < options.BufferSize;
            foreach (var newsItem in ReadNewsFromBufferedBytes(ref buffer, isFinalBlock))
            {
                yield return newsItem;
            }

            ArrayPool<byte>.Shared.Return(buffer, clearArray: true);
        }
    }

    private IEnumerable<News> ReadNewsFromBufferedBytes(ref byte[] buffer, bool isFinalBlock)
    {
        var listOfNews = ArrayPool<News>.Shared.Rent(minimumLength: 10);

        var jsonReader = new Utf8JsonReader(buffer, isFinalBlock, new(new() { AllowTrailingCommas = true }));
        var listOfNewsIndex = 0;
        while (jsonReader.Read() && jsonReader.TokenType is not JsonTokenType.EndArray)
        {
            if (jsonReader.TokenType is JsonTokenType.StartArray) continue;

            var currentTokenIndex = (int)jsonReader.TokenStartIndex;
            if (jsonReader.TrySkip())
            {
                var lastTokenIndex = (int)jsonReader.TokenStartIndex + 1;

                listOfNews[listOfNewsIndex] = Deserialize<News>(
                    utf8Json: buffer.AsSpan()[currentTokenIndex..lastTokenIndex],
                    options: s_serializerOptions)!;

                listOfNewsIndex++;
                continue;
            }

            var remainingBytes = buffer[(int)jsonReader.TokenStartIndex..]
                .Prepend(StartArrayTokenByte)
                .ToArray();

            Array.Resize(ref _remainingBytes, remainingBytes.Length);
            Array.Clear(_remainingBytes);
            remainingBytes.CopyTo(_remainingBytes, 0);
            break;
        }

        var targetIndex = Array.IndexOf(listOfNews, null) - 1;
        var readedNews = listOfNews[0..targetIndex];

        ArrayPool<News>.Shared.Return(listOfNews, clearArray: true);
        return readedNews;
    }
}
