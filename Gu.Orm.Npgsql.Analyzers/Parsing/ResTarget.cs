namespace Gu.Orm.Npgsql.Analyzers.Parsing
{
    public class ResTarget : SqlNode
    {
        public ResTarget(string sql, SqlExpression value, RawToken @as, SqlSimpleName name)
            : base(sql, CreateChildren(value, name))
        {
            this.Value = value;
            this.As = @as.WithParent(this);
            this.Name = name;
        }

        public SqlExpression Value { get; }

        public SqlToken As { get; }

        public SqlSimpleName Name { get; }

        public override bool IsValid
        {
            get
            {
                if (this.As.Kind == SqlKind.None)
                {
                    return this.Value?.IsValid == true &&
                           this.Name == null;
                }

                return this.Value?.IsValid == true &&
                       this.As.Kind == SqlKind.Identifier &&
                       this.Name?.IsValid == true;
            }
        }

        public override string ToDisplayString()
        {
            return this.Name != null
                ? $"{this.Value.ToDisplayString()} {this.As.ToDisplayString(this.Sql)} {this.Name.ToDisplayString()}"
                : this.Value.ToDisplayString();
        }
    }
}
