namespace Gu.Orm.Npgsql.Analyzers.Parsing
{
    using System.Collections.Immutable;

    public abstract class SqlExpression : SqlNode
    {
        protected SqlExpression(ImmutableArray<SqlNode> children)
            : base(children)
        {
        }
    }
}
