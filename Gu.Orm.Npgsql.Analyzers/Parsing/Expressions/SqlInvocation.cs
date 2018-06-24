namespace Gu.Orm.Npgsql.Analyzers.Parsing
{
    public class SqlInvocation : SqlExpression
    {
        public SqlInvocation(string sql, SqlNameSyntax name, RawToken openParen, SqlArgumentList argumentList, RawToken closeParen)
            : base(sql, CreateChildren(name, argumentList))
        {
            this.Name = name;
            this.OpenParen = openParen.WithParent(this);
            this.ArgumentList = argumentList;
            this.CloseParen = closeParen.WithParent(this);
        }

        public SqlNameSyntax Name { get; }

        public SqlToken OpenParen { get; }

        public SqlArgumentList ArgumentList { get; }

        public SqlToken CloseParen { get; }

        public override string ToDisplayString()
        {
            return $"{this.Name.ToDisplayString()}({this.ArgumentList?.ToDisplayString() ?? string.Empty})";
        }
    }
}
