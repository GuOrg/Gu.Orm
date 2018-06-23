namespace Gu.Orm.Npgsql.Analyzers.Parsing
{
    using System.Collections.Immutable;

    public class SqlLiteralExpression : SqlExpression
    {
        public SqlLiteralExpression(RawToken token)
            : base(ImmutableArray<SqlNode>.Empty)
        {
            this.Token = token.WithParent(this);
        }

        public SqlToken Token { get; }
    }
}
