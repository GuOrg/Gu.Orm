namespace Gu.Orm.Npgsql.Analyzers.Parsing
{
    public class RangeVar : SqlNode
    {
        public RangeVar(string sql, SqlNameSyntax name, SqlNameSyntax alias)
            : base(sql, CreateChildren(name, alias))
        {
            this.Name = name;
            this.Alias = alias;
        }

        public SqlNameSyntax Name { get; }

        public SqlNameSyntax Alias { get; }

        public override string ToDisplayString()
        {
            if (this.Alias != null)
            {
                return $"{this.Name.ToDisplayString()} {this.Alias.ToDisplayString()}";
            }

            return this.Name.ToDisplayString();
        }
    }
}
