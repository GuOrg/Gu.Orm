namespace Gu.Orm.Npgsql.Analyzers.Parsing
{
    public class SqlQualifiedName : SqlName
    {
        public SqlQualifiedName(string sql, SqlSimpleName prefix, RawToken dot, SqlSimpleName identifier)
        : base(sql, CreateChildren(prefix, identifier))
        {
            this.Prefix = prefix;
            this.Dot = dot.WithParent(this);
            this.Identifier = identifier;
        }

        public SqlSimpleName Prefix { get; }

        public SqlToken Dot { get; }

        public SqlSimpleName Identifier { get; }

        public override string ToDisplayString()
        {
            return this.Prefix != null
                ? $"{this.Prefix.ToDisplayString()}.{this.Identifier.ToDisplayString()}"
                : this.Identifier.ToDisplayString();
        }
    }
}
