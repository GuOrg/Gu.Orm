namespace Gu.Orm.Npgsql.Analyzers.Parsing
{
    public class SqlInvocation : SqlExpression
    {
        public SqlInvocation(string sql, SqlNameSyntax name, SqlArgumentList argumentList)
            : base(sql, CreateChildren(name, argumentList))
        {
            this.Name = name;
            this.ArgumentList = argumentList;
        }

        public SqlNameSyntax Name { get; }

        public SqlArgumentList ArgumentList { get; }

        public override string ToDisplayString()
        {
            return $"{this.Name.ToDisplayString()}({this.ArgumentList?.ToDisplayString() ?? string.Empty})";
        }
    }
}
