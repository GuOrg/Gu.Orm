namespace Gu.Orm.Npgsql.Analyzers.Parsing
{
    using System.Collections.Immutable;

    public class ColumnRef : SqlNode
    {
        public ColumnRef(string sql, SqlExpression expression)
            : base(sql, ImmutableArray.Create<SqlNode>(expression))
        {
            this.Expression = expression;
        }

        public SqlExpression Expression { get; }

        public override string ToString() => this.Expression?.ToString() ?? "<missing>";
    }
}
