namespace Gu.Orm.Npgsql.Analyzers.Parsing
{
    using System.Collections.Immutable;

    public class QuotedIdentiferName : SqlNameSyntax
    {
        public QuotedIdentiferName(string sql, RawToken openQuote, RawToken identifier, RawToken closeQuote)
            : base(sql, ImmutableArray<SqlNode>.Empty)
        {
            this.OpenQuote = openQuote.WithParent(this);
            this.Identifier = identifier.WithParent(this);
            this.CloseQuote = closeQuote.WithParent(this);
        }

        public SqlToken OpenQuote { get; }

        public SqlToken Identifier { get; }

        public SqlToken CloseQuote { get; }

        public override string ToDisplayString() => this.Identifier.ToDisplayString(this.Sql);
    }
}