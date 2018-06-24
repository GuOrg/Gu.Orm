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
                case SqlKind.FactorialPrefix:
                case SqlKind.Abs:
                case SqlKind.Comma:
                case SqlKind.Point:
                case SqlKind.Semicolon:
                case SqlKind.Colon:
                case SqlKind.OpenParen:
                case SqlKind.CloseParen:
                case SqlKind.OpenBracket:
                case SqlKind.CloseBracket:
                case SqlKind.Comment:
                case SqlKind.BlockComment:
                case SqlKind.None:
                    return false;
                case SqlKind.Add:
                case SqlKind.Subtract:
                case SqlKind.Multiply:
                case SqlKind.Divide:
                case SqlKind.Exponent:
                case SqlKind.Modulo:
                case SqlKind.SquareRoot:
                case SqlKind.CubeRoot:
                case SqlKind.Factorial:
                case SqlKind.BitwiseAnd:
                case SqlKind.BitwiseOr:
                case SqlKind.BitwiseXor:
                case SqlKind.BitwiseNot:
                case SqlKind.BitwiseShiftLeft:
                case SqlKind.BitwiseShiftRight:
                case SqlKind.Equal:
                case SqlKind.NotEqual:
                case SqlKind.LessThan:
                case SqlKind.LessThanOrEqual:
                case SqlKind.GreaterThan:
                case SqlKind.GreaterThanOrEqual:
                    return true;
                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }
        }
    }
}
