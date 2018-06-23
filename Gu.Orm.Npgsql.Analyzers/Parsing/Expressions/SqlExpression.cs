namespace Gu.Orm.Npgsql.Analyzers.Parsing
{
    using System.Collections.Immutable;

    public abstract class SqlExpression : SqlNode
    {
        protected SqlExpression(string sql, ImmutableArray<SqlNode> children)
            : base(sql, children)
        {
        }
    }
}
