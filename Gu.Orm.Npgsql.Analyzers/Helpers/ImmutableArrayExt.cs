namespace Gu.Orm.Npgsql.Analyzers.Helpers
{
    using System.Collections.Immutable;

    internal static class ImmutableArrayExt
    {
        internal static bool TryElementAt<T>(this ImmutableArray<T> source, int index, out T item)
        {
            if (index >= 0 &&
                index < source.Length)
            {
                item = source[index];
                return true;
            }

            item = default;
            return false;
        }
    }
}
