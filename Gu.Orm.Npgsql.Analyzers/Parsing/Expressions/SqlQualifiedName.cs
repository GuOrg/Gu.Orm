namespace Gu.Orm.Npgsql.Analyzers.Parsing
{
    public class SqlQualifiedName : SqlNameSyntax
    {
        public SqlQualifiedName(string sql, SqlIdentifierName prefix, RawToken dot, SqlIdentifierName identifier)
        : base(sql, CreateChildren(prefix, identifier))
        {
            this.Prefix = prefix;
            this.Dot = dot.WithParent(this);
            this.Identifier = identifier;
        }

        public SqlIdentifierName Prefix { get; }

        public SqlToken Dot { get; }

        public SqlIdentifierName Identifier { get; }

        public override string ToDisplayString()
        {
            return this.Prefix != null
                ? $"{this.Prefix.ToDisplayString()}.{this.Identifier.ToDisplayString()}"
                : this.Identifier.ToDisplayString();
        }
    }
}
