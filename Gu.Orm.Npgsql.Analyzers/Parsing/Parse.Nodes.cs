namespace Gu.Orm.Npgsql.Analyzers.Parsing
{
    using System.Collections.Generic;
    using System.Collections.Immutable;

    /// <summary>
    /// https://www.postgresql.org/docs/current/static/sql-syntax-lexical.html
    /// </summary>
    public static partial class Parse
    {
        public static RangeVar RangeVar(string sql)
        {
            var tokens = Tokens(sql);
            var position = 0;
            return RangeVar(sql, tokens, ref position);
        }

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

        public static SqlLiteral Literal(string sql)
        {
            var tokens = Tokens(sql);
            var position = 0;
            return Literal(sql, tokens, ref position);
        }

        public static SqlInvocation Invocation(string sql)
        {
            var tokens = Tokens(sql);
            var position = 0;
            return Invocation(sql, tokens, ref position);
        }

        private static RangeVar RangeVar(string sql, ImmutableArray<RawToken> tokens, ref int position)
        {
            var start = position;
            if (Name(sql, tokens, ref position) is SqlNameSyntax name)
            {
                if (TryMatch(tokens, position, SqlKind.Identifier, out var next) &&
                    !TryMatchKeyword(sql, tokens, position, "WHERE", out _))
                {
                    position++;
                    return new RangeVar(sql, name, new SqlIdentifierName(sql, next));
                }

                return new RangeVar(sql, name, null);
            }

            position = start;
            return null;
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

            if (Literal(sql, tokens, ref position) is SqlLiteral literal)
            {
                if (TryAs(sql, tokens, ref position, out var @as, out var name))
                {
                    return new ResTarget(sql, literal, @as, name);
                }

                return new ResTarget(sql, literal, RawToken.None, null);
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

            var start = position;
            if (Name(sql, tokens, ref position) is SqlNameSyntax name &&
                !TryMatch(tokens, position, SqlKind.OpenParen, out _))
            {
                return new ColumnRef(sql, name);
            }

            position = start;
            return null;
        }

        private static SqlLiteral Literal(string sql, ImmutableArray<RawToken> tokens, ref int position)
        {
            if (TryMatch(tokens, position, SqlKind.Integer, out var token) ||
                TryMatch(tokens, position, SqlKind.Float, out token) ||
                TryMatch(tokens, position, SqlKind.String, out token))
            {
                position++;
                return new SqlLiteral(sql, token);
            }

            return null;
        }

        private static SqlInvocation Invocation(string sql, ImmutableArray<RawToken> tokens, ref int position)
        {
            var start = position;
            if (Name(sql, tokens, ref position) is SqlNameSyntax name &&
                TryMatch(tokens, position, SqlKind.OpenParen, out var openParen))
            {
                position++;
                List<SqlArgument> arguments = null;
                while (true)
                {
                    if (TryMatch(tokens, position, SqlKind.CloseParen, out var closeParen))
                    {
                        return new SqlInvocation(sql, name, openParen, arguments == null ? null : new SqlArgumentList(sql, arguments.ToImmutableArray()), closeParen);
                    }

                    if (Argument(sql, tokens, ref position) is SqlArgument argument)
                    {
                        if (arguments == null)
                        {
                            arguments = new List<SqlArgument>();
                        }

                        arguments.Add(argument);
                        if (TryMatch(tokens, position, SqlKind.Comma, out _))
                        {
                            position++;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }

            position = start;
            return null;
        }

        private static SqlArgument Argument(string sql, ImmutableArray<RawToken> tokens, ref int position)
        {
            var start = position;
            if (Expression(sql, tokens, ref position) is SqlExpression expression &&
                (TryMatch(tokens, position, SqlKind.CloseParen, out _) ||
                 TryMatch(tokens, position, SqlKind.Comma, out _)))
            {
                return new SqlArgument(sql, expression);
            }

            position = start;
            return null;
        }

        private static SqlExpression Expression(string sql, ImmutableArray<RawToken> tokens, ref int position)
        {
            if (Name(sql, tokens, ref position) is SqlNameSyntax name)
            {
                return name;
            }

            if (Literal(sql, tokens, ref position) is SqlLiteral literal)
            {
                return literal;
            }

            if (Invocation(sql, tokens, ref position) is SqlInvocation invocation)
            {
                return invocation;
            }

            return null;
        }

        private static SqlNameSyntax Name(string sql, ImmutableArray<RawToken> tokens, ref int position)
        {
            if (TryMatch(tokens, position, SqlKind.Identifier, out var token))
            {
                position++;
                if (TryMatch(tokens, position, SqlKind.Point, out var point))
                {
                    position++;
                    if (TryMatch(tokens, position, SqlKind.Identifier, out var name))
                    {
                        position++;
                        return new SqlQualifiedName(sql, new SqlIdentifierName(sql, token), point, new SqlIdentifierName(sql, name));
                    }

                    return new SqlQualifiedName(sql, new SqlIdentifierName(sql, token), point, null);
                }

                return new SqlIdentifierName(sql, token);
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
