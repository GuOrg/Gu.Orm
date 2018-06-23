namespace Gu.Orm.Npgsql.Analyzers.Parsing
{
    using System.Collections.Immutable;

    public class SqlIdentifierName : SqlNameSyntax
    {
        public SqlIdentifierName(string sql, RawToken identifier)
            : base(sql, ImmutableArray<SqlNode>.Empty)
        {
            this.Identifier = identifier.WithParent(this);
        }

        public SqlToken Identifier { get; }

        public override string ToString() => this.Sql.Substring(this.Identifier.Start, this.Identifier.Length);
    }
}
