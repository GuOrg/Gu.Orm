namespace Gu.Orm.Npgsql.Analyzers.Parsing
{
    using System;
    using System.Collections.Generic;

    public static partial class Parse
    {
        public static IReadOnlyList<SqlToken> Tokens(string sql)
        {
            if (string.IsNullOrEmpty(sql))
            {
                return Array.Empty<SqlToken>();
            }

            var tokens = new List<SqlToken>();
            var position = 0;
            while (position < sql.Length)
            {
                TokenParser.SkipWhitespace(sql, ref position);
                switch (sql[position])
                {
                    case '(':
                        tokens.Add(SqlToken.SingleChar(SqlKind.OpenParens, position));
                        position++;
                        continue;
                    case ')':
                        tokens.Add(SqlToken.SingleChar(SqlKind.CloseParens, position));
                        position++;
                        continue;
                    case '[':
                        tokens.Add(SqlToken.SingleChar(SqlKind.OpenBracket, position));
                        position++;
                        continue;
                    case ']':
                        tokens.Add(SqlToken.SingleChar(SqlKind.CloseBracket, position));
                        position++;
                        continue;
                    case '.':
                        tokens.Add(SqlToken.SingleChar(SqlKind.Point, position));
                        position++;
                        continue;
                    case ',':
                        tokens.Add(SqlToken.SingleChar(SqlKind.Comma, position));
                        position++;
                        continue;
                    case '*':
                        tokens.Add(SqlToken.SingleChar(SqlKind.Star, position));
                        position++;
                        continue;
                    case ';':
                        tokens.Add(SqlToken.SingleChar(SqlKind.Semicolon, position));
                        position++;
                        continue;
                    case '=':
                        tokens.Add(SqlToken.SingleChar(SqlKind.Equal, position));
                        position++;
                        continue;
                    case '<':
                        if (TokenParser.TryPeekNext(sql, position + 1, out var next))
                        {
                            switch (next)
                            {
                                case '=':
                                    tokens.Add(new SqlToken(SqlKind.LessThanOrEqual, position, position + 2));
                                    position += 2;
                                    continue;
                                case '>':
                                    tokens.Add(new SqlToken(SqlKind.NotEqual, position, position + 2));
                                    position += 2;
                                    continue;
                            }
                        }

                        tokens.Add(SqlToken.SingleChar(SqlKind.LessThan, position));
                        position++;
                        continue;

                    case '>':
                        if (TokenParser.TryPeekNext(sql, position + 1, out next))
                        {
                            switch (next)
                            {
                                case '=':
                                    tokens.Add(new SqlToken(SqlKind.GreaterThanOrEqual, position, position + 2));
                                    position += 2;
                                    continue;
                            }
                        }

                        tokens.Add(SqlToken.SingleChar(SqlKind.GreaterThan, position));
                        position++;
                        continue;
                    case '\'':
                        {
                            var start = position;
                            position++;
                            if (TokenParser.SkipTo(sql, '\'', ref position))
                            {
                                tokens.Add(new SqlToken(SqlKind.String, start, position));
                            }

                            continue;
                        }

                    case '"':
                        {
                            var start = position;
                            position++;
                            if (TokenParser.SkipTo(sql, '"', ref position))
                            {
                                tokens.Add(new SqlToken(SqlKind.Identifier, start, position));
                            }

                            continue;
                        }

                    case '-' when TokenParser.TryPeekNext(sql, position, out next) &&
                                  next == '-':
                        {
                            var start = position;
                            position++;
                            if (TokenParser.SkipTo(sql, '\n', ref position) ||
                                position == sql.Length)
                            {
                                tokens.Add(new SqlToken(SqlKind.Comment, start, position));
                            }

                            continue;
                        }
                    default:
                        if (char.IsLetter(sql[position]))
                        {
                            var start = position;
                            TokenParser.SkipIdentifier(sql, ref position);
                            tokens.Add(new SqlToken(SqlKind.Identifier, start, position));
                        }
                        else if (char.IsDigit(sql[position]))
                        {
                            var start = position;
                            while (TokenParser.TryPeekNext(sql, position, out next) &&
                                   char.IsDigit(next))
                            {
                                position++;
                            }

                            if (TokenParser.TryPeekNext(sql, position, out next) &&
                                next == '.')
                            {
                                position++;
                                while (TokenParser.TryPeekNext(sql, position, out next) &&
                                       char.IsDigit(next))
                                {
                                    position++;
                                }

                                tokens.Add(new SqlToken(SqlKind.Float, start, position));
                            }
                            else
                            {
                                tokens.Add(new SqlToken(SqlKind.Integer, start, position));
                            }
                        }
                        else
                        {
                            tokens.Add(SqlToken.SingleChar(SqlKind.Unknown, position));
                            position++;
                        }

                        continue;
                }
            }

            return tokens;
        }

        private static class TokenParser
        {
            public static bool TryPeekNext(string sql, int position, out char c)
            {
                if (position < sql.Length)
                {
                    c = sql[position];
                    return true;
                }

                c = default(char);
                return false;
            }

            public static void SkipWhitespace(string sql, ref int position)
            {
                while (position < sql.Length &&
                       Char.IsWhiteSpace(sql[position]))
                {
                    position++;
                }
            }

            public static void SkipIdentifier(string sql, ref int position)
            {
                while (position < sql.Length && IsIdentifier(sql[position]))
                {
                    position++;
                }
            }

            private static bool IsIdentifier(char c) => Char.IsLetterOrDigit(c) || c == '_';

            public static bool SkipTo(string sql, char end, ref int position)
            {
                while (TryPeekNext(sql, position, out var next))
                {
                    position++;
                    if (next == end)
                    {
                        return true;
                    }
                }

                return false;
            }
        }
    }
}