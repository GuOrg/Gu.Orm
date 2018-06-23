namespace Gu.Orm.Npgsql.Analyzers.Parsing
{
    using System.Collections.Immutable;

    public class ColumnRef : SqlNode
    {
        public ColumnRef(SqlExpression expression)
            : base(ImmutableArray.Create<SqlNode>(expression))
        {
            this.Expression = expression;
        }

        public SqlExpression Expression { get; }

        public override string ToString() => this.Expression?.ToString() ?? "<missing>";
    }
}
