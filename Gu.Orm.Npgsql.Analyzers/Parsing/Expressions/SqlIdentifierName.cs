namespace Gu.Orm.Npgsql.Analyzers.Parsing
{
    using System.Collections.Immutable;

    public class SqlIdentifierName : SqlNameSyntax
    {
        public SqlIdentifierName(RawToken identifier)
            : base(ImmutableArray<SqlNode>.Empty)
        {
            this.Identifier = identifier.WithParent(this);
        }

        public SqlToken Identifier { get; }
    }
}
