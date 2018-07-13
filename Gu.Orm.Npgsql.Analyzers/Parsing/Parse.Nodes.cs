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

        public static SqlPrefixUnaryExpression PrefixUnaryExpression(string sql)
        {
            var tokens = Tokens(sql);
            var position = 0;
            return PrefixUnaryExpression(sql, tokens, ref position);
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
                if (TryMatch(tokens, position, SqlKind.CommaToken, out _))
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
            if (TryMatch(tokens, position, SqlKind.AsteriskToken, out var token))
            {
                position++;
                return new ColumnRef(sql, new Star(sql, token));
            }

            var start = position;
            if (Name(sql, tokens, ref position) is SqlName name &&
                !TryMatch(tokens, position, SqlKind.OpenParenToken, out _))
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
                tokens.TryElementAt(position, out var candidate) &&
                TryBinaryOperator(out var binaryOperator))
            {
                position++;
                var right = Expression(sql, tokens, ref position);
                if (right is SqlBinaryExpression binary &&
                    Precedence(binaryOperator.Kind) < Precedence(binary.Operator.Kind))
                {
                    return new SqlBinaryExpression(
                        sql,
                        new SqlBinaryExpression(sql, left, binaryOperator, binary.Left),
                        new RawToken(binary.Operator.Kind, binary.Operator.Start, binary.Operator.End),
                        binary.Right);
                }

                return new SqlBinaryExpression(sql, left, binaryOperator, right);
            }

            position = start;
            return null;

            bool TryBinaryOperator(out RawToken result)
            {
                switch (candidate.Kind)
                {
                    case SqlKind.PlusToken:
                    case SqlKind.MinusToken:
                    case SqlKind.AsteriskToken:
                    case SqlKind.SlashToken:
                    case SqlKind.ExponentToken:
                    case SqlKind.PercentToken:
                    case SqlKind.SquareRootToken:
                    case SqlKind.CubeRootToken:
                    case SqlKind.ExclamationToken:
                    case SqlKind.AmpersandToken:
                    case SqlKind.BarToken:
                    case SqlKind.HashToken:
                    case SqlKind.TildeToken:
                    case SqlKind.LessThanLessThanToken:
                    case SqlKind.GreaterThanGreaterThanToken:
                    case SqlKind.EqualsToken:
                    case SqlKind.NotEqualsToken:
                    case SqlKind.LessThanToken:
                    case SqlKind.LessThanEqualsToken:
                    case SqlKind.GreaterThanToken:
                    case SqlKind.GreaterThanEqualsToken:
                    case SqlKind.AndKeyword:
                    case SqlKind.OrKeyword:
                        result = candidate;
                        return true;
                    case SqlKind.Identifier:
                        if (TryMatchKeyword(sql, candidate, "AND"))
                        {
                            result = candidate.WithKind(SqlKind.AndKeyword);
                            return true;
                        }

                        if (TryMatchKeyword(sql, candidate, "OR"))
                        {
                            result = candidate.WithKind(SqlKind.OrKeyword);
                            return true;
                        }

                        break;
                }

                result = default;
                return false;
            }

            int Precedence(SqlKind kind)
            {
                // https://www.postgresql.org/docs/current/static/sql-syntax-lexical.html#SQL-PRECEDENCE
                switch (kind)
                {
                    case SqlKind.ExponentToken:
                        return 0;
                    case SqlKind.AsteriskToken:
                    case SqlKind.SlashToken:
                    case SqlKind.PercentToken:
                        return 1;
                    case SqlKind.PlusToken:
                    case SqlKind.MinusToken:
                        return 2;
                    case SqlKind.ExclamationToken:
                    case SqlKind.AmpersandToken:
                    case SqlKind.BarToken:
                    case SqlKind.HashToken:
                    case SqlKind.TildeToken:
                    case SqlKind.LessThanLessThanToken:
                    case SqlKind.GreaterThanGreaterThanToken:
                        return 4;
                    case SqlKind.LessThanToken:
                    case SqlKind.LessThanEqualsToken:
                    case SqlKind.GreaterThanToken:
                    case SqlKind.GreaterThanEqualsToken:
                    case SqlKind.EqualsToken:
                    case SqlKind.NotEqualsToken:
                        return 5;
                    case SqlKind.NotKeyword:
                        return 6;
                    case SqlKind.AndKeyword:
                        return 7;
                    case SqlKind.OrKeyword:
                        return 8;
                    default:
                        return 3;
                }
            }
        }

        private static SqlPrefixUnaryExpression PrefixUnaryExpression(string sql, ImmutableArray<RawToken> tokens, ref int position)
        {
            var start = position;
            if (tokens.TryElementAt(position, out var candidate) &&
                TryUnaryOperator(out var token))
            {
                position++;
                if (Expression(sql, tokens, ref position) is SqlExpression operand)
                {
                    return new SqlPrefixUnaryExpression(sql, token, operand);
                }
            }

            position = start;
            return null;

            bool TryUnaryOperator(out RawToken result)
            {
                switch (candidate.Kind)
                {
                    case SqlKind.PlusToken:
                    case SqlKind.MinusToken:
                    case SqlKind.TildeToken:
                    case SqlKind.ExclamationExclamationToken:
                    case SqlKind.AtToken:
                        result = candidate;
                        return true;
                    case SqlKind.Identifier:
                        if (TryMatchKeyword(sql, candidate, "NOT"))
                        {
                            result = candidate.WithKind(SqlKind.NotKeyword);
                            return true;
                        }

                        break;
                }

                result = default;
                return false;
            }

        }

        private static SqlParenthesizedExpression ParenthesizedExpression(string sql, ImmutableArray<RawToken> tokens, ref int position)
        {
            var start = position;
            if (TryMatch(tokens, position, SqlKind.OpenParenToken, out var open))
            {
                position++;
                if (Expression(sql, tokens, ref position) is SqlExpression expression)
                {
                    if (TryMatch(tokens, position, SqlKind.CloseParenToken, out var close))
                    {
                        position++;
                        return new SqlParenthesizedExpression(sql, open, expression, close);
                    }

                    return new SqlParenthesizedExpression(sql, open, expression, RawToken.None);
                }
                else
                {
                    if (TryMatch(tokens, position, SqlKind.CloseParenToken, out var close))
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
                TryMatch(tokens, position, SqlKind.OpenParenToken, out var openParen))
            {
                position++;
                List<SqlArgument> arguments = null;
                while (true)
                {
                    if (TryMatch(tokens, position, SqlKind.CloseParenToken, out var closeParen))
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
                        if (TryMatch(tokens, position, SqlKind.CommaToken, out _))
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
                (TryMatch(tokens, position, SqlKind.CloseParenToken, out _) ||
                 TryMatch(tokens, position, SqlKind.CommaToken, out _)))
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
                   (SqlExpression)PrefixUnaryExpression(sql, tokens, ref position) ??
                   (SqlExpression)Literal(sql, tokens, ref position) ??
                   (SqlExpression)Name(sql, tokens, ref position);
            //// ReSharper restore RedundantCast
        }

        private static SqlName Name(string sql, ImmutableArray<RawToken> tokens, ref int position)
        {
            if (SqlSimpleName(sql, tokens, ref position, allowKeyword: true) is SqlSimpleName simpleName)
            {
                if (TryMatch(tokens, position, SqlKind.DotToken, out var point))
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
                return new QuotedIdentifier(sql, token);
            }

            return null;
        }

        private static bool TryAs(string sql, ImmutableArray<RawToken> tokens, ref int position, out RawToken @as, out SqlSimpleName name)
        {
            name = null;
            if (TryMatchKeyword(sql, tokens, position, "AS", out @as))
            {
                position++;
                if (SqlSimpleName(sql, tokens, ref position, allowKeyword: true) is SqlSimpleName simpleName)
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

            token = default;
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

            token = default;
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
