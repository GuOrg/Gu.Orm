namespace Gu.Orm.Npgsql.Analyzers.Parsing
{
    public class SqlArgument : SqlNode
    {
        public SqlArgument(string sql, SqlExpression expression)
            : base(sql, CreateChildren(expression))
        {
            this.Expression = expression;
        }

        public SqlExpression Expression { get; }

        public override bool IsValid => this.Expression?.IsValid == true;

        public override string ToDisplayString() => this.Expression.ToDisplayString();
    }
}
