namespace Gu.Orm.Npgsql.Analyzers.Parsing
{
    using System.Collections.Immutable;

    public abstract class SqlNameSyntax : SqlExpression
    {
        protected SqlNameSyntax(ImmutableArray<SqlNode> children)
            : base(children)
        {
        }
    }
}
