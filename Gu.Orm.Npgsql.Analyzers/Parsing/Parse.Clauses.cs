namespace Gu.Orm.Npgsql.Analyzers.Parsing
{
    using System.Collections.Immutable;
    using Gu.Orm.Npgsql.Analyzers.Helpers;
    using Gu.Orm.Npgsql.Analyzers.Parsing.Clauses;

    public static partial class Parse
    {
        public static FromClause FromClause(string sql)
        {
            var tokens = Tokens(sql);
            var position = 0;
            return FromClause(sql, tokens, ref position);
        }

        private static FromClause FromClause(string sql, ImmutableArray<RawToken> tokens, ref int position)
        {
            var start = position;
            if (tokens.TryElementAt(position, out var candidate) &&
                TryMatchKeyword(sql, candidate, "FROM"))
            {
                position++;
                if (RangeVar(sql, tokens, ref position) is RangeVar rangeVar)
                {
                    return new FromClause(sql, candidate.WithKind(SqlKind.FromKeyword), rangeVar);
                }
            }

            position = start;
            return null;
        }
    }
}
