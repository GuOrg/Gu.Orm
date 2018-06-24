namespace Gu.Orm.Npgsql.Analyzers.Parsing
{
    using System.Collections.Generic;
    using System.Collections.Immutable;

    public static partial class Parse
    {
        public static TargetList TargetList(string sql)
        {
            var tokens = Tokens(sql);
            var position = 0;
            return TargetList(sql, tokens, ref position);
        }

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

        public static SqlNode Literal(string sql)
        {
            var tokens = Tokens(sql);
            var position = 0;
            return Literal(sql, tokens, ref position);
        }

        private static TargetList TargetList(string sql, ImmutableArray<RawToken> tokens, ref int position)
        {
            var targets = new List<ResTarget>();
            while (ResTarget(sql, tokens, ref position) is ResTarget target)
            {
                targets.Add(target);
                if (TryMatch(tokens, position, SqlKind.Comma, out _))
                {
                    position++;
                }
                else
                {
                    break;
                }
            }

            return new TargetList(sql, targets.ToImmutableArray());
        }

        private static ResTarget ResTarget(string sql, ImmutableArray<RawToken> tokens, ref int position)
        {
            if (ColumnRef(sql, tokens, ref position) is ColumnRef columnRef)
            {
                if (TryAs(sql, tokens, ref position, out var @as, out var name))
                {
                    return new ResTarget(sql, columnRef, @as, name);
                }

                return new ResTarget(sql, columnRef, RawToken.None, null);
            }

            if (Literal(sql, tokens, ref position) is SqlLiteralExpression literal)
            {
                if (TryAs(sql, tokens, ref position, out var @as, out var name))
                {
                    return new ResTarget(sql, literal, @as, name);
                }

                return new ResTarget(sql, literal, RawToken.None, null);
            }

            return null;
        }

        private static bool TryAs(string sql, ImmutableArray<RawToken> tokens, ref int position, out RawToken @as, out SqlIdentifierName name)
        {
            name = null;
            if (TryMatchKeyword(sql, tokens, position, "AS", out @as))
            {
                position++;
                if (TryMatch(tokens, position, SqlKind.Identifier, out var nameToken))
                {
                    position++;
                    name = new SqlIdentifierName(sql, nameToken);
                }

                return true;
            }

            return false;
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

        private static SqlNode Literal(string sql, ImmutableArray<RawToken> tokens, ref int position)
        {
            if (TryMatch(tokens, position, SqlKind.Integer, out var token) ||
                TryMatch(tokens, position, SqlKind.Float, out token) ||
                TryMatch(tokens, position, SqlKind.String, out token))
            {
                position++;
                return new SqlLiteralExpression(sql, token);
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

        private static bool TryMatchKeyword(string sql, ImmutableArray<RawToken> tokens, int position, string keyWord, out RawToken token)
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
