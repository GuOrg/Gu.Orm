namespace Gu.Orm.Npgsql.Analyzers.Parsing
{
    using System.Collections.Immutable;

    public class SqlLiteralExpression : SqlExpression
    {
        public SqlLiteralExpression(string sql, RawToken token)
            : base(sql, ImmutableArray<SqlNode>.Empty)
        {
            this.Token = token.WithParent(this);
        }

        public SqlToken Token { get; }
    }
}
