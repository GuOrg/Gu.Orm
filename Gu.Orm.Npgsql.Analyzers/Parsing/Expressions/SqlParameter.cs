namespace Gu.Orm.Npgsql.Analyzers.Parsing
{
    public class SqlParameter : SqlName
    {
        public SqlParameter(string sql, RawToken at, SqlSimpleName identifier)
            : base(sql, CreateChildren(identifier))
        {
            this.At = at.WithParent(this);
            this.Identifier = identifier;
        }

        public SqlToken At { get; }

        public SqlExpression Identifier { get; }

        public override bool IsValid => this.Identifier?.IsValid != false;

        public override string ToDisplayString() => $"{this.At.ToDisplayString(this.Sql)}{this.Identifier.ToDisplayString()}";
    }
}
