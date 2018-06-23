namespace Gu.Orm.Npgsql.Analyzers.Parsing
{
    using System.Collections.Immutable;

    public class RangeVar : SqlNode
    {
        public RangeVar(SqlIdentifierName schemaName, SqlIdentifierName name, SqlIdentifierName alias)
            : base(ImmutableArray.Create<SqlNode>(schemaName, name, alias))
        {
            this.SchemaName = schemaName;
            this.Name = name;
            this.Alias = alias;
        }

        public SqlIdentifierName SchemaName { get; }

        public SqlIdentifierName Name { get; }

        public SqlIdentifierName Alias { get; }
    }
}
