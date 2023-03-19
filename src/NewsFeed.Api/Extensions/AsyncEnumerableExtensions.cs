//using System.Text.RegularExpressions;

//namespace NewsFeed.Api.Extensions;

//public static partial class AsyncEnumerableExtensions
//{
//    public static IAsyncEnumerable<T> OrderByExpression<T>(
//        this IAsyncEnumerable<T> source,
//        string orderByExpression)
//    {
//        if (!TryGetExpressionByString(orderByExpression, out var expr))
//        {
//            return news;
//        }

//        var x = Expression.Call(default, "",default);

//        source.OrderBy(_ => "");

//        return source.OrderBy(expr.Compile());
//    }

//    private static bool TryGetExpressionByString(string rawValue, out LambdaExpression? expression)
//    {
//        var matchResult = ExpressionMatcher().Match(rawValue);
//        if (!matchResult.Success)
//        {
//            expression = null;
//            return false;
//        }

//        expression = default;
//        return true;
//    }

//    [GeneratedRegex("^([a-zA-Z]+) (asc|desc)$", RegexOptions.Compiled | RegexOptions.IgnoreCase)]
//    private static partial Regex ExpressionMatcher();
//}

////public static class IAsyncEnumerableExtensions
////{
////    public static async IAsyncEnumerable<T> OrderByAsync<T>(this IAsyncEnumerable<T> source, string orderBy)
////    {
////        if (string.IsNullOrWhiteSpace(orderBy))
////        {
////            // Se não houver nenhuma ordem especificada, retorna a sequência sem alterações
////            await foreach (var item in source)
////            {
////                yield return item;
////            }
////            yield break;
////        }

////        var orderByClauses = orderBy.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
////            .Select(o => o.Trim().Split(' ', 2))
////            .Select(s => new { PropertyName = s[0], Direction = s.Length > 1 && s[1].ToLower() == "desc" ? SortDirection.Descending : SortDirection.Ascending })
////            .ToList();

////        if (orderByClauses.Count == 0)
////        {
////            // Se nenhuma ordem válida for encontrada, retorna a sequência sem alterações
////            await foreach (var item in source)
////            {
////                yield return item;
////            }
////            yield break;
////        }

////        var parameter = Expression.Parameter(typeof(T), "x");
////        var propertyAccess = Expression.PropertyOrField(parameter, orderByClauses[0].PropertyName);
////        var orderByExpression = Expression.Lambda(propertyAccess, parameter);

////        for (int i = 1; i < orderByClauses.Count; i++)
////        {
////            var clause = orderByClauses[i];
////            parameter = Expression.Parameter(typeof(T), "x");
////            propertyAccess = Expression.PropertyOrField(parameter, clause.PropertyName);
////            var thenByExpression = Expression.Lambda(propertyAccess, parameter);
////            orderByExpression = Expression.Call(typeof(Queryable), clause.Direction == SortDirection.Ascending ? "ThenBy" : "ThenByDescending",
////                new Type[] { typeof(T), propertyAccess.Type }, orderByExpression, thenByExpression);
////        }

////        var orderedQuery = source.ToAsyncEnumerable().OrderBy(orderByExpression.Compile());

////        await foreach (var item in orderedQuery)
////        {
////            yield return item;
////        }
////    }

////    enum SortDirection
////    {
////        Ascending,
////        Descending
////    }
////}
