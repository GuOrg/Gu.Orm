namespace Gu.Orm.Npgsql.Analyzers.Parsing
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Gu.Roslyn.AnalyzerExtensions;

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
                    TokenParser.TryParse(sql, position, "<>", SqlKind.NotEqualsToken, out token) ||
                    TokenParser.TryParse(sql, position, "<=", SqlKind.LessThanEqualsToken, out token) ||
                    TokenParser.TryParse(sql, position, ">=", SqlKind.GreaterThanEqualsToken, out token) ||
                    TokenParser.TryParse(sql, position, "|/", SqlKind.SquareRootToken, out token) ||
                    TokenParser.TryParse(sql, position, "||/", SqlKind.CubeRootToken, out token) ||
                    TokenParser.TryParse(sql, position, "!!", SqlKind.ExclamationExclamationToken, out token) ||
                    TokenParser.TryParse(sql, position, "<<", SqlKind.LessThanLessThanToken, out token) ||
                    TokenParser.TryParse(sql, position, ">>", SqlKind.GreaterThanGreaterThanToken, out token) ||
                    TokenParser.TryParse(sql, position, '+', SqlKind.PlusToken, out token) ||
                    TokenParser.TryParse(sql, position, '-', SqlKind.MinusToken, out token) ||
                    TokenParser.TryParse(sql, position, '*', SqlKind.AsteriskToken, out token) ||
                    TokenParser.TryParse(sql, position, '/', SqlKind.SlashToken, out token) ||
                    TokenParser.TryParse(sql, position, '^', SqlKind.ExponentToken, out token) ||
                    TokenParser.TryParse(sql, position, '%', SqlKind.PercentToken, out token) ||
                    TokenParser.TryParse(sql, position, '@', SqlKind.AtToken, out token) ||
                    TokenParser.TryParse(sql, position, '!', SqlKind.ExclamationToken, out token) ||
                    TokenParser.TryParse(sql, position, '&', SqlKind.AmpersandToken, out token) ||
                    TokenParser.TryParse(sql, position, '|', SqlKind.BarToken, out token) ||
                    TokenParser.TryParse(sql, position, '#', SqlKind.HashToken, out token) ||
                    TokenParser.TryParse(sql, position, '~', SqlKind.TildeToken, out token) ||
                    TokenParser.TryParse(sql, position, '=', SqlKind.EqualsToken, out token) ||
                    TokenParser.TryParse(sql, position, '<', SqlKind.LessThanEqualsToken, out token) ||
                    TokenParser.TryParse(sql, position, '>', SqlKind.GreaterThanToken, out token) ||
                    TokenParser.TryParse(sql, position, '(', SqlKind.OpenParenToken, out token) ||
                    TokenParser.TryParse(sql, position, ')', SqlKind.CloseParenToken, out token) ||
                    TokenParser.TryParse(sql, position, '[', SqlKind.OpenBracketToken, out token) ||
                    TokenParser.TryParse(sql, position, ']', SqlKind.CloseBracketToken, out token) ||
                    TokenParser.TryParse(sql, position, '.', SqlKind.DotToken, out token) ||
                    TokenParser.TryParse(sql, position, ',', SqlKind.CommaToken, out token) ||
                    TokenParser.TryParse(sql, position, ';', SqlKind.SemicolonToken, out token) ||
                    TokenParser.TryParse(sql, position, ':', SqlKind.ColonToken, out token))
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
                    while (sql.TryElementAt(position, out var next) &&
                           char.IsDigit(next))
                    {
                        position++;
                    }

                    if (sql.TryElementAt(position, out var c) &&
                        c == '.')
                    {
                        position++;
                        while (sql.TryElementAt(position, out var next) &&
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

                token = default;
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

                token = default;
                return false;
            }

            internal static bool TryParseIdentifier(string sql, int position, out RawToken token)
            {
                if (char.IsLetter(sql[position]))
                {
                    var start = position;
                    position++;
                    while (sql.TryElementAt(position, out var next) &&
                           (char.IsLetterOrDigit(next) || next == '_'))
                    {
                        position++;
                    }

                    token = new RawToken(SqlKind.Identifier, start, position);
                    return true;
                }

                if (sql.TryElementAt(position, out var c) &&
                    c == '"')
                {
                    var start = position;
                    position++;
                    token = SkipNext(sql, '"', ref position)
                        ? new RawToken(SqlKind.QuotedIdentifier, start, position)
                        : new RawToken(SqlKind.QuotedIdentifier, start, sql.Length);

                    return true;
                }

                token = default;
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

                token = default;
                return false;
            }

            internal static bool TryParse(string sql, int position, char expected, SqlKind kind, out RawToken token)
            {
                if (sql.TryElementAt(position, out var c) &&
                    c == expected)
                {
                    token = RawToken.SingleChar(kind, position);
                    return true;
                }

                token = default;
                return false;
            }

            internal static bool TryParse(string sql, int position, string expected, SqlKind kind, out RawToken token)
            {
                if (TryMatch(sql, position, expected))
                {
                    token = new RawToken(kind, position, position + expected.Length);
                    return true;
                }

                token = default;
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

            private static bool TryMatch(string sql, int position, string expected)
            {
                if (sql.TryElementAt(position, out var c) &&
                    c == expected[0])
                {
                    for (var i = 1; i < expected.Length; i++)
                    {
                        if (!sql.TryElementAt(position + i, out c) ||
                            expected[i] != c)
                        {
                            return false;
                        }
                    }

                    return true;
                }

                return false;
            }

            private static bool SkipNext(string sql, char end, ref int position)
            {
                while (sql.TryElementAt(position, out var next))
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
                        position += end.Length - 1;
                        return true;
                    }
                }

                return false;
            }
        }
    }
}
