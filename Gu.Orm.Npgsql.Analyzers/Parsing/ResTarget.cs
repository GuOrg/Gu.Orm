namespace Gu.Orm.Npgsql.Analyzers.Parsing
{
    public class ResTarget : SqlNode
    {
        public ResTarget(string sql, ColumnRef value, RawToken @as, SqlIdentifierName name)
            : base(sql, CreateChildren(value, name))
        {
            this.Value = value;
            this.As = @as.WithParent(this);
            this.Name = name;
        }

        public ColumnRef Value { get; }

        public SqlToken As { get; }

        public SqlIdentifierName Name { get; }

        public override string ToDisplayString()
        {
            return this.Name != null
                ? $"{this.Value.ToDisplayString()} {this.As.ToDisplayString(this.Sql)} {this.Name.ToDisplayString()}"
                : this.Value.ToDisplayString();
        }
    }
}
