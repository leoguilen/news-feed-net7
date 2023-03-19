var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IDataLoader<News>, JsonDataLoader>();
builder.Services.AddScoped<INewsRepository, NewsRepository>();
builder.Services.AddRateLimiter(options =>
{
    options.OnRejected = (ctx, _) =>
    {
        if (ctx.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
        {
            ctx.HttpContext.Response.Headers.RetryAfter =
                ((int)retryAfter.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo);
        }

        ctx.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

        return new ValueTask();
    };

    options.AddFixedWindowLimiter(policyName: "fixed", options =>
    {
        options.PermitLimit = 4;
        options.Window = TimeSpan.FromSeconds(12);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 0;
    });
});
builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(policy => policy
        .SetCacheKeyPrefix($"{Environment.MachineName}-")
        .SetVaryByHost(enabled: true));
    options.AddPolicy("CacheById", policy
        => policy.SetVaryByRouteValue(new[] { "newsId" }));
    options.AddPolicy("CacheByFilterArgs", policy
        => policy.SetVaryByQuery(new[] { "headline", "category", "summary", "authors", "pageNumber", "pageSize" }));
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRateLimiter();
app.UseOutputCache();

var v1 = app.MapGroup("api/v1").RequireRateLimiting(policyName: "fixed");

v1.MapGet("news", (
    [FromQuery] string? headline,
    [FromQuery] string? category,
    [FromQuery] string? summary,
    [FromQuery] string? authors,
    [FromQuery] int? pageNumber,
    [FromQuery] int? pageSize,
    [FromServices] INewsRepository newsRepository,
    CancellationToken requestCancellationToken)
    => Results.Ok(newsRepository.ListAllAsync(
        filter: new(pageNumber ?? 1, pageSize ?? 20)
        {
            Headline = headline,
            Category = category,
            Summary = summary,
            Authors = authors,
        },
        requestCancellationToken)))
.CacheOutput("CacheByFilterArgs");

v1.MapGet("news/{newsId}", async (
    [FromRoute] string newsId,
    [FromServices] INewsRepository newsRepository,
    CancellationToken requestCancellationToken)
    => await newsRepository.GetByIdAsync(newsId, requestCancellationToken) switch
    {
        null => Results.NotFound(),
        var news => Results.Ok(news),
    })
.CacheOutput("CacheById");

await app
    .RunAsync()
    .ConfigureAwait(false);