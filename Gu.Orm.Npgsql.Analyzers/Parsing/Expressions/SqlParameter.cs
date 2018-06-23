namespace Gu.Orm.Npgsql.Analyzers.Parsing
{
    public class SqlParameter : SqlNameSyntax
    {
        public SqlParameter(string sql, RawToken at, SqlIdentifierName identifier)
            : base(sql, CreateChildren(identifier))
        {
            this.At = at.WithParent(this);
            this.Identifier = identifier;
        }

        public SqlToken At { get; }

        public SqlExpression Identifier { get; }

        public override string ToDisplayString() => $"{this.At.ToDisplayString(this.Sql)}{this.Identifier.ToDisplayString()}";
    }
}
