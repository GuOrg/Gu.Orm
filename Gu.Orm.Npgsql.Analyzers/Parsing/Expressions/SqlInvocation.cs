namespace Gu.Orm.Npgsql.Analyzers.Parsing
{
    public class SqlInvocation : SqlExpression
    {
        public SqlInvocation(string sql, SqlName name, RawToken openParen, SqlArgumentList argumentList, RawToken closeParen)
            : base(sql, CreateChildren(name, argumentList))
        {
            this.Name = name;
            this.OpenParen = openParen.WithParent(this);
            this.ArgumentList = argumentList;
            this.CloseParen = closeParen.WithParent(this);
        }

        public SqlName Name { get; }

        public SqlToken OpenParen { get; }

        public SqlArgumentList ArgumentList { get; }

        public SqlToken CloseParen { get; }

        public override bool IsValid => this.Name?.IsValid == true &&
                                        this.OpenParen.Kind == SqlKind.OpenParen &&
                                        this.ArgumentList?.IsValid != false &&
                                        this.CloseParen.Kind == SqlKind.CloseParen;

        public override string ToDisplayString()
        {
            return $"{this.Name.ToDisplayString()}({this.ArgumentList?.ToDisplayString() ?? string.Empty}{this.CloseParen.ToDisplayString(this.Sql)}";
        }
    }
}
