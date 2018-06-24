namespace Gu.Orm.Npgsql.Analyzers.Parsing
{
    using System.Collections.Immutable;

    public class Star : SqlExpression
    {
        public Star(string sql, RawToken token)
            : base(sql, ImmutableArray<SqlNode>.Empty)
        {
            this.Token = token.WithParent(this);
        }

        public SqlToken Token { get; }

        public override bool IsValid => this.Token.Kind == SqlKind.Multiply;

        public override string ToDisplayString() => "*";
    }
}
