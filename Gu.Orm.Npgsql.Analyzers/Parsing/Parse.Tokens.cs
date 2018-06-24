namespace Gu.Orm.Npgsql.Analyzers.Parsing
{
    using System.Collections.Generic;
    using System.Collections.Immutable;

    public static partial class Parse
    {
        public static ImmutableArray<RawToken> Tokens(string sql)
        {
            if (string.IsNullOrEmpty(sql))
            {
                return ImmutableArray<RawToken>.Empty;
            }

            var tokens = new List<RawToken>();
            var position = 0;
            while (position < sql.Length)
            {
                TokenParser.SkipWhitespace(sql, ref position);
                if (TokenParser.TryParseNumber(sql, position, out var token) ||
                    TokenParser.TryParseString(sql, position, out token) ||
                    TokenParser.TryParseComment(sql, position, out token) ||
                    TokenParser.TryParseIdentifier(sql, position, out token) ||
                    TokenParser.TryParse(sql, position, "<=", SqlKind.LessThanOrEqual, out token) ||
                    TokenParser.TryParse(sql, position, ">=", SqlKind.GreaterThanOrEqual, out token) ||
                    TokenParser.TryParse(sql, position, "|/", SqlKind.SquareRoot, out token) ||
                    TokenParser.TryParse(sql, position, "||/", SqlKind.CubeRoot, out token) ||
                    TokenParser.TryParse(sql, position, "!!", SqlKind.FactorialPrefix, out token) ||
                    TokenParser.TryParse(sql, position, "<<", SqlKind.BitwiseShiftLeft, out token) ||
                    TokenParser.TryParse(sql, position, ">>", SqlKind.BitwiseShiftRight, out token) ||
                    TokenParser.TryParse(sql, position, '+', SqlKind.Add, out token) ||
                    TokenParser.TryParse(sql, position, '-', SqlKind.Subtract, out token) ||
                    TokenParser.TryParse(sql, position, '*', SqlKind.Multiply, out token) ||
                    TokenParser.TryParse(sql, position, '/', SqlKind.Divide, out token) ||
                    TokenParser.TryParse(sql, position, '^', SqlKind.Exponent, out token) ||
                    TokenParser.TryParse(sql, position, '%', SqlKind.Modulo, out token) ||
                    TokenParser.TryParse(sql, position, '@', SqlKind.Abs, out token) ||
                    TokenParser.TryParse(sql, position, '!', SqlKind.Factorial, out token) ||
                    TokenParser.TryParse(sql, position, '&', SqlKind.BitwiseAnd, out token) ||
                    TokenParser.TryParse(sql, position, '|', SqlKind.BitwiseOr, out token) ||
                    TokenParser.TryParse(sql, position, '#', SqlKind.BitwiseXor, out token) ||
                    TokenParser.TryParse(sql, position, '~', SqlKind.BitwiseNot, out token) ||
                    TokenParser.TryParse(sql, position, '=', SqlKind.Equal, out token) ||
                    TokenParser.TryParse(sql, position, '<', SqlKind.LessThan, out token) ||
                    TokenParser.TryParse(sql, position, '>', SqlKind.GreaterThan, out token) ||
                    TokenParser.TryParse(sql, position, '(', SqlKind.OpenParen, out token) ||
                    TokenParser.TryParse(sql, position, ')', SqlKind.CloseParen, out token) ||
                    TokenParser.TryParse(sql, position, '[', SqlKind.OpenBracket, out token) ||
                    TokenParser.TryParse(sql, position, ']', SqlKind.CloseBracket, out token) ||
                    TokenParser.TryParse(sql, position, '.', SqlKind.Point, out token) ||
                    TokenParser.TryParse(sql, position, ',', SqlKind.Comma, out token) ||
                    TokenParser.TryParse(sql, position, ';', SqlKind.Semicolon, out token) ||
                    TokenParser.TryParse(sql, position, ':', SqlKind.Colon, out token))
                {
                    position = token.End;
                    tokens.Add(token);
                }
                else
                {
                    tokens.Add(RawToken.SingleChar(SqlKind.Unknown, position));
                    position++;
                }
            }

            return tokens.ToImmutableArray();
        }

        private static class TokenParser
        {
            internal static bool TryParseNumber(string sql, int position, out RawToken token)
            {
                if (char.IsDigit(sql[position]))
                {
                    var start = position;
                    position++;
                    while (TryPeek(sql, position, out var next) &&
                           char.IsDigit(next))
                    {
                        position++;
                    }

                    if (TryMatch(sql, position, '.'))
                    {
                        position++;
                        while (TryPeek(sql, position, out var next) &&
                               char.IsDigit(next))
                        {
                            position++;
                        }

                        token = new RawToken(SqlKind.Float, start, position);
                    }
                    else
                    {
                        token = new RawToken(SqlKind.Integer, start, position);
                    }

                    return true;
                }

                token = default(RawToken);
                return false;
            }

            internal static bool TryParseString(string sql, int position, out RawToken token)
            {
                if (sql[position] == '\'')
                {
                    var start = position;
                    position++;
                    if (SkipNext(sql, '\'', ref position))
                    {
                        token = new RawToken(SqlKind.String, start, position);
                    }
                    else
                    {
                        token = new RawToken(SqlKind.String, start, sql.Length);
                    }

                    return true;
                }

                token = default(RawToken);
                return false;
            }

            internal static bool TryParseIdentifier(string sql, int position, out RawToken token)
            {
                if (char.IsLetter(sql[position]))
                {
                    var start = position;
                    position++;
                    while (TryPeek(sql, position, out var next) &&
                           (char.IsLetterOrDigit(next) || next == '_'))
                    {
                        position++;
                    }

                    token = new RawToken(SqlKind.Identifier, start, position);
                    return true;
                }

                if (TryMatch(sql, position, '"'))
                {
                    var start = position;
                    position++;
                    token = SkipNext(sql, '"', ref position)
                        ? new RawToken(SqlKind.QuotedIdentifier, start, position)
                        : new RawToken(SqlKind.QuotedIdentifier, start, sql.Length);

                    return true;
                }

                token = default(RawToken);
                return false;
            }

            internal static bool TryParseComment(string sql, int position, out RawToken token)
            {
                if (TryMatch(sql, position, "--"))
                {
                    var start = position;
                    position += 2;
                    if (SkipNext(sql, '\n', ref position) ||
                        position == sql.Length)
                    {
                        var end = position;
                        if (sql[position - 2] == '\r')
                        {
                            end -= 2;
                        }
                        else if (sql[position - 1] == '\n')
                        {
                            end -= 1;
                        }

                        token = new RawToken(SqlKind.Comment, start, end);
                    }
                    else
                    {
                        token = new RawToken(SqlKind.Comment, start, sql.Length);
                    }

                    return true;
                }

                if (TryMatch(sql, position, "/*"))
                {
                    var start = position;
                    position += 2;
                    token = SkipNext(sql, "*/", ref position)
                        ? new RawToken(SqlKind.BlockComment, start, position)
                        : new RawToken(SqlKind.BlockComment, start, sql.Length);

                    return true;
                }

                token = default(RawToken);
                return false;
            }

            internal static bool TryParse(string sql, int position, char expected, SqlKind kind, out RawToken token)
            {
                if (TryMatch(sql, position, expected))
                {
                    token = RawToken.SingleChar(kind, position);
                    return true;
                }

                token = default(RawToken);
                return false;
            }

            internal static bool TryParse(string sql, int position, string expected, SqlKind kind, out RawToken token)
            {
                if (TryMatch(sql, position, expected))
                {
                    token = new RawToken(kind, position, position + expected.Length);
                    return true;
                }

                token = default(RawToken);
                return false;
            }

            internal static void SkipWhitespace(string sql, ref int position)
            {
                while (position < sql.Length &&
                       char.IsWhiteSpace(sql[position]))
                {
                    position++;
                }
            }

            private static bool TryMatch(string sql, int position, char expected)
            {
                return TryPeek(sql, position, out var c) &&
                       c == expected;
            }

            private static bool TryMatch(string sql, int position, string expected)
            {
                if (TryPeek(sql, position, out var c) &&
                    c == expected[0])
                {
                    for (var i = 1; i < expected.Length; i++)
                    {
                        if (!TryPeek(sql, position + i, out c) ||
                            expected[i] != c)
                        {
                            return false;
                        }
                    }

                    return true;
                }

                return false;
            }

            private static bool TryPeek(string sql, int position, out char c)
            {
                if (position < sql.Length)
                {
                    c = sql[position];
                    return true;
                }

                c = default(char);
                return false;
            }

            private static bool SkipNext(string sql, char end, ref int position)
            {
                while (TryPeek(sql, position, out var next))
                {
                    position++;
                    if (next == end)
                    {
                        return true;
                    }
                }

                return false;
            }

            private static bool SkipNext(string sql, string end, ref int position)
            {
                while (SkipNext(sql, end[0], ref position))
                {
                    if (TryMatch(sql, position - 1, end))
                    {
                        return true;
                    }
                }

                return false;
            }
        }
    }
}
