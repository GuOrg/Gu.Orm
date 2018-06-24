namespace Gu.Orm.Npgsql.Analyzers.Parsing
{
    using System.Collections.Immutable;

    public abstract class SqlName : SqlExpression
    {
        protected SqlName(string sql, ImmutableArray<SqlNode> children)
            : base(sql, children)
        {
        }
    }
}
