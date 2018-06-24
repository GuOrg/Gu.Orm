namespace Gu.Orm.Npgsql.Analyzers.Parsing
{
    public class ColumnRef : SqlExpression
    {
        public ColumnRef(string sql, SqlExpression expression)
            : base(sql, CreateChildren(expression))
        {
            this.Expression = expression;
        }

        public SqlExpression Expression { get; }

        public override bool IsValid => this.Expression?.IsValid != false;

        public override string ToDisplayString() => this.Expression?.ToDisplayString() ?? "<missing>";
    }
}
