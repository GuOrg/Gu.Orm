namespace Gu.Orm.Npgsql.Analyzers.Parsing
{
    using System.Collections.Immutable;

    public abstract class SqlSimpleName : SqlName
    {
        protected SqlSimpleName(string sql, ImmutableArray<SqlNode> children)
            : base(sql, children)
        {
        }
    }
}