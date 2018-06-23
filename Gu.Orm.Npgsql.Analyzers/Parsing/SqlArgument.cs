namespace Gu.Orm.Npgsql.Analyzers.Parsing
{
    using System.Collections.Immutable;

    public class SqlArgument : SqlNode
    {
        public SqlArgument(SqlExpression expression)
            : base(ImmutableArray.Create<SqlNode>(expression))
        {
            this.Expression = expression;
        }

        public SqlExpression Expression { get; }
    }
}
