namespace Gu.Orm.Npgsql.Analyzers.Parsing
{
    using System.Collections.Immutable;

    public class SqlLiteral : SqlExpression
    {
        public SqlLiteral(string sql, RawToken token)
            : base(sql, ImmutableArray<SqlNode>.Empty)
        {
            this.Token = token.WithParent(this);
        }

        public SqlToken Token { get; }

        public override string ToDisplayString() => this.Token.ToDisplayString(this.Sql);
    }
}
