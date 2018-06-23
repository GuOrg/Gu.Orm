namespace Gu.Orm.Npgsql.Analyzers.Parsing
{
    using System.Collections.Immutable;

    public static partial class Parse
    {
        public static ResTarget ResTarget(string sql)
        {
            var tokens = Tokens(sql);
            var position = 0;
            return ResTarget(sql, tokens, ref position);
        }

        public static ColumnRef ColumnRef(string sql)
        {
            var tokens = Tokens(sql);
            var position = 0;
            return ColumnRef(sql, tokens, ref position);
        }

        private static ResTarget ResTarget(string sql, ImmutableArray<RawToken> tokens, ref int position)
        {
            if (ColumnRef(sql, tokens, ref position) is ColumnRef columnRef)
            {
                if (TryMatchKeyword(tokens, position, sql, "AS", out var @as))
                {
                    position++;
                    if (TryMatch(tokens, position, SqlKind.Identifier, out var name))
                    {
                        position++;
                        return new ResTarget(sql, columnRef, @as, new SqlIdentifierName(sql, name));
                    }

                    return null;
                }

                return new ResTarget(sql, columnRef, RawToken.None, null);
            }

            return null;
        }

        private static ColumnRef ColumnRef(string sql, ImmutableArray<RawToken> tokens, ref int position)
        {
            if (TryMatch(tokens, position, SqlKind.Multiply, out var token))
            {
                position++;
                return new ColumnRef(sql, new Star(sql, token));
            }

            if (TryMatch(tokens, position, SqlKind.Identifier, out token))
            {
                position++;
                if (TryMatch(tokens, position, SqlKind.Point, out var point))
                {
                    position++;
                    if (TryMatch(tokens, position, SqlKind.Identifier, out var name))
                    {
                        position++;
                        return new ColumnRef(sql, new SqlQualifiedName(sql, new SqlIdentifierName(sql, token), point, new SqlIdentifierName(sql, name)));
                    }

                    return null;
                }

                return new ColumnRef(sql, new SqlIdentifierName(sql, token));
            }

            return null;
        }

        private static bool TryMatch(ImmutableArray<RawToken> tokens, int position, SqlKind kind, out RawToken token)
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

        private static bool TryMatchKeyword(ImmutableArray<RawToken> tokens, int position, string sql, string keyWord, out RawToken token)
        {
            if (TryMatch(tokens, position, SqlKind.Identifier, out token) &&
                token.Length == keyWord.Length)
            {
                for (int i = 0; i < token.Length; i++)
                {
                    if (char.ToUpper(sql[token.Start + i]) != keyWord[i])
                    {
                        token = default(RawToken);
                        return false;
                    }
                }

                return true;
            }

            token = default(RawToken);
            return false;
        }
    }
}
