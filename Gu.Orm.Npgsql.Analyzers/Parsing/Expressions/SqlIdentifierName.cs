namespace Gu.Orm.Npgsql.Analyzers.Parsing
{
    using System.Collections.Immutable;

    public class SqlIdentifierName : SqlSimpleName
    {
        public SqlIdentifierName(string sql, RawToken identifier)
            : base(sql, ImmutableArray<SqlNode>.Empty)
        {
            this.Identifier = identifier.WithParent(this);
        }

        public SqlToken Identifier { get; }

        public override bool IsValid => this.Identifier.Kind == SqlKind.Identifier;

        public override string ToDisplayString() => this.Identifier.ToDisplayString(this.Sql);
    }
}
