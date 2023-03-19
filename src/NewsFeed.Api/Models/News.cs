namespace NewsFeed.Api.Models;

public record News
{
    public string Id
    {
        get
        {
            var lastSegment = Link.Segments[^1];
            return lastSegment[(lastSegment.LastIndexOf('_') + 1)..];
        }
    }

    public string Headline { get; init; } = null!;

    public string Category { get; init; } = null!;

    [JsonPropertyName("short_description")]
    public string Summary { get; init; } = null!;

    public string Authors { get; init; } = null!;

    [JsonPropertyName("date")]
    public DateOnly PublishDate { get; init; }

    public Uri Link { get; init; } = null!;
}
