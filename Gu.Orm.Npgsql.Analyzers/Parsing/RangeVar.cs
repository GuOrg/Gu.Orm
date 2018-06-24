namespace Gu.Orm.Npgsql.Analyzers.Parsing
{
    public class RangeVar : SqlNode
    {
        public RangeVar(string sql, SqlName name, SqlName alias)
            : base(sql, CreateChildren(name, alias))
        {
            this.Name = name;
            this.Alias = alias;
        }

        public SqlName Name { get; }

        public SqlName Alias { get; }

        public override bool IsValid => this.Name?.IsValid == true &&
                                        this.Alias?.IsValid != false;

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
