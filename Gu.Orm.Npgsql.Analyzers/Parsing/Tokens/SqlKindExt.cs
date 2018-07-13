namespace Gu.Orm.Npgsql.Analyzers.Parsing
{
    using System;

    public static class SqlKindExt
    {
        public static bool IsBinaryOperator(this SqlKind kind)
        {
            switch (kind)
            {
                case SqlKind.Unknown:
                case SqlKind.Identifier:
                case SqlKind.QuotedIdentifier:
                case SqlKind.String:
                case SqlKind.Integer:
                case SqlKind.Float:
                case SqlKind.ExclamationExclamationToken:
                case SqlKind.AtToken:
                case SqlKind.CommaToken:
                case SqlKind.DotToken:
                case SqlKind.SemicolonToken:
                case SqlKind.ColonToken:
                case SqlKind.OpenParenToken:
                case SqlKind.CloseParenToken:
                case SqlKind.OpenBracketToken:
                case SqlKind.CloseBracketToken:
                case SqlKind.SingleLineCommentTrivia:
                case SqlKind.MultiLineCommentTrivia:
                case SqlKind.NotKeyword:
                case SqlKind.None:
                    return false;
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
                    return true;
                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }
        }

        /// <summary>
        /// https://www.postgresql.org/docs/current/static/sql-syntax-lexical.html#SQL-PRECEDENCE
        /// </summary>
        /// <param name="kind"></param>
        /// <returns></returns>
        public static int Precedence(this SqlKind kind)
        {
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
}
