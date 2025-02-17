namespace Infrastructure.ExtensionService
{
    public static class OrderByHelper
    {
        public static IOrderedEnumerable<TEntity> OrderBy<TEntity, TKey>(this IQueryable<TEntity> source,
                                                                         Func<TEntity, TKey> keySelector)
        {
            return source.OrderBy(keySelector);
        }

        public static IOrderedEnumerable<TEntity> OrderByDescending<TEntity, TKey>(this IQueryable<TEntity> source,
                                                                         Func<TEntity, TKey> keySelector)
        {
            return source.OrderByDescending(keySelector);
        }
        public static IOrderedEnumerable<TEntity> ThenBy<TEntity, TKey>(this IQueryable<TEntity> source,
                                                                         Func<TEntity, TKey> keySelector)
        {
            return source.ThenBy(keySelector);
        }

        public static IOrderedEnumerable<TEntity> ThenByDescending<TEntity, TKey>(this IQueryable<TEntity> source,
                                                                         Func<TEntity, TKey> keySelector)
        {
            return source.ThenBy(keySelector);
        }
    }
}
