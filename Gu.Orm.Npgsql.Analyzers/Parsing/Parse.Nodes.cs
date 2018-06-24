namespace Gu.Orm.Npgsql.Analyzers.Parsing
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Gu.Orm.Npgsql.Analyzers.Helpers;

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

        public static SqlBinaryExpression BinaryExpression(string sql)
        {
            var tokens = Tokens(sql);
            var position = 0;
            return BinaryExpression(sql, tokens, ref position);
        }

        public static SqlParenthesizedExpression ParenthesizedExpression(string sql)
        {
            var tokens = Tokens(sql);
            var position = 0;
            return ParenthesizedExpression(sql, tokens, ref position);
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
            if (Name(sql, tokens, ref position) is SqlName name)
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

            if (Invocation(sql, tokens, ref position) is SqlInvocation invocation)
            {
                if (TryAs(sql, tokens, ref position, out var @as, out var name))
                {
                    return new ResTarget(sql, invocation, @as, name);
                }

                return new ResTarget(sql, invocation, RawToken.None, null);
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
            if (Name(sql, tokens, ref position) is SqlName name &&
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

        private static SqlBinaryExpression BinaryExpression(string sql, ImmutableArray<RawToken> tokens, ref int position)
        {
            var start = position;
            //// ReSharper disable RedundantCast
            var left = (SqlExpression)Invocation(sql, tokens, ref position) ??
                       (SqlExpression)ParenthesizedExpression(sql, tokens, ref position) ??
                       (SqlExpression)Literal(sql, tokens, ref position) ??
                       (SqlExpression)Name(sql, tokens, ref position);
            //// ReSharper restore RedundantCast
            if (left != null &&
                tokens.TryElementAt(position, out var op) &&
                IsBinaryOperator(op))
            {
                position++;
                var right = Expression(sql, tokens, ref position);
                return new SqlBinaryExpression(sql, left, op, right);
            }

            position = start;
            return null;

            bool IsBinaryOperator(RawToken token)
            {
                if (token.Kind.IsBinaryOperator())
                {
                    return true;
                }

                if (token.Kind is SqlKind.Identifier)
                {
                    return TryMatchKeyword(sql, token, "AND") ||
                           TryMatchKeyword(sql, token, "OR");
                }

                return false;
            }
        }

        private static SqlParenthesizedExpression ParenthesizedExpression(string sql, ImmutableArray<RawToken> tokens, ref int position)
        {
            var start = position;
            if (TryMatch(tokens, position, SqlKind.OpenParen, out var open))
            {
                position++;
                if (Expression(sql, tokens, ref position) is SqlExpression expression)
                {
                    if (TryMatch(tokens, position, SqlKind.CloseParen, out var close))
                    {
                        position++;
                        return new SqlParenthesizedExpression(sql, open, expression, close);
                    }

                    return new SqlParenthesizedExpression(sql, open, expression, RawToken.None);
                }
                else
                {
                    if (TryMatch(tokens, position, SqlKind.CloseParen, out var close))
                    {
                        position++;
                        return new SqlParenthesizedExpression(sql, open, null, close);
                    }
                }
            }

            position = start;
            return null;
        }

        private static SqlInvocation Invocation(string sql, ImmutableArray<RawToken> tokens, ref int position)
        {
            var start = position;
            if (Name(sql, tokens, ref position) is SqlName name &&
                TryMatch(tokens, position, SqlKind.OpenParen, out var openParen))
            {
                position++;
                List<SqlArgument> arguments = null;
                while (true)
                {
                    if (TryMatch(tokens, position, SqlKind.CloseParen, out var closeParen))
                    {
                        position++;
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
                        return new SqlInvocation(sql, name, openParen, arguments == null ? null : new SqlArgumentList(sql, arguments.ToImmutableArray()), closeParen);
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
            //// ReSharper disable RedundantCast for symmetry
            return (SqlExpression)Invocation(sql, tokens, ref position) ??
                   (SqlExpression)ParenthesizedExpression(sql, tokens, ref position) ??
                   (SqlExpression)BinaryExpression(sql, tokens, ref position) ??
                   (SqlExpression)Literal(sql, tokens, ref position) ??
                   (SqlExpression)Name(sql, tokens, ref position);
            //// ReSharper restore RedundantCast
        }

        private static SqlName Name(string sql, ImmutableArray<RawToken> tokens, ref int position)
        {
            if (SqlSimpleName(sql, tokens, ref position, allowKeyword: true) is SqlSimpleName simpleName)
            {
                if (TryMatch(tokens, position, SqlKind.Point, out var point))
                {
                    position++;
                    if (SqlSimpleName(sql, tokens, ref position, allowKeyword: true) is SqlSimpleName name)
                    {
                        return new SqlQualifiedName(sql, simpleName, point, name);
                    }

                    return new SqlQualifiedName(sql, simpleName, point, null);
                }

                return simpleName;
            }

            return null;
        }

        private static SqlSimpleName SqlSimpleName(string sql, ImmutableArray<RawToken> tokens, ref int position, bool allowKeyword)
        {
            if (TryMatch(tokens, position, SqlKind.Identifier, out var token))
            {
                if (!allowKeyword &&
                    IsReservedKeyword(sql, token))
                {
                    return null;
                }

                position++;
                return new SqlIdentifierName(sql, token);
            }

            if (TryMatch(tokens, position, SqlKind.QuotedIdentifier, out token))
            {
                position++;
                return new QuotedIdentifer(sql, token);
            }

            return null;
        }

        private static bool TryAs(string sql, ImmutableArray<RawToken> tokens, ref int position, out RawToken @as, out SqlSimpleName name)
        {
            name = null;
            if (TryMatchKeyword(sql, tokens, position, "AS", out @as))
            {
                position++;
                if (SqlSimpleName(sql, tokens, ref position, true) is SqlSimpleName simpleName)
                {
                    name = simpleName;
                }

                return true;
            }

            return false;
        }

        private static bool TryMatch(ImmutableArray<RawToken> tokens, int position, SqlKind kind, out RawToken token)
        {
            if (tokens.TryElementAt(position, out token) &&
                token.Kind == kind)
            {
                return true;
            }

            token = default(RawToken);
            return false;
        }

        private static bool TryMatchKeyword(string sql, RawToken token, string keyword)
        {
            if (token.Length == keyword.Length)
            {
                for (int i = 0; i < token.Length; i++)
                {
                    if (char.ToUpper(sql[token.Start + i]) != keyword[i])
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        private static bool TryMatchKeyword(string sql, ImmutableArray<RawToken> tokens, int position, string keyword, out RawToken token)
        {
            if (TryMatch(tokens, position, SqlKind.Identifier, out token) &&
                TryMatchKeyword(sql, token, keyword))
            {
                return true;
            }

            token = default(RawToken);
            return false;
        }

        private static bool IsReservedKeyword(string sql, RawToken token)
        {
            foreach (var keyword in Keywords.Reserved)
            {
                if (TryMatchKeyword(sql, token, keyword))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
