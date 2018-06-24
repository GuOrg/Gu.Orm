namespace Gu.Orm.Npgsql.Analyzers.Parsing
{
    public class SqlParenthesizedExpression : SqlExpression
    {
        public SqlParenthesizedExpression(string sql, RawToken openParen, SqlExpression expression, RawToken closeParen)
            : base(sql, CreateChildren(expression))
        {
            this.OpenParen = openParen.WithParent(this);
            this.Expression = expression;
            this.CloseParen = closeParen.WithParent(this);
        }

        public SqlToken OpenParen { get; }

        public SqlExpression Expression { get; }

        public SqlToken CloseParen { get; }

        public override bool IsValid => this.OpenParen.Kind == SqlKind.OpenParen &&
                                        this.Expression?.IsValid == true &&
                                        this.CloseParen.Kind == SqlKind.CloseParen;

        public override string ToDisplayString() => $"{this.OpenParen.ToDisplayString(this.Sql)}{this.Expression.ToDisplayString()}{this.CloseParen.ToDisplayString(this.Sql)}";
    }
}
