namespace Gu.Orm.Npgsql.Analyzers.Parsing
{
    using System.Collections.Immutable;

    public class ResTarget : SqlNode
    {
        public ResTarget(SqlIdentifierName name, ColumnRef value)
            : base(ImmutableArray.Create<SqlNode>(name, value))
        {
            this.Name = name;
            this.Value = value;
        }

        public SqlIdentifierName Name { get; }

        public ColumnRef Value { get; }
    }
}
