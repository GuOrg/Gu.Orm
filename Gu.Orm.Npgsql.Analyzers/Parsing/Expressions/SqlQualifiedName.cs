namespace Gu.Orm.Npgsql.Analyzers.Parsing
{
    using System.Collections.Immutable;

    public class SqlQualifiedName : SqlNameSyntax
    {
        public SqlQualifiedName(string sql, SqlIdentifierName prefix, RawToken dot, SqlIdentifierName identifier)
        : base(sql, ImmutableArray.Create<SqlNode>(prefix, identifier))
        {
            this.Prefix = prefix;
            this.Dot = dot.WithParent(this);
            this.Identifier = identifier;
        }

        public SqlIdentifierName Prefix { get; }

        public SqlToken Dot { get; }

        public SqlIdentifierName Identifier { get; }

        public override string ToString() => $"{this.Prefix}.{this.Identifier}";
    }
}
