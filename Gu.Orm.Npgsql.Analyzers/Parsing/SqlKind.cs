namespace Gu.Orm.Npgsql.Analyzers.Parsing
{
    public enum SqlKind
    {
        Unknown,
        Identifier,
        Comma,
        Point,
        Semicolon,
        Star,
        OpenParens,
        CloseParens,
        OpenBracket,
        CloseBracket,
        Comment,
        Equal,
        NotEqual,
        LessThan,
        LessThanOrEqual,
        GreaterThan,
        GreaterThanOrEqual,
        None
    }
}