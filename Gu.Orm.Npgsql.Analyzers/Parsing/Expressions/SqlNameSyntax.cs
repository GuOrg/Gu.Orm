namespace Gu.Orm.Npgsql.Analyzers.Parsing
{
    using System.Collections.Immutable;

    public abstract class SqlNameSyntax : SqlExpression
    {
        protected SqlNameSyntax(string sql, ImmutableArray<SqlNode> children)
            : base(sql, children)
        {
        }
    }
}
