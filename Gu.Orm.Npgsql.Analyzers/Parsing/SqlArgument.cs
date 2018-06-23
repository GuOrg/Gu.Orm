namespace Gu.Orm.Npgsql.Analyzers.Parsing
{
    using System.Collections.Immutable;

    public class SqlArgument : SqlNode
    {
        public SqlArgument(string sql, SqlExpression expression)
            : base(sql, ImmutableArray.Create<SqlNode>(expression))
        {
            this.Expression = expression;
        }

        public SqlExpression Expression { get; }
    }
}
