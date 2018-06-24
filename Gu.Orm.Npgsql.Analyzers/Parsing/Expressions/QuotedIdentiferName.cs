namespace Gu.Orm.Npgsql.Analyzers.Parsing
{
    using System.Collections.Immutable;

    public class QuotedIdentiferName : SqlNameSyntax
    {
        public QuotedIdentiferName(string sql, RawToken identifier)
            : base(sql, ImmutableArray<SqlNode>.Empty)
        {
            this.Identifier = identifier.WithParent(this);
        }

        public SqlToken Identifier { get; }

        public override string ToDisplayString() => this.Identifier.ToDisplayString(this.Sql);
    }
}
