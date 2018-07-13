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

        public static WhereClause WhereClause(string sql)
        {
            var tokens = Tokens(sql);
            var position = 0;
            return WhereClause(sql, tokens, ref position);
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

        private static WhereClause WhereClause(string sql, ImmutableArray<RawToken> tokens, ref int position)
        {
            var start = position;
            if (tokens.TryElementAt(position, out var candidate) &&
                TryMatchKeyword(sql, candidate, "WHERE"))
            {
                position++;
                if (BinaryExpression(sql, tokens, ref position) is SqlBinaryExpression rangeVar)
                {
                    return new WhereClause(sql, candidate.WithKind(SqlKind.WhereKeyword), rangeVar);
                }
            }

            position = start;
            return null;
        }
    }
}
