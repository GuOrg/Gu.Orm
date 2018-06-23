namespace Gu.Orm.Npgsql.Analyzers.Parsing
{
    using System.Collections.Immutable;

    public static partial class Parse
    {
        public static ColumnRef ColumnRef(string sql)
        {
            var tokens = Tokens(sql);
            var position = 0;
            return ColumnRef(tokens, ref position);
        }

        private static ColumnRef ColumnRef(ImmutableArray<RawToken> tokens, ref int position)
        {
            if (TryMatch(tokens, position, SqlKind.Multiply, out var token))
            {
                position++;
                return new ColumnRef(new Star(token));
            }

            if (TryMatch(tokens, position, SqlKind.Identifier, out token))
            {
                position++;
                if (TryMatch(tokens, position, SqlKind.Point, out var point))
                {
                    if (TryMatch(tokens, position, SqlKind.Identifier, out var name))
                    {
                        return new ColumnRef(new SqlQualifiedName(new SqlIdentifierName(token), point, new SqlIdentifierName(name)));
                    }

                    return null;
                }

                return new ColumnRef(new SqlIdentifierName(token));
            }

            return null;
        }

        private static bool TryMatch(this ImmutableArray<RawToken> tokens, int position, SqlKind kind, out RawToken token)
        {
            if (position < tokens.Length)
            {
                token = tokens[position];
                if (token.Kind == kind)
                {
                    return true;
                }
            }

            token = default(RawToken);
            return false;
        }
    }
}
