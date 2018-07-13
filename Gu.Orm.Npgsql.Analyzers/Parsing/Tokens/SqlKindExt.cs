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
                case SqlKind.Comment:
                case SqlKind.BlockComment:
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
                case SqlKind.NotEqualToken:
                case SqlKind.LessThanToken:
                case SqlKind.LessThanEqualsToken:
                case SqlKind.GreaterThanToken:
                case SqlKind.GreaterThanEqualsToken:
                    return true;
                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }
        }
    }
}
