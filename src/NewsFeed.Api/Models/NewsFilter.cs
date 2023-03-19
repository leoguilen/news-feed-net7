using NewsFeed.Api.Extensions;

namespace NewsFeed.Api.Models;

public readonly record struct NewsFilter(int PageNumber, int PageSize)
{
    public string? Headline { get; init; }

    public string? Category { get; init; }

    public string? Summary { get; init; }

    public string? Authors { get; init; }

    private Expression<Func<News, bool>>? SearchPredicate
    {
        get
        {
            var expressions = new List<Expression>(capacity: 4);
            var paramExpr = Expression.Parameter(typeof(News), "n");

            if (!string.IsNullOrWhiteSpace(Headline))
            {
                expressions.Add(Headline.AsContainsExpression(nameof(Headline), paramExpr));
            }

            if (!string.IsNullOrWhiteSpace(Category))
            {
                expressions.Add(Category.AsEqualExpression(nameof(Category), paramExpr));
            }

            if (!string.IsNullOrWhiteSpace(Summary))
            {
                expressions.Add(Summary.AsContainsExpression(nameof(Summary), paramExpr));
            }

            if (!string.IsNullOrWhiteSpace(Authors))
            {
                expressions.Add(Authors.AsContainsExpression(nameof(Authors), paramExpr));
            }

            if (expressions.Count is 0)
            {
                return null;
            }

            var completeExpr = expressions.Aggregate(Expression.AndAlso);
            return Expression.Lambda<Func<News, bool>>(completeExpr, paramExpr);
        }
    }

    public void Deconstruct(
        out Func<News, bool> predicate,
        out int pageNumber,
        out int pageSize)
    {
        predicate = SearchPredicate?.Compile() ?? new(_ => true);
        pageNumber = PageNumber;
        pageSize = PageSize;
    }
}