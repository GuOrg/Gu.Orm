namespace Gu.Orm.Npgsql.Analyzers.Parsing
{
    public class RangeVar : SqlNode
    {
        public RangeVar(string sql, SqlIdentifierName schemaName, SqlIdentifierName name, SqlIdentifierName alias)
            : base(sql, CreateChildren(schemaName, name, alias))
        {
            this.SchemaName = schemaName;
            this.Name = name;
            this.Alias = alias;
        }

        public SqlIdentifierName SchemaName { get; }

        public SqlIdentifierName Name { get; }

        public SqlIdentifierName Alias { get; }

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
