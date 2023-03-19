namespace NewsFeed.Api.Extensions;

public static class ExpressionExtensions
{
    public static Expression AsEqualExpression(
        this object value,
        string propName,
        ParameterExpression paramExpr)
    {
        var propExpr = Expression.Property(paramExpr, propName);
        var argExpr = Expression.Constant(value, value.GetType());

        return Expression.Equal(propExpr, argExpr);
    }

    public static Expression AsContainsExpression(
        this object value,
        string propName,
        ParameterExpression paramExpr)
    {
        var propExpr = Expression.Property(paramExpr, propName);
        var argExpr = Expression.Constant(value, value.GetType());

        var containsMethod = typeof(string).GetMethod(
            name: nameof(string.Contains),
            types: new[] { typeof(string), typeof(StringComparison) });

        return Expression.Call(propExpr, containsMethod!, argExpr);
    }
}
