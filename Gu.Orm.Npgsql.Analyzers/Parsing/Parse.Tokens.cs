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
                SkipWhitespace(sql, ref position);
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
                        if (TryPeek(sql, position + 1, out var next))
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
                        if (TryPeek(sql, position + 1, out next))
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
                    default:
                        if (IsIdentifier(sql[position]))
                        {
                            var start = position;
                            SkipIdentifier(sql, ref position);
                            tokens.Add(new SqlToken(SqlKind.Identifier, start, position));
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

        private static void SkipWhitespace(string sql, ref int position)
        {
            while (position < sql.Length &&
                  char.IsWhiteSpace(sql[position]))
            {
                position++;
            }
        }

        private static void SkipIdentifier(string sql, ref int position)
        {
            while (position < sql.Length && IsIdentifier(sql[position]))
            {
                position++;
            }
        }

        private static bool IsIdentifier(char c) => char.IsLetterOrDigit(c) || c == '_';
    }
}